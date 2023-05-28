using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using XiaoTasiBackend.Models;
using XiaoTasiBackend.Models.Dto;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Service;
using XiaoTasiBackend.Service.Impl;
using static XiaoTasiBackend.Controllers.TravelController;

namespace XiaoTasiBackend.Controllers
{
    public class ReservationController : Controller
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        TransportationService _transportationService;
        ReservationService _reservationService;
        SeatService _seatService;
        OrderController orderController;
        MemberIndexController memberIndexController;
        TravelController travelController;

        public ReservationController()
        {
            _transportationService = new TransportationServiceImpl();
            _reservationService = new ReservationServiceImpl();
            _seatService = new SeatServiceImpl();
            orderController = new OrderController();
            memberIndexController = new MemberIndexController();
            travelController = new TravelController();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetReservationList(int draw = 1, int start = 0, int length = 0)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = " rl.reservation_id as reservationId,  rl.reservation_code as reservationCode,  rl.member_code as memberCode,  rl.travel_id as travelId,  rl.reservation_num as reservationNum, rl.reservation_cost as reservationCost, rl.status, rcl.reservation_check_pic_path as reservationCheckPicPath, ml.name as memberName";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from reservation_list rl left join reservation_check_list rcl ON rcl.reservation_code = rl.reservation_code and rcl.member_code = rl.member_code and rcl.status = 1 inner join member_list ml ON ml.member_code = rl.member_code", connection);
            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            PageControl<ReservationData> pageControl = new PageControl<ReservationData>();
            List<ReservationData> reservationDatas = new List<ReservationData>();
            while (reader.Read())
            {
                ReservationData reservationData = new ReservationData();
                reservationData.reservationId = (int)reader[0];
                reservationData.reservationCode = reader.IsDBNull(1) ? "" : (string)reader[1];
                reservationData.memberCode = (string)reader[2];
                reservationData.travelId = (int)reader[3];
                reservationData.reservationNum = (int)reader[4];
                reservationData.reservationCost = (int)reader[5];
                reservationData.status = (int)reader[6];
                reservationData.reservationCheckPicPath = reader.IsDBNull(7) ? "" : (string)reader[7];
                reservationData.memberName = reader.IsDBNull(8) ? "" : (string)reader[8];
                reservationDatas.Add(reservationData);
            }
            connection.Close();

            List<ReservationData> reservationListNew = pageControl.pageControl((start + 1), length, reservationDatas);

            // 整理回傳內容
            return Json(new ApiReturn<List<ReservationData>>(1, draw, pageControl.size, pageControl.size, reservationDatas));
        }

        [HttpPost]
        public async Task<ActionResult> GetReservationListTest(int draw = 1, int start = 0, int length = 0)
        {

            List<ReservationDto> reservationDtos = await _reservationService.GetReservationList();

            PageControl<ReservationDto> pageControl = new PageControl<ReservationDto>();

            List<ReservationDto> reservationListNew = pageControl.pageControl((start + 1), length, reservationDtos);

            // 整理回傳內容
            return Json(new ApiReturn<List<ReservationDto>>(1, draw, pageControl.size, pageControl.size, reservationDtos));
        }

