using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using travelManagement.Models.Entity;
using travelManagement.Service;
using travelManagement.Service.Impl;

namespace travelManagement.Controllers
{
    public class TravelDetailController : Controller
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
        TransportationService _transportationService;

        public TravelDetailController()
        {
            _transportationService = new TransportationServiceImpl();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetTravelReservationSeatList(int travelStepId)
        {

            // 旅遊梯次詳情與交通綁定資訊
            TripStepTransportMatchModel travelStepTransportMatch = this.getTravelStepInfo(travelStepId);
            GetReservationSeatListResponse getReservationSeatListApi = new GetReservationSeatListResponse();
            String transportationIds = travelStepTransportMatch.transportationIds;
            String[] transportationIdArr = transportationIds == null ? new String[0] : transportationIds.Split(',');

            List<TransportationEntity> transportationEntities = await _transportationService.GetTransportationList();

            // 取得陣列堆疊
            Stack<string> transportationIdArrStack = new Stack<string>(transportationIdArr);

            //// 後進先出，去掉最後一個值
            //transportationIdArrStack.Pop();
            String[] transportationIdArrNew = transportationIdArr.ToArray();
            List<ReservationSeatModel> reservationSeatList = new List<ReservationSeatModel>();
            List<TransportationModel> transportationList = new List<TransportationModel>();
            int transportationStep = 1;
            List<int> seatRetainCount = new List<int>();
            foreach (var transportationId in transportationIdArr)
            {
                if (string.IsNullOrEmpty(transportationId))
                {
                    continue;
                }
                int useStatus = 1;
                String transportationCode = "";
                Console.WriteLine("{0}", transportationId);
                ReservationSeatModel reservationSeatData = new ReservationSeatModel();
                TransportationModel transportationListData = new TransportationModel();
                List<TripReservationSeatMatchModel> reservationSeatMatch = this.getTravelSeatList(transportationId, travelStepTransportMatch.travelStepId);
                Console.WriteLine("{0}", travelStepTransportMatch.travelStepId);
                int retainSeat = 0;
                Console.WriteLine("{0}", retainSeat);
                foreach (TripReservationSeatMatchModel reservationSeatMatchTmp in reservationSeatMatch)
                {
                    if (reservationSeatMatchTmp.seatStatus.Equals(1))
                    {
                        retainSeat = retainSeat + 1;
                    }
                }
                foreach (TransportationEntity transportationEntity in transportationEntities)
                {
                    if (transportationEntity.transportationId.Equals(Convert.ToInt16(transportationId)))
                    {
                        transportationCode = transportationEntity.transportationLicensesNumber;
                    }
                }
                seatRetainCount.Add(retainSeat);
                //if ((transportationStep == 1 && retainSeat >= 0) || ((transportationStep - 2) >= 0 && seatRetainCount[transportationStep - 2] <= 5))
                //{
                //    useStatus = 1;
                //}
                reservationSeatData.transportationId = Convert.ToInt16(transportationId);
                reservationSeatData.transportationStep = transportationStep;
                reservationSeatData.transportationCode = transportationCode;
                Console.WriteLine("{0}", transportationStep);
                reservationSeatData.reservationSeatList = reservationSeatMatch;
                reservationSeatData.useStatus = useStatus;
                transportationListData.transportationId = Convert.ToInt16(transportationId);
                transportationListData.transportationStep = transportationStep;
                transportationListData.remainSeatNum = retainSeat.ToString();
                transportationListData.transportationCode = transportationCode;
                reservationSeatList.Add(reservationSeatData);
                transportationList.Add(transportationListData);
                transportationStep++;
            }
            string format = "yyyy-MM-dd";
            string startDate = "";
            if (travelStepTransportMatch.travelStime != null)
            {
                DateTime startDateTime = DateTime.Parse(travelStepTransportMatch.travelStime);
                startDate = startDateTime.ToString(format);
            }
            getReservationSeatListApi.success = 1;
            getReservationSeatListApi.travelStepId = travelStepId;
            getReservationSeatListApi.travelStepCode = travelStepTransportMatch.travelStepCode;
            getReservationSeatListApi.startDate = startDate;
            getReservationSeatListApi.reservationSeatList = reservationSeatList;
            getReservationSeatListApi.transportationList = transportationList;
            return Json(getReservationSeatListApi);
        }


        // 取得旅遊梯次乘車詳情
        public TripStepTransportMatchModel getTravelStepInfo(int travelStepId)
        {
            // SQL Command
            SqlConnection connection = new SqlConnection(this.sql_DB);
            string fieldSql = "tsl.travel_step_id as travelStepId, tsl.travel_step_code as travelStepCode, tsl.travel_cost as travelCost, tsl.travel_num as travelNum, tsl.travel_s_time as travelStime, tsl.travel_e_time as travelEtime, tsl.travel_id as travelId, tsl.sell_seat_num as sellSeatNum, tsl.remain_seat_num as remainSeatNum, ttml.transportation_ids as transportationIds";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from travel_step_list as tsl inner join travel_transportation_match_list ttml ON ttml.travel_step_id = tsl.travel_step_id WHERE tsl.travel_step_id = @travelStepId and tsl.status = 1", connection);
            select.Parameters.AddWithValue("@travelStepId", travelStepId);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            TripStepTransportMatchModel travelStepTransportMatch = new TripStepTransportMatchModel();
            while (reader.Read())
            {
                travelStepTransportMatch.travelStepId = (int)reader[0];
                travelStepTransportMatch.travelStepCode = reader.IsDBNull(1) ? "" : (string)reader[1];
                travelStepTransportMatch.travelCost = reader.IsDBNull(2) ? 0 : (int)reader[2];
                travelStepTransportMatch.travelNum = reader.IsDBNull(3) ? 0 : (int)reader[3];
                string format = "yyyy-MM-dd HH:mm:ss";
                travelStepTransportMatch.travelStime = ((DateTime)reader[4]).ToString(format);
                travelStepTransportMatch.travelEtime = ((DateTime)reader[5]).ToString(format);
                travelStepTransportMatch.travelId = reader.IsDBNull(6) ? 0 : (int)reader[6];
                travelStepTransportMatch.sellSeatNum = reader.IsDBNull(7) ? 0 : (int)reader[7];
                travelStepTransportMatch.remainSeatNum = reader.IsDBNull(8) ? 0 : (int)reader[8];
                travelStepTransportMatch.transportationIds = reader.IsDBNull(9) ? "" : (string)reader[9];
            }
            return travelStepTransportMatch;
        }

        // 取得旅遊梯次乘車詳情
        public List<TripReservationSeatMatchModel> getTravelSeatList(string transportationId, int travelStepId)
        {
            // SQL Command
            SqlConnection connection = new SqlConnection(this.sql_DB);
            string fieldSql = "select sl.seat_id as seatId, spl.seat_pos_name as seatName, stml.status, mrl.id, mrl.name, mrl.phone, bl.boarding_datetime as boardingDatetime, ll.location_name as locationName from seat_list AS sl INNER JOIN seat_pos_list spl ON spl.seat_pos = sl.seat_pos LEFT JOIN seat_travel_match_list stml ON stml.seat_id = sl.seat_id and stml.travel_step_id = @travelStepId and stml.status >= 0 left join member_reservation_list mrl ON stml.seat_id = mrl.seat_id and mrl.travel_step_id = stml.travel_step_id and mrl.status = 1 left join boarding_list bl ON bl.boarding_id = mrl.boarding_id left join location_list ll ON ll.location_id = bl.location_id WHERE sl.transportation_id = @transportationId  order by sl.transportation_id ASC, sl.seat_pos ASC";
            SqlCommand select = new SqlCommand(fieldSql, connection);
            select.Parameters.AddWithValue("@transportationId", transportationId);
            select.Parameters.AddWithValue("@travelStepId", travelStepId);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<TripReservationSeatMatchModel> reservationSeatList = new List<TripReservationSeatMatchModel>();
            while (reader.Read())
            {
                TripReservationSeatMatchModel reservationSeatMatch = new TripReservationSeatMatchModel();
                reservationSeatMatch.seatId = (int)reader[0];
                reservationSeatMatch.seatName = reader.IsDBNull(1) ? "" : (string)reader[1];
                reservationSeatMatch.seatStatus = reader.IsDBNull(2) ? 1 : (int)reader[2];
                reservationSeatMatch.memberIdCode = reader.IsDBNull(3) ? "" : (string)reader[3];
                reservationSeatMatch.memberName = reader.IsDBNull(4) ? "" : (string)reader[4];
                reservationSeatMatch.memberPhone = reader.IsDBNull(5) ? "" : (string)reader[5];
                string format = "HH:mm";
                reservationSeatMatch.boardingDatetime = reader.IsDBNull(6) ? "" : ((DateTime)reader[6]).ToString(format);
                reservationSeatMatch.locationName = reader.IsDBNull(7) ? "" : (string)reader[7];
                reservationSeatList.Add(reservationSeatMatch);
            }
            return reservationSeatList;
        }


        public class ReservationSeatModel
        {
            public int transportationId { get; set; }
            public int transportationStep { get; set; }
            public string transportationCode { get; set; }
            public int useStatus { get; set; }
            public List<TripReservationSeatMatchModel> reservationSeatList { get; set; }
            public static explicit operator ReservationSeatModel(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        public class TransportationModel
        {
            public int transportationId { get; set; }
            public int transportationStep { get; set; }
            public string transportationCode { get; set; }
            public string remainSeatNum { get; set; }
            public static explicit operator TransportationModel(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        public class TripReservationSeatMatchModel
        {
            public int seatId { get; set; }
            public string seatName { get; set; }
            public int seatStatus { get; set; }
            public string memberIdCode { get; set; }
            public string memberName { get; set; }
            public string memberPhone { get; set; }
            public string boardingDatetime { get; set; }
            public string locationName { get; set; }
            public static explicit operator TripReservationSeatMatchModel(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        public class TripStepTransportMatchModel
        {
            public int travelStepId { get; set; }
            public string travelStepCode { get; set; }
            public int travelCost { get; set; }
            public int travelNum { get; set; }
            public string travelStime { get; set; }
            public string travelEtime { get; set; }
            public int travelId { get; set; }
            public int sellSeatNum { get; set; }
            public int remainSeatNum { get; set; }
            public string transportationIds { get; set; }
            public static explicit operator TripStepTransportMatchModel(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        public class GetReservationSeatListResponse
        {
            public int success { get; set; }
            public int travelStepId { get; set; }
            public string travelCode { get; set; }
            public string travelStepCode { get; set; }
            public string startDate { get; set; }
            public List<ReservationSeatModel> reservationSeatList { get; set; }
            public List<TransportationModel> transportationList { get; set; }
            public int status { get; set; }
            public static explicit operator GetReservationSeatListResponse(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }
    }
}