        [HttpPost]
        public async Task<ActionResult> GetReservationInfo(int reservationId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "rl.reservation_id as reservationId, rl.reservation_code as reservationCode, rl.member_code as memberCode, rl.travel_id as travelId, rl.reservation_num as reservationNum, rl.reservation_cost as reservationCost, rl.status, rcl.reservation_check_pic_path as reservationCheckPicPath";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from reservation_list rl left join reservation_check_list rcl ON rcl.reservation_code = rl.reservation_code and rcl.member_code = rl.member_code and rcl.status = 1 WHERE rl.reservation_id = @reservationId", connection);
            select.Parameters.AddWithValue("@reservationId", reservationId);
            // 開啟資料庫連線
            await connection.OpenAsync();
            ReservationData reservationData = new ReservationData();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                reservationData.reservationId = (int)reader[0];
                reservationData.reservationCode = (string)reader[1];
                reservationData.memberCode = (string)reader[2];
                reservationData.travelId = (int)reader[3];
                reservationData.reservationNum = (int)reader[4];
                reservationData.reservationCost = (int)reader[5];
                reservationData.status = (int)reader[6];
                reservationData.reservationCheckPicPath = reader.IsDBNull(7) ? "" : (string)reader[7];
            }
            connection.Close();
            return Json(new ApiResult<object>(reservationData));
        }

        [HttpPost]
        public ActionResult UpdateReservationInfo(int reservationId, string reservationName)
        {

            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE reservation_list SET name = @reservationName WHERE reservation_id = @reservationId", connection);
            select.Parameters.AddWithValue("@reservationId", reservationId);
            select.Parameters.Add("@reservationName", SqlDbType.Text).Value = reservationName;
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();

            return Json(new ApiResult<string>("Update Success", "更新成功"));
        }

        [HttpPost]
        public ActionResult DeleteReservation(int reservationId)
        {

            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("DELETE reservation_list WHERE reservation_id = @reservationId", connection);
            select.Parameters.AddWithValue("@reservationId", reservationId);
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();

            return Json(new ApiResult<string>("Delete Success", "刪除成功"));
        }


        //群組資訊物件
        public class ReservationData
        {
            public int reservationId { get; set; }
            public string reservationCode { get; set; }
            public string memberCode { get; set; }
            public int travelId { get; set; }
            public int reservationNum { get; set; }
            public int reservationCost { get; set; }
            public int travelStepId { get; set; }
            public int status { get; set; }
            public string reservationCheckPicPath { get; set; }
            public string memberName { get; set; }
            public string seatIds { get; set; }
            public static explicit operator ReservationData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //取得預訂座位資訊(API回傳參數格式)
        public class GetReservationSeatListApi
        {
            public int success { get; set; }
            public string travelCode { get; set; }
            public int travelStep { get; set; }
            public string startDate { get; set; }
            public List<ReservationListData> reservationList { get; set; }
            public List<TransportationListData> transportationList { get; set; }
            public int status { get; set; }
            public static explicit operator GetReservationSeatListApi(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //取得旅遊預訂資訊(API回傳參數格式)
        public class GetTravelReservationInfoApi
        {
            public int success { get; set; }
            public int payStatus { get; set; }
            public TravelReservationInfoData travelReservationInfo { get; set; }
            public List<MemberReservationListData> memberReservationList { get; set; }
            public static explicit operator GetTravelReservationInfoApi(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //取得旅遊預訂列表(API回傳參數格式)
        public class GetTravelReservationListApi
        {
            public int success { get; set; }
            public int count { get; set; }
            public int page { get; set; }
            public int limit { get; set; }
            public List<TravelReservationInfoData> travelReservationList { get; set; }
            public static explicit operator GetTravelReservationListApi(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //取得旅遊匯款列表(API回傳參數格式)
        public class GetTravelReservationCheckListApi
        {
            public int success { get; set; }
            public int count { get; set; }
            public int page { get; set; }
            public int limit { get; set; }
            public List<ReservationCheckData> travelReservationCheckList { get; set; }
            public static explicit operator GetTravelReservationCheckListApi(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //預定乘車資訊列表
        public class TransportationListData
        {
            public int transportationStep { get; set; }
            public string transportationCode { get; set; }
            public string remainSeatNum { get; set; }
            public static explicit operator TransportationListData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }
        //預約座位列表
        public class ReservationSeatListData
        {
            public int seatId { get; set; }
            public int seatStatus { get; set; }
            public static explicit operator ReservationSeatListData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //預定資訊列表
        public class ReservationListData
        {
            public int transportationStep { get; set; }
            public List<ReservationSeatMatch> reservationSeatList { get; set; }
            public static explicit operator ReservationListData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //旅遊信息
        public class TravelReservationInfoData
        {
            public string travelReservationCode { get; set; }
            public string travelCode { get; set; }
            public int travelType { get; set; }
            public int travelStep { get; set; }
            public string travelTraditionalTitle { get; set; }
            public string travelEnTitle { get; set; }
            public string travelTraditionalContent { get; set; }
            public int dayNum { get; set; }
            public int cost { get; set; }
            public string startDate { get; set; }
            public string travelStepCode { get; set; }
            public int payStatus { get; set; }
            public string orderName { get; set; }
            public string memberCode { get; set; }
            public static explicit operator TravelReservationInfoData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //成員訂位資訊
        public class MemberReservationListData
        {
            public string id { get; set; }
            public string name { get; set; }
            public string phone { get; set; }
            public string birthday { get; set; }
            public int mealsType { get; set; }
            public int boardingLocationId { get; set; }
            public int transportationStep { get; set; }
            public int seatId { get; set; }
            public string seatName { get; set; }
            public string transportationLicenesNumber { get; set; }
            public static explicit operator MemberReservationListData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }


        //取得旅遊梯次資訊
        public class TravelStepInfo
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
            public static explicit operator TravelStepInfo(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //預定車位綁定資訊
        public class ReservationSeatMatch
        {
            public int seatId { get; set; }
            public string seatName { get; set; }
            public int seatStatus { get; set; }
            public static explicit operator ReservationSeatMatch(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //預定車位綁定資訊
        public class TravelStepTransportMatch
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
            public static explicit operator TravelStepTransportMatch(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }


        //建立預定預設資訊
        public class CreateTravelReservationBo
        {
            public string token { get; set; }
            public string travelStepCode { get; set; }
            public List<MemberReservationArrBo> memberReservationArr { get; set; }
            public string note { get; set; }
            public static explicit operator CreateTravelReservationBo(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //建立預定預設資訊
        public class CreateTravelReservationAPI
        {
            public int success { get; set; }
            public string reservationCode { get; set; }
        }

        //成員訂位資訊
        public class MemberReservationArrBo
        {
            public string id { get; set; }
            public string name { get; set; }
            public string phone { get; set; }
            public string birthday { get; set; }
            public int mealsType { get; set; }
            public int boardingLocationId { get; set; }
            public int transportationStep { get; set; }
            public int seatId { get; set; }
            public static explicit operator MemberReservationArrBo(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //成員訂位匯款資訊
        public class ReservationCheckData
        {
            public string reservationCode { get; set; }
            public string memberCode { get; set; }
            public string bankAccountName { get; set; }
            public string bankAccountCode { get; set; }
            public int checkStatus { get; set; }
            public string fDate { get; set; }
            public string eDate { get; set; }
            public static explicit operator ReservationCheckData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //成員訂位匯款資訊
        public class MemberReservationData
        {
            public string transportationCode { get; set; }
            public int seatPos { get; set; }
            public int travelType { get; set; }
            public int travelStepId { get; set; }
            public string reservationMemberCode { get; set; }
            public string memberName { get; set; }
            public string memberBirthday { get; set; }
            public string memberIdCode { get; set; }
            public string memberPhone { get; set; }
            public int roomsType { get; set; }
            public int foodsType { get; set; }
            public string memo { get; set; }
            public int boardingId { get; set; }
            public static explicit operator MemberReservationData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        // 取得旅遊梯次乘車詳情
        public TravelStepTransportMatch getTraveStepInfo(string travelStepCode)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "tsl.travel_step_id as travelStepId, tsl.travel_step_code as travelStepCode, tsl.travel_cost as travelCost, tsl.travel_num as travelNum, tsl.travel_s_time as travelStime, tsl.travel_e_time as travelEtime, tsl.travel_id as travelId, tsl.sell_seat_num as sellSeatNum, tsl.remain_seat_num as remainSeatNum, ttml.transportation_ids as transportationIds";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from travel_step_list as tsl inner join travel_transportation_match_list ttml ON  ttml.travel_step_id = tsl.travel_step_id WHERE tsl.travel_step_code = @travelStepCode", connection);
            select.Parameters.AddWithValue("@travelStepCode", travelStepCode);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            TravelStepTransportMatch travelStepTransportMatch = new TravelStepTransportMatch();
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
        public List<ReservationSeatMatch> getTraveSeatList(string transportationId, int travelStepId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            //string fieldSql = "select sl.seat_id as seatId, sl.seat_name as seatName, stml.status from seat_list AS sl INNER JOIN seat_travel_match_list stml ON stml.seat_id = sl.seat_id WHERE sl.transportation_id = @transportationId and stml.travel_step_id = @travelStepId";
            string fieldSql = "select sl.seat_id as seatId, sl.seat_name as seatName, stml.status from seat_list AS sl INNER JOIN seat_travel_match_list stml ON stml.seat_id = sl.seat_id WHERE sl.transportation_id = @transportationId";
            SqlCommand select = new SqlCommand(fieldSql, connection);
            select.Parameters.AddWithValue("@transportationId", transportationId);
            //select.Parameters.AddWithValue("@travelStepId", travelStepId);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<ReservationSeatMatch> reservationSeatList = new List<ReservationSeatMatch>();
            while (reader.Read())
            {
                ReservationSeatMatch reservationSeatMatch = new ReservationSeatMatch();
                reservationSeatMatch.seatId = (int)reader[0];
                reservationSeatMatch.seatName = reader.IsDBNull(1) ? "" : (string)reader[1];
                reservationSeatMatch.seatStatus = reader.IsDBNull(2) ? 0 : (int)reader[2];
                reservationSeatList.Add(reservationSeatMatch);
            }
            return reservationSeatList;
        }

        // 取得會員旅遊訂單列表
        [HttpPost]
        public ActionResult getTravelReservationList(string memberCode, int page, int limit)
        {

            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "select rl.reservation_code as travelReservationCode, tl.travel_code as travelCode, stl.travel_step_code as travelStepCode, rl.reservation_cost as cost, stl.travel_s_time as startDate, tl.travel_name as travelTraditionalTitle, rl.status as payStatus from reservation_list AS rl INNER JOIN travel_step_list stl ON stl.travel_step_id = rl.travel_step_id INNER JOIN travel_list tl ON tl.travel_id = stl.travel_id WHERE rl.member_code = @memberCode";
            SqlCommand select = new SqlCommand(fieldSql, connection);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<TravelReservationInfoData> travelReservationInfoDatas = new List<TravelReservationInfoData>();
            PageControl<TravelReservationInfoData> pageControl = new PageControl<TravelReservationInfoData>();
            GetTravelReservationListApi getTravelReservationListApi = new GetTravelReservationListApi();
            String name = memberIndexController._getMemberInfo(memberCode).name;
            while (reader.Read())
            {
                TravelReservationInfoData travelReservationInfo = new TravelReservationInfoData();
                travelReservationInfo.travelReservationCode = reader.IsDBNull(0) ? "" : (string)reader[0];
                travelReservationInfo.travelCode = reader.IsDBNull(1) ? "" : (string)reader[1];
                travelReservationInfo.travelStepCode = reader.IsDBNull(2) ? "" : (string)reader[2];
                travelReservationInfo.cost = reader.IsDBNull(3) ? 0 : (int)reader[3];
                string format = "yyyy-MM-dd HH:mm:ss";
                travelReservationInfo.startDate = ((DateTime)reader[4]).ToString(format);
                travelReservationInfo.travelTraditionalTitle = reader.IsDBNull(5) ? "" : (string)reader[5];
                travelReservationInfo.payStatus = reader.IsDBNull(6) ? 0 : (int)reader[6];
                travelReservationInfo.orderName = name;
                travelReservationInfo.memberCode = memberCode;
                travelReservationInfoDatas.Add(travelReservationInfo);
            }
            List<TravelReservationInfoData> travelReservationInfoDatasNew = pageControl.pageControl(page, limit, travelReservationInfoDatas);
            getTravelReservationListApi.success = 1;
            getTravelReservationListApi.count = pageControl.size;
            getTravelReservationListApi.page = page;
            getTravelReservationListApi.limit = limit;
            getTravelReservationListApi.travelReservationList = travelReservationInfoDatasNew;
            return Json(getTravelReservationListApi);
        }

        // 取得會員旅遊訂單資訊
        public List<MemberReservationListData> _memberReservationList(string memberCode, string travelReservationCode)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "select mrl.id, mrl.name, mrl.birthday, mrl.phone, mrl.meals_type as mealsType, sl.seat_id as seatId, sl.seat_name as seatName, tl.transportation_licenses_number as transportationLicenesNumber from  member_reservation_list mrl INNER JOIN seat_list sl ON sl.seat_id = mrl.seat_id INNER JOIN transportation_list tl ON tl.transportation_id = sl.transportation_id WHERE mrl.member_code = @memberCode and  mrl.reservation_code = @travelReservationCode";
            SqlCommand select = new SqlCommand(fieldSql, connection);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            select.Parameters.AddWithValue("@travelReservationCode", travelReservationCode);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<MemberReservationListData> memberReservationListDatas = new List<MemberReservationListData>();
            while (reader.Read())
            {
                MemberReservationListData memberReservation = new MemberReservationListData();
                memberReservation.id = reader.IsDBNull(0) ? "" : (string)reader[0];
                memberReservation.name = reader.IsDBNull(1) ? "" : (string)reader[1];
                memberReservation.birthday = reader.IsDBNull(2) ? "" : (string)reader[2];
                memberReservation.phone = reader.IsDBNull(3) ? "" : (string)reader[3];
                memberReservation.mealsType = reader.IsDBNull(4) ? 0 : (int)reader[4];
                memberReservation.seatId = reader.IsDBNull(5) ? 0 : (int)reader[5];
                memberReservation.seatName = reader.IsDBNull(6) ? "" : (string)reader[6];
                memberReservation.transportationLicenesNumber = reader.IsDBNull(7) ? "" : (string)reader[7];
                memberReservationListDatas.Add(memberReservation);
            }
            return memberReservationListDatas;
        }

        // 取得會員旅遊訂單資訊
        public TravelReservationInfoData _getTravelReservationInfo(string memberCode, string travelReservationCode)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "select rl.reservation_code as travelReservationCode, tl.travel_code as travelCode, stl.travel_step_code as travelStepCode, rl.reservation_cost as cost, stl.travel_s_time as startDate, tl.travel_name as travelTraditionalTitle, tl.travel_content as travelTraditionalContent, rl.status as payStatus, rl.seat_ids as seatIds from reservation_list AS rl INNER JOIN travel_step_list stl ON stl.travel_step_id = rl.travel_step_id INNER JOIN travel_list tl ON tl.travel_id = stl.travel_id WHERE rl.reservation_code = @travelReservationCode and rl.member_code = @memberCode";
            SqlCommand select = new SqlCommand(fieldSql, connection);
            select.Parameters.AddWithValue("@travelReservationCode", travelReservationCode);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            TravelReservationInfoData travelReservationInfo = new TravelReservationInfoData();
            while (reader.Read())
            {
                travelReservationInfo.travelReservationCode = reader.IsDBNull(0) ? "" : (string)reader[0];
                travelReservationInfo.travelCode = reader.IsDBNull(1) ? "" : (string)reader[1];
                travelReservationInfo.travelStepCode = reader.IsDBNull(2) ? "" : (string)reader[2];
                travelReservationInfo.cost = reader.IsDBNull(3) ? 0 : (int)reader[3];
                string format = "yyyy-MM-dd HH:mm:ss";
                travelReservationInfo.startDate = ((DateTime)reader[4]).ToString(format);
                travelReservationInfo.travelTraditionalTitle = reader.IsDBNull(5) ? "" : (string)reader[5];
                travelReservationInfo.travelTraditionalContent = reader.IsDBNull(6) ? "" : (string)reader[6];
                travelReservationInfo.payStatus = reader.IsDBNull(7) ? 0 : (int)reader[7];
            }
            return travelReservationInfo;
        }


        // 新增旅遊預定會員資訊
        public void addReservationMemberInfo(MemberReservationArrBo memberReservationArrBo, int travelStepId, string memberCode, string orderCode)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("INSERT INTO member_reservation_list (id, name, phone, birthday, meals_type, seat_id, travel_step_id, member_code, reservation_code) " +
            "VALUES ('" + memberReservationArrBo.id + "', @reservationName, '" + memberReservationArrBo.phone + "', '" + memberReservationArrBo.birthday + "', " + memberReservationArrBo.mealsType + ", " + memberReservationArrBo.seatId + ", " + travelStepId + ", '" + memberCode + "', '" + orderCode + "')", connection);
            select.Parameters.Add("@reservationName", SqlDbType.NVarChar).Value = memberReservationArrBo.name;
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
        }

        // 新增旅遊預定資訊
        public void addReservation(string memberCode, string reservationCode, int reservationNum, int reservationCost, string seatIds, int travelStepId, string note, int travelId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("INSERT INTO reservation_list (reservation_code, reservation_num, reservation_cost, seat_ids, note, travel_id, travel_step_id, member_code) " +
            "VALUES ('" + reservationCode + "', " + reservationNum + ", " + reservationCost + ", '" + seatIds + "', '" + note + "', " + travelId + ", " + travelStepId + ", '" + memberCode + "')", connection);
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
        }

        //產生訂單編號
        public string createOrderNumber()
        {
            string n = DateTime.Now.ToString("yyyyMMddHHmmss");
            return "OR" + n;
        }

        // 更新旅遊訂位匯款狀態
        public async Task<ActionResult> updateReservationCheckStatus(string reservationId, int status)
        {

            int reservStatus = 0;

            IEnumerable<ReservationEntity> reservationEntity = await _reservationService.GetReservationAsync(Convert.ToInt16(reservationId));
            var reservation = reservationEntity.FirstOrDefault();
            if (status > -1)
            {
                bool flag = this._checkReservationCheckIsUpdate(reservation.reservationCode);
                if (!flag)
                {
                    return Json(new ApiResult<string>("Status Update", "匯款資訊狀態已更新"));
                }
                SqlConnection connection = new SqlConnection(this.sql_DB);
                // SQL Command
                SqlCommand select = new SqlCommand("UPDATE reservation_check_list SET status = @checkStatus, e_date = @eDate WHERE reservation_code = @reservationCode and status = 1", connection);
                select.Parameters.AddWithValue("@reservationCode", reservation.reservationCode);
                select.Parameters.Add("@checkStatus", SqlDbType.Int).Value = status;
                select.Parameters.Add("@eDate", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                ////開啟資料庫連線
                await connection.OpenAsync();
                select.ExecuteNonQuery();
                connection.Close();
            }

            //更新旅遊訂位狀態
            if (status == 1)
            {
                reservStatus = 1;
            }
            else if (status == -1)
            {
                reservStatus = -1;
            }
            else if (status == -2)
            {
                reservStatus = -2;
            }
            else
            {
                reservStatus = 0;
            }

            //更新旅遊訂位狀態
            this.updateReservationStatus(reservationId, reservStatus);


            return Json(new ApiResult<string>("Update Success", "匯款資訊狀態更新成功"));
        }

        // 更新旅遊訂位狀態
        public bool updateReservationStatus(string reservationId, int status)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE reservation_list SET status = @Status,  e_date = @eDate WHERE reservation_id = @reservationId", connection);
            select.Parameters.AddWithValue("@reservationId", reservationId);
            select.Parameters.Add("@status", SqlDbType.Int).Value = status;
            select.Parameters.Add("@eDate", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            ////開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return true;
        }

        // 取得旅遊訂位匯款狀態
        [HttpPost]
        public ActionResult getReservationCheckList(int page, int limit, string memberCode, string reservationCode)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select reservation_code as reservationCode, member_code as memberCode, bankbook_account_name as bankbookAccountName, bankbook_account_code as bankbookAccountCode, status as checkStatus, f_date as fDate, e_date as eDate from reservation_check_list WHERE reservation_code = @reservationCode and member_code = @memberCode", connection);
            if (memberCode != "" && !memberCode.Equals(""))
            {
                select.Parameters.AddWithValue("@memberCode", memberCode);
            }

            if (reservationCode != "" && !reservationCode.Equals(""))
            {
                select.Parameters.AddWithValue("@reservationCode", reservationCode);
            }

            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<ReservationCheckData> reservationCheckDatas = new List<ReservationCheckData>();
            PageControl<ReservationCheckData> pageControl = new PageControl<ReservationCheckData>();
            GetTravelReservationCheckListApi getTravelReservationCheckListApi = new GetTravelReservationCheckListApi();
            while (reader.Read())
            {
                ReservationCheckData reservationCheckData = new ReservationCheckData();
                reservationCheckData.reservationCode = reader.IsDBNull(0) ? "" : (string)reader[0];
                reservationCheckData.memberCode = reader.IsDBNull(1) ? "" : (string)reader[1];
                reservationCheckData.bankAccountName = reader.IsDBNull(2) ? "" : (string)reader[2];
                reservationCheckData.bankAccountCode = reader.IsDBNull(3) ? "" : (string)reader[3];
                reservationCheckData.checkStatus = (int)reader[4];
                string format = "yyyy-MM-dd HH:mm:ss";
                reservationCheckData.fDate = ((DateTime)reader[5]).ToString(format);
                reservationCheckData.eDate = ((DateTime)reader[6]).ToString(format);
                reservationCheckDatas.Add(reservationCheckData);
            }
            connection.Close();
            List<ReservationCheckData> reservationCheckDatasNew = pageControl.pageControl(page, limit, reservationCheckDatas);
            getTravelReservationCheckListApi.success = 1;
            getTravelReservationCheckListApi.count = pageControl.size;
            getTravelReservationCheckListApi.page = page;
            getTravelReservationCheckListApi.limit = limit;
            getTravelReservationCheckListApi.travelReservationCheckList = reservationCheckDatasNew;
            return Json(getTravelReservationCheckListApi);
        }


        [HttpPost]
        public async Task<ActionResult> cancelReservationBySystem(int seatId, int travelStepId)
        {
            try
            {
                await _reservationService.cancelReservationBySystem(seatId, travelStepId);
            }
            catch (Exception e)
            {
                throw e;
            }

            return Json(new ApiResult<string>("Cancel Success", "取消成功"));
        }

        // 旅遊訂位匯款是否更新
        public bool _checkReservationCheckIsUpdate(string reservationCode)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select reservation_code as reservationCode, member_code as memberCode, bankbook_account_name as bankbookAccountName, bankbook_account_code as bankbookAccountCode, status as checkStatus, f_date as fDate, e_date as eDate from reservation_check_list WHERE reservation_code = @reservationCode and status = @status ", connection);
            select.Parameters.AddWithValue("@reservationCode", reservationCode);
            select.Parameters.AddWithValue("@status", 1);

            Boolean flag = false;
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();

            while (reader.Read())
            {
                flag = reader.IsDBNull(0) ? false : true;
            }
            connection.Close();
            return flag;
        }

        public async Task<bool> _AddMemberReservation(List<MemberReservationData> memberReservatoionDatas)
        {
            List<KeyValuePair<string, ReservationData>> keyValuePairKeyArr = new List<KeyValuePair<string, ReservationData>>();
            string orderCode = "";
            int lastCount = memberReservatoionDatas.Count - 1;
            int memberReservationSeatIndex = 0;
            int reservationIndex = 0;
            string memberReservationSql = "INSERT INTO member_reservation_list (id, name, phone, birthday, meals_type, rooms_type, seat_id, boarding_id, transportation_id, travel_step_id, member_code, reservation_code, note) VALUES ";
            string seatTravelSql = "INSERT INTO seat_travel_match_list (seat_id, travel_step_id) VALUES ";
            string reservationSql = "INSERT INTO reservation_list (reservation_code, reservation_num, reservation_cost, seat_ids, note, travel_id, travel_step_id, member_code) VALUES ";
            foreach (MemberReservationData memberReservatoionData in memberReservatoionDatas)
            {
                string memberCode = memberReservatoionData.reservationMemberCode;
                string transportationCode = memberReservatoionData.transportationCode;
                int travelStepId = memberReservatoionData.travelStepId;
                int seatPos = memberReservatoionData.seatPos;
                TripStepTransportMatchModel tripStepTransportMatchModel = travelController.getTravelStepInfo(travelStepId.ToString());
                IEnumerable<TransportationEntity> transportationEntity = await _transportationService.GetTransportationByCode(transportationCode);
                int seatId = _seatService.GetSeatIdByTransportationIdAndPos(transportationEntity.First().transportationId, seatPos);
                int keyValueIndex = keyValuePairKeyArr.FindIndex(item => item.Key == (memberCode + travelStepId));
                if (keyValueIndex == -1)
                {
                    orderCode = this.createOrderNumber();
                    ReservationData reservationData = new ReservationData();
                    reservationData.reservationCode = orderCode;
                    reservationData.reservationCost = tripStepTransportMatchModel.travelCost;
                    reservationData.reservationNum = 1;
                    reservationData.travelStepId = travelStepId;
                    reservationData.travelId = tripStepTransportMatchModel.travelId;
                    reservationData.memberCode = memberCode;
                    reservationData.seatIds = seatId + ",";
                    KeyValuePair<string, ReservationData> keyValuePairKey = new KeyValuePair<string, ReservationData>(memberCode + travelStepId, reservationData);
                    keyValuePairKeyArr.Add(keyValuePairKey);
                }
                else
                {
                    ReservationData reservationData = keyValuePairKeyArr[keyValueIndex].Value;
                    orderCode = reservationData.reservationCode;
                    int num = reservationData.reservationNum + 1;
                    int cost = reservationData.reservationCost + reservationData.reservationCost;
                    string seatIdTotal = reservationData.seatIds;
                    reservationData.reservationCost = cost;
                    reservationData.reservationNum = num;
                    reservationData.seatIds = seatIdTotal + seatId + ",";
                    KeyValuePair<string, ReservationData> keyValuePairKey = new KeyValuePair<string, ReservationData>(memberCode + travelStepId, reservationData);
                    keyValuePairKeyArr[keyValueIndex] = keyValuePairKey;
                }
                string id = memberReservatoionData.memberIdCode;
                string name = memberReservatoionData.memberName;
                string memberPhone = memberReservatoionData.memberPhone;
                string birthday = memberReservatoionData.memberBirthday;
                int mealsType = memberReservatoionData.foodsType;
                int roomsType = memberReservatoionData.roomsType;
                int boardingId = memberReservatoionData.boardingId;
                int transportationId = transportationEntity.First().transportationId;
                string note = memberReservatoionData.memo;
                if (memberReservationSeatIndex == lastCount)
                {
                    memberReservationSql += "('" + id + "', N'" + name + "', '" + memberPhone + "', '" + birthday + "', " + mealsType + ", " + roomsType + ", " + seatId + ", " + boardingId + ", " + transportationId + ", " + travelStepId + ", '" + memberCode + "', '" + orderCode + "', N'" + note + "')";
                    seatTravelSql += "(" + seatId + "," + travelStepId + ")";
                }
                else
                {
                    memberReservationSql += "('" + id + "', N'" + name + "', '" + memberPhone + "', '" + birthday + "', " + mealsType + ", " + roomsType + ", " + seatId + ", " + boardingId + ", " + transportationId + ", " + travelStepId + ", '" + memberCode + "', '" + orderCode + "', N'" + note + "'),";
                    seatTravelSql += "(" + seatId + "," + travelStepId + "),";
                }
                memberReservationSeatIndex++;
            }

            foreach (KeyValuePair<string, ReservationData> keyValuePairKey in keyValuePairKeyArr)
            {
                ReservationData reservationData = keyValuePairKey.Value;
                string reservationCode = reservationData.reservationCode;
                int reservationNum = reservationData.reservationNum;
                int reservationCost = reservationData.reservationCost;
                string seatIds = reservationData.seatIds;
                string note = "";
                int travelId = reservationData.travelId;
                int travelStepId = reservationData.travelStepId;
                string memberCode = reservationData.memberCode;
                if (reservationIndex == (keyValuePairKeyArr.Count - 1))
                {
                    reservationSql += "('" + reservationCode + "', " + reservationNum + ", " + reservationCost + ", '" + seatIds + "', N'" + note + "', " + travelId + ", " + travelStepId + ", '" + memberCode + "')";
                }
                else
                {
                    reservationSql += "('" + reservationCode + "', " + reservationNum + ", " + reservationCost + ", '" + seatIds + "', N'" + note + "', " + travelId + ", " + travelStepId + ", '" + memberCode + "'),";
                }
                reservationIndex++;
            }

            // SQL Command
            SqlConnection connection = new SqlConnection(this.sql_DB);
            SqlCommand memberReservationSelect = new SqlCommand(memberReservationSql, connection);
            SqlCommand seatTravelSelect = new SqlCommand(seatTravelSql, connection);
            SqlCommand reservationSelect = new SqlCommand(reservationSql, connection);

            ////開啟資料庫連線
            connection.Open();

            // 開啟Transaction
            SqlTransaction trans = connection.BeginTransaction();
            memberReservationSelect.Transaction = trans;
            seatTravelSelect.Transaction = trans;
            reservationSelect.Transaction = trans;
            bool pass = false;
            try
            {
                memberReservationSelect.ExecuteNonQuery();
                seatTravelSelect.ExecuteNonQuery();
                reservationSelect.ExecuteNonQuery();
                trans.Commit();
                pass = true;
            } // try
            catch (Exception excep)
            {
                trans.Rollback();  // 出現例外就rollback
                pass = false;
            } // catch
            finally
            {
                connection.Close();
                memberReservationSelect.Dispose();
                seatTravelSelect.Dispose();
                reservationSelect.Dispose();
                connection.Dispose();
            } // finally
            return pass;
        }
    }
}
