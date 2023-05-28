using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using XiaoTasiBackend.Helpers;
using XiaoTasiBackend.Models;

namespace XiaoTasiBackend.Controllers
{
    public class TravelController : Controller
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
        private readonly StorageHelper _storageHelper;
        private readonly UtilHelper _utilHelper;
        private readonly Random _random;

        public TravelController()
        {
            _storageHelper = new StorageHelper();
            _utilHelper = new UtilHelper();
            _random = new Random();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetLocationBoardingDateTimeList()
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select bl.boarding_id, ll.location_name, bl.boarding_datetime from location_list ll, boarding_list bl where ll.location_id = bl.location_id", connection);
            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<BoardingDatetimeData> boardingDatetimeDatas = new List<BoardingDatetimeData>();
            while (reader.Read())
            {
                BoardingDatetimeData boardingDatetimeData = new BoardingDatetimeData();
                boardingDatetimeData.boardingId = (int)reader[0];
                boardingDatetimeData.locationName = (string)reader[1];
                string format = "HH:mm";
                boardingDatetimeData.boardingDatetime = ((DateTime)reader[2]).ToString(format);
                boardingDatetimeDatas.Add(boardingDatetimeData);
            }
            connection.Close();
            return Json(new ApiResult<List<BoardingDatetimeData>>(boardingDatetimeDatas), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> GetTravelDetailList(string travelId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string travelSql = "SELECT travel_id as travelId, travel_code as travelCode, travel_name as travelTraditionalTitle, travel_en_name as travelEnTitle, travel_cost as costs, travel_type as travelType, travel_pic_path as travelPicPath, travel_url as travelUrl, travel_subject as travelSubject, travel_content as travelContent FROM travel_list WHERE travel_id = @travelId";

            SqlCommand select = new SqlCommand(travelSql, connection);
            //// 開啟資料庫連線
            select.Parameters.AddWithValue("@travelId", travelId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<TravelList> travelShowDatas = new List<TravelList>();
            //int count = 0;
            List<TravelStatisticList> travelStatisticList = null;
            List<DateTravelPicList> dateTravelPicList = null;
            TravelInfoData travelInfoData = null;
            TravelCostInfoTest travelCostInfo = null;
            Dictionary<string, object> travelInfo = new Dictionary<string, object>();
            while (reader.Read())
            {
                travelInfoData = new TravelInfoData();
                travelInfo.Add("travelCode", (string)reader[1]);
                travelInfo.Add("travelTitle", reader.IsDBNull(2) ? "" : (string)reader[2]);
                travelInfo.Add("travelSubject", reader.IsDBNull(8) ? "" : (string)reader[8]);
                travelInfo.Add("travelContent", reader.IsDBNull(9) ? "" : (string)reader[9]);
                travelInfo.Add("travelMoviePath", "");
                travelStatisticList = await this.GetTravelStatisticList((int)reader[0]);
                dateTravelPicList = await this.GetDateTravelPicList((int)reader[0]);
                travelCostInfo = await this.GetTravelCostInfo((int)reader[0]);
            }
            connection.Close();
            GetTravelInfoResponse getTravelInfoResponse = new GetTravelInfoResponse();
            getTravelInfoResponse.success = 1;
            getTravelInfoResponse.travelStatisticList = travelStatisticList;
            getTravelInfoResponse.travelInfo = travelInfo;
            getTravelInfoResponse.dateTravelPicList = dateTravelPicList;
            getTravelInfoResponse.costInfo = travelCostInfo;
            getTravelInfoResponse.announcementsList = new String[0];
            getTravelInfoResponse.nonIncludeCostList = new String[0];
            return Json(getTravelInfoResponse);
        }

        [HttpPost]
        public async Task<ActionResult> GetTravelList(int draw = 1, int start = 0, int length = 0)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select tl.travel_id, tl.travel_type, tl.travel_name, tl.travel_content, tl.travel_s_time, tl.travel_e_time, tl.travel_pic_path, tl.travel_cost, tl.travel_num, tl.travel_date_num from travel_list tl ", connection);
            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            PageControl<TravelData> pageControl = new PageControl<TravelData>();
            List<TravelData> travelDatas = new List<TravelData>();
            List<KeyValuePair<int, List<TravelStepData>>> travelStepListKeyVal = await this._getTravelStepList();
            List<KeyValuePair<int, ArrayList>> sTimeArrayL = await this._getTravelStepStimeArrL();
            List<KeyValuePair<int, List<TravelDetailData>>> travelDetailkeyVal = await this._getTravelDetailList();
            while (reader.Read())
            {
                string format = "yyyy-MM-dd HH:mm:ss";
                TravelData travelData = new TravelData();
                travelData.travelId = (int)reader[0];
                travelData.travelType = (int)reader[1];
                travelData.travelName = reader.IsDBNull(2) ? "" : (string)reader[2];
                travelData.travelContent = reader.IsDBNull(3) ? "" : (string)reader[3];
                travelData.travelStime = ((DateTime)reader[4]).ToString(format);
                travelData.travelEtime = ((DateTime)reader[5]).ToString(format);
                travelData.travelPicPath = reader.IsDBNull(6) ? "" : "~/Scripts/img/travel/" + (string)reader[6];
                List<TravelStepData> travelStepDatasGet;
                List<TravelDetailData> travelDetailDatasGet;
                ArrayList arrayListDatasGet;
                try
                {
                    // 取得索引
                    int keyValueIndexStepList = travelStepListKeyVal.FindIndex(item => item.Key == (int)reader[0]);
                    int keyValueIndexDetailList = travelDetailkeyVal.FindIndex(item => item.Key == (int)reader[0]);
                    int keyValueIndexArr = sTimeArrayL.FindIndex(item => item.Key == (int)reader[0]);
                    travelStepDatasGet = travelStepListKeyVal[keyValueIndexStepList].Value;
                    arrayListDatasGet = sTimeArrayL[keyValueIndexArr].Value;
                    travelDetailDatasGet = travelDetailkeyVal[keyValueIndexDetailList].Value;
                }
                catch (Exception e)
                {
                    arrayListDatasGet = new ArrayList();
                    travelStepDatasGet = new List<TravelStepData>();
                    travelDetailDatasGet = new List<TravelDetailData>();
                    Console.WriteLine(e);
                }
                string[] sTimeArr = (string[])arrayListDatasGet.ToArray(typeof(string));
                string sTimes = string.Join(",", sTimeArr);
                travelData.travelCost = (int)reader[7];
                travelData.travelNum = (int)reader[8];
                travelData.travelDateNum = (int)reader[9];
                travelData.travelDetailSdate = sTimes;
                travelData.travelStepList = travelStepDatasGet;
                travelData.travelDetailIdList = travelDetailDatasGet;
                travelDatas.Add(travelData);
            }
            connection.Close();

            List<TravelData> travelListNew = pageControl.pageControl((start + 1), length, travelDatas);

            // 整理回傳內容
            return Json(new ApiReturn<List<TravelData>>(1, draw, pageControl.size, pageControl.size, travelDatas));
        }

        [HttpGet]
        public async Task<ActionResult> GetTravelListForSelect()
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select tl.travel_id, tl.travel_name from travel_list tl ", connection);
            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<TravelData> travelDatas = new List<TravelData>();
            while (reader.Read())
            {
                TravelData travelData = new TravelData();
                travelData.travelId = (int)reader[0];
                travelData.travelName = (string)reader[1];
                travelDatas.Add(travelData);
            }
            connection.Close();
            return Json(new ApiResult<List<TravelData>>(travelDatas), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public async Task<ActionResult> GetTravelInfo(int travelId)
        {
            TravelData travelData = await this._getTravelInfo(travelId.ToString());
            return Json(new ApiResult<object>(travelData));
        }

        //取得旅遊明細編號
        public async Task<List<int>> _getTravelDetailId(int travelId, int day = 0)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            SqlCommand select;
            // SQL Command
            if (!day.Equals(0))
            {
                select = new SqlCommand("select travel_detail_id as travelDetailId from travel_detail_list WHERE travel_id = @travelId and day = @day", connection);
                select.Parameters.AddWithValue("@day", day);
            }
            else
            {
                select = new SqlCommand("select travel_detail_id as travelDetailId from travel_detail_list WHERE travel_id = @travelId order by travel_id, day ASC", connection);
            }
            select.Parameters.AddWithValue("@travelId", travelId);
            // 開啟資料庫連線
            await connection.OpenAsync();
            List<int> travelDetailIdList = new List<int>();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                travelDetailIdList.Add((int)reader[0]);
            }
            connection.Close();
            return travelDetailIdList;
        }

        //取得旅遊最大編號
        public async Task<int> _getTravelMaxId()
        {
            int idMax = 0;
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select max(travel_id) as travelIdMax from travel_list", connection);
            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                idMax = (int)reader[0];
            }
            connection.Close();
            return idMax;
        }

        //取得旅遊明細最大編號
        public async Task<int> _getTravelDetailMaxId()
        {
            int idMax = 0;
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select max(travel_detail_id) as travelDetailIdMax from travel_detail_list", connection);
            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                idMax = (int)reader[0];
            }
            connection.Close();
            return idMax;
        }

        //取得旅遊梯次最大編號
        public async Task<int> _getTravelStepMaxId()
        {
            int idMax = 0;
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select max(travel_step_id) as travelStepIdMax from travel_step_list", connection);
            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                idMax = (int)reader[0];
            }
            connection.Close();
            return idMax;
        }


        //取得旅遊梯次開始時間ArrayList
        public async Task<List<KeyValuePair<int, ArrayList>>> _getTravelStepStimeArrL()
        {
            // SQL Command
            SqlConnection connection = new SqlConnection(this.sql_DB);
            SqlCommand select = new SqlCommand("select travel_id, travel_s_time as travelStime from travel_step_list where status = 1", connection);

            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();

            // 旅遊編號陣列
            List<int> travelIdArr = new List<int>();
            List<KeyValuePair<int, ArrayList>> keyValuePairs = new List<KeyValuePair<int, ArrayList>>();
            while (reader.Read())
            {
                int travelId = (int)reader[0];
                if (!travelIdArr.Contains(travelId))
                {
                    travelIdArr.Add(travelId);
                    ArrayList travelStimeArrL = new ArrayList();
                    KeyValuePair<int, ArrayList> keyValuePair = new KeyValuePair<int, ArrayList>(travelId, travelStimeArrL);
                    keyValuePairs.Add(keyValuePair);
                }

                // 取得索引
                int keyValueIndex = keyValuePairs.FindIndex(item => item.Key == travelId);
                ArrayList arrayListGet = keyValuePairs[keyValueIndex].Value;

                if (reader[0] != null)
                {
                    string format = "yyyy-MM-dd";
                    string stime = ((DateTime)reader[1]).ToString(format);
                    arrayListGet.Add(stime);
                }
            }
            connection.Close();
            return keyValuePairs;
        }

        //取得旅遊梯次日期資訊
        public async Task<List<TravelStepData>> _getTravelStepList(int travelId)
        {
            // SQL Command
            SqlConnection connection = new SqlConnection(this.sql_DB);
            SqlCommand select = new SqlCommand("select tsl.travel_id as travelId, tsl.travel_step_id as travelStepId, tsl.travel_s_time as travelStime, tsl.travel_e_time as travelEtime, tsl.travel_step_code as travelStepCode from travel_step_list tsl left join seat_travel_match_list stml ON tsl.travel_step_id = stml.travel_step_id WHERE tsl.travel_id = @travelId and tsl.status = 1", connection);
            select.Parameters.AddWithValue("@travelId", travelId);

            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();

            // 旅遊編號陣列
            List<TravelStepData> travelStepDatas = new List<TravelStepData>();
            while (reader.Read())
            {
                TravelStepData travelStepData = new TravelStepData();
                if (reader[0] != null)
                {
                    string format = "yyyy-MM-dd";
                    int travelStepId = (int)reader[1];
                    string stime = ((DateTime)reader[2]).ToString(format);
                    string etime = ((DateTime)reader[3]).ToString(format);
                    string travelStepCode = (string)reader[4];
                    travelStepData.travelStepId = travelStepId;
                    travelStepData.travelStime = stime;
                    travelStepData.travelEtime = etime;
                    travelStepData.travelStepCode = travelStepCode;
                    travelStepDatas.Add(travelStepData);
                }
            }
            connection.Close();
            return travelStepDatas;
        }

        //取得旅遊梯次日期資訊
        public async Task<List<KeyValuePair<int, List<TravelStepData>>>> _getTravelStepList()
        {
            // SQL Command
            SqlConnection connection = new SqlConnection(this.sql_DB);
            SqlCommand select = new SqlCommand("select tsl.travel_id as travelId, tsl.travel_step_id as travelStepId, tsl.travel_s_time as travelStime, tsl.travel_e_time as travelEtime, tsl.travel_step_code as travelStepCode from travel_step_list tsl inner join travel_list tl ON tsl.travel_id = tl.travel_id and tsl.status = 1", connection);

            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();

            // 旅遊編號陣列
            List<int> travelIdArr = new List<int>();
            List<KeyValuePair<int, List<TravelStepData>>> keyValuePairs = new List<KeyValuePair<int, List<TravelStepData>>>();
            while (reader.Read())
            {
                int travelId = (int)reader[0];
                if (!travelIdArr.Contains(travelId))
                {
                    travelIdArr.Add(travelId);
                    List<TravelStepData> travelStepDatas = new List<TravelStepData>();
                    KeyValuePair<int, List<TravelStepData>> keyValuePair = new KeyValuePair<int, List<TravelStepData>>(travelId, travelStepDatas);
                    keyValuePairs.Add(keyValuePair);
                }

                // 取得索引
                int keyValueIndex = keyValuePairs.FindIndex(item => item.Key == travelId);
                List<TravelStepData> travelStepDatasGet = keyValuePairs[keyValueIndex].Value;

                TravelStepData travelStepData = new TravelStepData();
                if (reader[0] != null)
                {
                    string format = "yyyy-MM-dd";
                    int travelStepId = (int)reader[1];
                    string stime = ((DateTime)reader[2]).ToString(format);
                    string etime = ((DateTime)reader[3]).ToString(format);
                    string travelStepCode = reader.IsDBNull(4) ? "" : (string)reader[4];
                    travelStepData.travelStepId = travelStepId;
                    travelStepData.travelStime = stime;
                    travelStepData.travelEtime = etime;
                    travelStepData.travelStepCode = travelStepCode;
                    travelStepDatasGet.Add(travelStepData);
                }
            }
            connection.Close();
            return keyValuePairs;
        }

        //取得旅遊梯次日期資訊
        public async Task<List<KeyValuePair<int, List<TravelDetailData>>>> _getTravelDetailList(int travelId = 0)
        {
            // SQL Command
            SqlConnection connection = new SqlConnection(this.sql_DB);
            SqlCommand select;
            if (travelId.Equals(0))
            {
                select = new SqlCommand("select travel_id as travelId, travel_detail_id as travelDetailId from travel_detail_list order by travel_id, day ASC", connection);
            }
            else
            {
                select = new SqlCommand("select travel_id as travelId, travel_detail_id as travelDetailId from travel_detail_list where travel_id = @travelId order by travel_id, day ASC", connection);
                select.Parameters.AddWithValue("@travelId", travelId);
            }
            // 開啟資料庫連線
            await connection.OpenAsync();
            List<int> travelDetailIdList = new List<int>();
            SqlDataReader reader = select.ExecuteReader();

            // 旅遊編號陣列
            List<int> travelIdArr = new List<int>();
            List<KeyValuePair<int, List<TravelDetailData>>> keyValuePairs = new List<KeyValuePair<int, List<TravelDetailData>>>();
            while (reader.Read())
            {
                int travelIdInner = (int)reader[0];
                if (!travelIdArr.Contains(travelIdInner))
                {
                    travelIdArr.Add(travelId);
                    List<TravelDetailData> travelDetailDatas = new List<TravelDetailData>();
                    KeyValuePair<int, List<TravelDetailData>> keyValuePair = new KeyValuePair<int, List<TravelDetailData>>(travelIdInner, travelDetailDatas);
                    keyValuePairs.Add(keyValuePair);
                }

                // 取得索引
                int keyValueIndex = keyValuePairs.FindIndex(item => item.Key == travelIdInner);
                List<TravelDetailData> travelDetailDatasGet = keyValuePairs[keyValueIndex].Value;

                TravelDetailData travelDetailData = new TravelDetailData();
                if (reader[0] != null)
                {
                    int travelDetailId = (int)reader[1];
                    travelDetailData.travelDetailId = travelDetailId;
                    travelDetailDatasGet.Add(travelDetailData);
                }
            }
            connection.Close();
            return keyValuePairs;
        }

        [HttpPost]
        public async Task<ActionResult> AddTravel(string travelName, string travelContent, string travelStime, string travelEtime, int travelCost, int travelNum, int travelDateNum, string travelCustomBoardingIds, string travelDetailSdate, string transportationInfo, string eatInfo, string liveInfo, string actionInfo, string insuranceInfo, string nearInfo, int travelTypeId, string travelCustomBoardingFlag)
        {
            var pic = System.Web.HttpContext.Current.Request.Files["travelPicFile"];

            // 圖片處理區塊
            string url = "";
            if (pic != null)
            {
                HttpPostedFileBase filebase = new HttpPostedFileWrapper(pic);
                bool res = _storageHelper.IsImage(filebase);
                if (!res)
                {
                    return Json(new ApiError(1015, "Error file format!", "檔案類型不符！"));
                }
                string picName = this.createPicName();
                url = _storageHelper.UploadFile("xiaotasi", "travel", picName, filebase);
            }

            DateTime stime = DateTime.Parse(travelStime);
            DateTime etime = DateTime.Parse(travelEtime);
            string format = "yyyy-MM-dd HH:mm:ss";
            string travelCode = this.createTravelNumber();
            int stepCount = 0;
            if (travelDetailSdate == null || travelDetailSdate.Equals(""))
            {
                stepCount = 0;
            }
            else
            {
                string[] travelDetailSdateArr = travelDetailSdate.Split(',');
                stepCount = travelDetailSdateArr.Count();
            }

            // SQL Command
            SqlConnection connection = new SqlConnection(this.sql_DB);
            SqlCommand select = new SqlCommand("INSERT INTO travel_list (travel_name, travel_content, travel_s_time, travel_e_time, travel_cost, travel_num, travel_date_num, travel_pic_path, travel_code, travel_type, travel_step_count, travel_custom_boarding_flag, travel_viewpoint_info) " +
            " VALUES (@travelName, @travelContent,  @travelStime, @travelEtime, @travelCost, @travelNum, @travelDateNum, @travelPicPath, @travelCode, @travelType, @travelStepCount, @travelCustomBoardingFlag, @travelViewpointInfo) SET @ID = SCOPE_IDENTITY() ; ", connection);
            select.Parameters.Add("@travelName", SqlDbType.NVarChar).Value = travelName;
            select.Parameters.Add("@travelContent", SqlDbType.NVarChar).Value = travelContent;
            select.Parameters.Add("@travelStime", SqlDbType.VarChar).Value = stime.ToString(format);
            select.Parameters.Add("@travelEtime", SqlDbType.VarChar).Value = etime.ToString(format);
            select.Parameters.Add("@travelPicPath", SqlDbType.VarChar).Value = url;
            select.Parameters.Add("@travelCode", SqlDbType.VarChar).Value = travelCode;
            select.Parameters.Add("@travelCost", SqlDbType.Int).Value = travelCost;
            select.Parameters.Add("@travelNum", SqlDbType.Int).Value = travelNum;
            select.Parameters.Add("@travelDateNum", SqlDbType.Int).Value = travelDateNum;
            select.Parameters.Add("@travelType", SqlDbType.Int).Value = travelTypeId;
            select.Parameters.Add("@travelStepCount", SqlDbType.Int).Value = stepCount;
            select.Parameters.Add("@travelCustomBoardingFlag", SqlDbType.Int).Value = Convert.ToInt16(travelCustomBoardingFlag);
            select.Parameters.Add("@travelViewpointInfo", SqlDbType.NVarChar).Value = travelContent;
            await connection.OpenAsync();
            SqlParameter IDParameter = new SqlParameter("@ID", SqlDbType.Int);
            IDParameter.Direction = ParameterDirection.Output;
            select.Parameters.Add(IDParameter);
            select.ExecuteNonQuery();
            int id = (int)IDParameter.Value;

            //開啟資料庫連線
            this._addTravelStep(travelCode, id, travelDetailSdate, travelCost, travelNum, travelDateNum, connection);
            this._addTravelCost(id, transportationInfo, eatInfo, liveInfo, actionInfo, insuranceInfo, nearInfo, connection);
            this._addTravelDetail(id, travelDateNum, connection);
            if (travelCustomBoardingFlag.Equals("2"))
            {
                this._addTravelBoarding(id, travelCustomBoardingIds, connection);
            }
            connection.Close();
            return Json(new ApiResult<string>("Add Success", "新增成功"));
        }

        [HttpPost]
        public async Task<ActionResult> AddTravelDayDetail(int travelId, int day, string viewpointIds = "", string hotelIds = "", string breakfast = null, string lunch = null, string dinner = null)
        {
            List<int> travelDetailIdList = await this._getTravelDetailId(travelId, day);
            if (travelDetailIdList.Count == 0)
            {
                return Json(new ApiError(1015, "Add Fail!", " 新增失敗！"));
            }
            int travelDetailId = travelDetailIdList[0];

            SqlConnection connection = new SqlConnection(this.sql_DB);
            await connection.OpenAsync();

            this._delTravelMeal(travelId, travelDetailId, connection);
            this._delTravelViewpoint(travelId, travelDetailId, connection);
            this._delTravelHotel(travelId, travelDetailId, connection);

            // 判斷飯店是否為空
            if (!string.IsNullOrEmpty(hotelIds))
            {
                string[] hotelIdArr = hotelIds.Split(',');
                Array.Sort(hotelIdArr);
                foreach (string hotelId in hotelIdArr)
                {
                    this._addTravelHotel(travelId, Convert.ToInt32(hotelId), travelDetailId, connection);
                }
            }
            if (!string.IsNullOrEmpty(viewpointIds))
            {
                string[] viewpointIdArr = viewpointIds.Split(',');
                Array.Sort(viewpointIdArr);
                foreach (string viewpointId in viewpointIdArr)
                {
                    this._addTravelViewpoint(travelId, Convert.ToInt32(viewpointId), travelDetailId, connection);
                }
            }

            this._addTravelMeal(travelId, travelDetailId, breakfast, lunch, dinner, connection);

            return Json(new ApiResult<string>("Add Success", "新增成功"));
        }

        public async Task _AddTravel(List<TravelData> travelDatas)
        {
            int idMax = await _getTravelMaxId();
            int lastCount = travelDatas.Count - 1;
            int index = 0;
            string travelSql = "INSERT INTO travel_list (travel_id, travel_name, travel_content, travel_s_time, travel_e_time, travel_cost, travel_num, travel_date_num, travel_pic_path, travel_code, travel_type, travel_step_count) VALUES ";
            foreach (TravelData travelData in travelDatas)
            {
                idMax++;
                string travelName = travelData.travelName;
                string travelContent = travelData.travelContent;
                string travelStime = travelData.travelStime;
                string travelEtime = travelData.travelEtime;
                int travelCost = travelData.travelCost;
                int travelNum = travelData.travelNum;
                int travelDateNum = travelData.travelDateNum;
                int travelType = travelData.travelType;
                string travelDetailSdate = travelData.travelDetailSdate;

                int stepCount = 0;
                DateTime stime = DateTime.Parse(travelStime);
                DateTime etime = DateTime.Parse(travelEtime);
                string format = "yyyy-MM-dd HH:mm:ss";
                string travelCode = this.createTravelNumber();
                if (travelDetailSdate == null || travelDetailSdate.Equals(""))
                {
                    stepCount = 0;
                }
                else
                {
                    string[] travelDetailSdateArr = travelDetailSdate.Split(',');
                    stepCount = travelDetailSdateArr.Count();
                }
                if (index == lastCount)
                {
                    travelSql += "(" + idMax + ", N'" + travelName + "', N'" + travelContent + "', '" + stime.ToString(format) + "', '" + etime.ToString(format) + "', " + travelCost + ", " + travelNum + ", " + travelDateNum + ", " + "" + ", '" + travelCode + "', " + travelType + ", " + stepCount + ")";
                }
                else
                {
                    travelSql += "(" + idMax + ", N'" + travelName + "', N'" + travelContent + "', '" + stime.ToString(format) + "', '" + etime.ToString(format) + "', " + travelCost + ", " + travelNum + ", " + travelDateNum + ", " + "" + ", '" + travelCode + "', " + travelType + ", " + stepCount + "),";
                }
                index++;
            }

            // SQL Command
            SqlConnection connection = new SqlConnection(this.sql_DB);
            SqlCommand select = new SqlCommand(travelSql, connection);

            ////開啟資料庫連線
            await connection.OpenAsync();
            select.ExecuteNonQuery();
            connection.Close();
        }

        public async Task<bool> _batchUploadTravel(List<TravelData> travelDatas)
        {
            Console.WriteLine("--------_batchUploadTravel--------");
            int idMax = await _getTravelMaxId();
            int detailIdMax = await _getTravelDetailMaxId();
            int stepIdMax = await _getTravelStepMaxId();
            int lastCount = travelDatas.Count - 1;
            int index = 0;
            string travelSql = "SET IDENTITY_INSERT travel_list ON; INSERT INTO travel_list (travel_id, travel_name, travel_content, travel_s_time, travel_e_time, travel_cost, travel_num, travel_date_num, travel_pic_path, travel_code, travel_type, travel_step_count, travel_custom_boarding_flag, travel_viewpoint_info) VALUES ";
            string travelStepSql = "SET IDENTITY_INSERT travel_step_list ON; INSERT INTO travel_step_list (travel_step_id, travel_id, travel_num, remain_seat_num, travel_cost, travel_s_time, travel_e_time, travel_step_code) VALUES ";
            string travelCostSql = "INSERT INTO travel_cost_list (travel_id, transportation_info, eat_info, action_info, live_info, insurance_info, near_info) VALUES ";
            string travelDetailSql = "SET IDENTITY_INSERT travel_detail_list ON; INSERT INTO travel_detail_list (travel_detail_id, travel_id, day) VALUES ";
            string travelTransportationSql = "INSERT INTO travel_transportation_match_list (travel_id, transportation_ids, travel_step_id) VALUES ";
            string travelViewpointSql = "";
            string travelHotelSql = "";
            string travelMealSql = "INSERT INTO travel_meal_list (travel_id, travel_detail_id, breakfast, lunch, dinner)  VALUES ";
            string travelBoardingSql = "";
            bool travelViewpointFlag = false;
            bool travelHotelFlag = false;
            bool travelMealFlag = false;
            bool travelBoardingFlag = false;
            foreach (TravelData travelData in travelDatas)
            {
                Console.WriteLine("--------Count:--------" + index);
                idMax++;
                string travelName = travelData.travelName;
                string travelContent = travelData.travelContent;
                string travelStime = travelData.travelStime;
                string travelEtime = travelData.travelEtime;
                int travelCost = travelData.travelCost;
                int travelNum = travelData.travelNum;
                int travelDateNum = travelData.travelDateNum;
                int travelType = travelData.travelType;
                string travelDetailSdate = travelData.travelDetailSdate;
                string travelCustomBoardingFlag = travelData.travelCustomBoardingFlag.ToString();
                string travelBoardingIds = travelData.travelBoardingIds.ToString();
                string travelTransportationIds = travelData.travelTransportationIds.ToString();
                string travelViewpointInfo = travelData.travelViewpointInfo;

                // 花費說明
                string transportationInfo = travelData.travelCostInfo.transportationInfo;
                string eatInfo = travelData.travelCostInfo.eatInfo;
                string liveInfo = travelData.travelCostInfo.liveInfo;
                string actionInfo = travelData.travelCostInfo.actionInfo;
                string insuranceInfo = travelData.travelCostInfo.insuranceInfo;
                string[] travelDetailHotelIdsArrPop = null;
                string[] travelDetailViewpointIdsArrPop = null;

                // 景點，飯店，餐別說明
                if (!travelType.Equals(1))
                {
                    string travelDetailViewpointIds = travelData.travelDetailViewpointIds;
                    string travelDetailHotelIds = travelData.travelDetailHotelIds;
                    string[] travelDetailViewpointIdsArr = travelDetailViewpointIds.Split(':');
                    travelDetailViewpointIdsArrPop = _utilHelper.stringArrPop(travelDetailViewpointIdsArr);
                    string[] travelDetailHotelIdsArr = travelDetailHotelIds.Split(':');
                    travelDetailHotelIdsArrPop = _utilHelper.stringArrPop(travelDetailHotelIdsArr);
                }
                string travelDetailMealNames = travelData.travelDetailMealNames;
                string[] travelDetailMealNamesArr = travelDetailMealNames.Split(':');
                string[] travelTransportationIdsArr = travelTransportationIds.Split(':');
                string[] travelDetailMealNamesArrPop = _utilHelper.stringArrPop(travelDetailMealNamesArr);


                if (travelCustomBoardingFlag.Equals("2"))
                {
                    string[] travelBoardingIdArr = travelBoardingIds.Split(',');
                    string[] travelBoardingIdArrPop = _utilHelper.stringArrPop(travelBoardingIdArr);
                    int boardingIdCount = travelBoardingIdArrPop.Count();
                    int boardingIdIndex = 0;

                    if (string.IsNullOrEmpty(travelBoardingSql))
                    {
                        travelBoardingSql = "INSERT INTO travel_boarding_list (travel_id, boarding_id) VALUES ";
                    }

                    // 整理行程上車ＳＱＬ
                    Console.WriteLine("--------Arrange Travel Boarding SQL--------");
                    foreach (string travelBoardingId in travelBoardingIdArrPop)
                    {
                        if ((index == lastCount) && boardingIdIndex == (boardingIdCount - 1))
                        {
                            travelBoardingSql += "(" + idMax + ", " + travelBoardingId + "),";
                        }
                        else
                        {
                            travelBoardingSql += "(" + idMax + ", " + travelBoardingId + "),";
                        }
                        boardingIdIndex++;
                    }
                    travelBoardingFlag = true;
                }

                int stepCount = 0;
                DateTime stime = DateTime.Parse(travelStime);
                DateTime etime = DateTime.Parse(travelEtime);
                string format = "yyyy-MM-dd HH:mm:ss";
                string travelCode = this.createTravelNumber();
                if (travelDetailSdate == null || travelDetailSdate.Equals(""))
                {
                    stepCount = 0;
                }
                else
                {
                    // 取得行程梯次開始日期陣列
                    string[] travelDetailSdateArr = travelDetailSdate.Split(',');
                    string[] travelDetailSdateArrPop = _utilHelper.stringArrPop(travelDetailSdateArr);
                    stepCount = travelDetailSdateArrPop.Count();
                    int stepIndex = 0;
                    int stepCountIndex = 1;
                    Array.Sort(travelDetailSdateArrPop);

                    // 整理行程梯次ＳＱＬ
                    foreach (string travelDetailSdateVal in travelDetailSdateArrPop)
                    {
                        stepIdMax++;
                        if (!travelDetailSdateVal.Equals(""))
                        {
                            Console.WriteLine("--------Arrange Travel Step SQL--------");
                            DateTime sDate1 = DateTime.Parse(travelDetailSdateVal);
                            DateTime eDate1 = sDate1.AddDays(travelDateNum - 1);
                            string sFormat = "yyyy-MM-dd 00:00:00";
                            string eFormat = "yyyy-MM-dd 23:59:00";

                            // SQL Command
                            String stepNumNew = stepCountIndex.ToString().PadLeft(3, '0');
                            String stepCode = travelCode + stepNumNew;

                            // 行程梯次
                            if ((index == lastCount) && stepIndex == (stepCount - 1))
                            {
                                travelStepSql += "(" + stepIdMax + ", " + idMax + ", " + travelNum + ", " + travelNum + ", " + travelCost + ", '" + sDate1.ToString(sFormat) + "', '" + eDate1.ToString(eFormat) + "', '" + stepCode + "'); SET IDENTITY_INSERT travel_step_list OFF; ";
                                travelTransportationSql += "(" + idMax + ", '" + travelTransportationIdsArr[stepIndex] + "', " + stepIdMax + ");";
                            }
                            else
                            {
                                travelStepSql += "(" + stepIdMax + ", " + idMax + ", " + travelNum + ", " + travelNum + ", " + travelCost + ", '" + sDate1.ToString(sFormat) + "', '" + eDate1.ToString(eFormat) + "', '" + stepCode + "'),";
                                travelTransportationSql += "(" + idMax + ", '" + travelTransportationIdsArr[stepIndex] + "', " + stepIdMax + "),";
                            }

                            stepCountIndex++;
                            stepIndex++;
                        }
                    }
                }

                // 整理行程明細ＳＱＬ
                Console.WriteLine("--------Arrange Travel Detail SQL--------");
                int detailIndex = 0;
                for (int dayNum = 1; dayNum <= travelDateNum; dayNum++)
                {


                    detailIdMax++;
                    if ((index == lastCount) && detailIndex == (travelDateNum - 1))
                    {
                        travelDetailSql += "(" + detailIdMax + ", " + idMax + ", " + dayNum + "); SET IDENTITY_INSERT travel_detail_list OFF; ";
                    }
                    else
                    {
                        travelDetailSql += "(" + detailIdMax + ", " + idMax + ", " + dayNum + "),";
                    }

                    // 整理景點ＳＱＬ
                    Console.WriteLine("--------Arrange Travel Viewpoint SQL--------");
                    if (travelDetailViewpointIdsArrPop != null && travelDetailViewpointIdsArrPop.Count() > detailIndex)
                    {
                        string travelDetailDayViewpointIds = travelDetailViewpointIdsArrPop[detailIndex];
                        string[] travelDetailDayViewpointIdArr = travelDetailDayViewpointIds.Split(',');
                        string[] travelDetailDayViewpointIdArrPop = _utilHelper.stringArrPop(travelDetailDayViewpointIdArr);
                        int viewpointCount = travelDetailDayViewpointIdArrPop.Count();
                        int viewpointIndex = 0;

                        if (string.IsNullOrEmpty(travelViewpointSql))
                        {
                            travelViewpointSql = "INSERT INTO travel_pic_intro_list (travel_id, viewpoint_id, travel_detail_id) VALUES ";
                        }

                        foreach (string travelDetailDayViewpointId in travelDetailDayViewpointIdArrPop)
                        {
                            if (!travelDetailDayViewpointId.Equals(""))
                            {
                                if ((index == lastCount) && (detailIndex == (travelDateNum - 1)) && (viewpointIndex == (viewpointCount - 1)))
                                {
                                    travelViewpointSql += "(" + idMax + ", " + travelDetailDayViewpointId + ", " + detailIdMax + "),";
                                }
                                else
                                {
                                    travelViewpointSql += "(" + idMax + ", " + travelDetailDayViewpointId + ", " + detailIdMax + "),";
                                }
                            }
                            viewpointIndex++;
                        }
                        travelViewpointFlag = true;
                    }

                    // 整理飯店ＳＱＬ
                    Console.WriteLine("--------Arrange Travel Hotel SQL--------");
                    if (travelDetailHotelIdsArrPop != null && travelDetailHotelIdsArrPop.Count() > detailIndex)
                    {

                        if (string.IsNullOrEmpty(travelHotelSql))
                        {
                            travelHotelSql = "INSERT INTO travel_hotel_list (travel_id, hotel_id, travel_detail_id) VALUES ";
                        }

                        string travelDetailDayHotelId = travelDetailHotelIdsArrPop[detailIndex];
                        if (!travelDetailDayHotelId.Equals(""))
                        {
                            if ((index == lastCount) && detailIndex == (travelDateNum - 2))
                            {
                                travelHotelSql += "(" + idMax + ", " + travelDetailDayHotelId + ", " + detailIdMax + "),";
                            }
                            else
                            {
                                travelHotelSql += "(" + idMax + ", " + travelDetailDayHotelId + ", " + detailIdMax + "),";
                            }
                        }
                        travelHotelFlag = true;
                    }

                    // 整理餐別ＳＱＬ
                    Console.WriteLine("--------Arrange Travel Meal SQL--------");
                    if (travelDetailMealNamesArrPop != null && travelDetailMealNamesArrPop.Count() > detailIndex)
                    {
                        string travelDetailDayMealNames = travelDetailMealNamesArrPop[detailIndex];
                        string[] travelDetailDayMealNameArr = travelDetailDayMealNames.Split(',');
                        string[] travelDetailDayMealNameArrPop = _utilHelper.stringArrPop(travelDetailDayMealNameArr);
                        string dayBreakfast = !travelDetailDayMealNameArrPop[0].Equals("") ? travelDetailDayMealNameArrPop[0] : "";
                        string dayLunch = !travelDetailDayMealNameArrPop[1].Equals("") ? travelDetailDayMealNameArrPop[1] : "";
                        string dayDinner = !travelDetailDayMealNameArrPop[2].Equals("") ? travelDetailDayMealNameArrPop[2] : "";
                        if ((index == lastCount) && detailIndex == (travelDateNum - 1))
                        {
                            travelMealSql += "(" + idMax + ", " + detailIdMax + ", N'" + dayBreakfast + "', N'" + dayLunch + "',  N'" + dayDinner + "');";
                        }
                        else
                        {
                            travelMealSql += "(" + idMax + ", " + detailIdMax + ", N'" + dayBreakfast + "', N'" + dayLunch + "',  N'" + dayDinner + "'),";
                        }
                        travelMealFlag = true;
                    }
                    detailIndex++;
                }

                // 整理行程, 行程花費說明ＳＱＬ
                Console.WriteLine("--------Arrange Travel, TravelCost SQL--------");
                if (index == lastCount)
                {
                    travelSql += "(" + idMax + ", N'" + travelName + "', N'" + travelContent + "', '" + stime.ToString(format) + "', '" + etime.ToString(format) + "', " + travelCost + ", " + travelNum + ", " + travelDateNum + ", '" + " " + "', '" + travelCode + "', " + travelType + ", " + stepCount + ", " + travelCustomBoardingFlag + ", N'" + travelViewpointInfo + "'); SET IDENTITY_INSERT travel_list OFF; ";
                    travelCostSql += "(" + idMax + ",  N'" + transportationInfo + "', N'" + eatInfo + "', N'" + liveInfo + "', N'" + actionInfo + "', N'" + insuranceInfo + "', '" + "" + "');";
                }
                else
                {
                    travelSql += "(" + idMax + ", N'" + travelName + "', N'" + travelContent + "', '" + stime.ToString(format) + "', '" + etime.ToString(format) + "', " + travelCost + ", " + travelNum + ", " + travelDateNum + ", '" + "" + "', '" + travelCode + "', " + travelType + ", " + stepCount + ", " + travelCustomBoardingFlag + ", N'" + travelViewpointInfo + "'),";
                    travelCostSql += "(" + idMax + ",  N'" + transportationInfo + "', N'" + eatInfo + "', N'" + liveInfo + "', N'" + actionInfo + "', N'" + insuranceInfo + "', '" + "" + "'),";
                }
                index++;
            }

            Console.WriteLine(travelSql);
            Console.WriteLine(travelStepSql);
            Console.WriteLine(travelCostSql);
            Console.WriteLine(travelDetailSql);
            Console.WriteLine(travelTransportationSql);
            Console.WriteLine(travelViewpointSql);
            Console.WriteLine(travelHotelSql);
            Console.WriteLine(travelMealSql);
            Console.WriteLine(travelBoardingSql);

            // SQL Command
            SqlConnection connection = new SqlConnection(this.sql_DB);
            SqlCommand selectTravel = new SqlCommand(travelSql, connection);
            SqlCommand selectTravelStep = new SqlCommand(travelStepSql, connection);
            SqlCommand selectTravelCost = new SqlCommand(travelCostSql, connection);
            SqlCommand selectTravelDetail = new SqlCommand(travelDetailSql, connection);
            SqlCommand selectTravelTransportation = new SqlCommand(travelTransportationSql, connection);

            if (!string.IsNullOrEmpty(travelViewpointSql))
            {
                travelViewpointSql = travelViewpointSql.Substring(0, travelViewpointSql.Length - 1) + ";";
            }
            if (!string.IsNullOrEmpty(travelHotelSql))
            {
                travelHotelSql = travelHotelSql.Substring(0, travelHotelSql.Length - 1) + ";";
            }
            if (!string.IsNullOrEmpty(travelBoardingSql))
            {
                travelBoardingSql = travelBoardingSql.Substring(0, travelBoardingSql.Length - 1) + ";";
            }
            SqlCommand selectTravelViewpoint = new SqlCommand(travelViewpointSql, connection);
            SqlCommand selectTravelHotel = new SqlCommand(travelHotelSql, connection);
            SqlCommand selectTravelMeal = new SqlCommand(travelMealSql, connection);
            SqlCommand selectTravelBoaeding = new SqlCommand(travelBoardingSql, connection);
            bool pass = false;

            ////開啟資料庫連線
            await connection.OpenAsync();

            // 開啟Transaction
            SqlTransaction trans = connection.BeginTransaction();
            try
            {
                selectTravel.Transaction = trans;
                selectTravelStep.Transaction = trans;
                selectTravelCost.Transaction = trans;
                selectTravelDetail.Transaction = trans;
                selectTravelTransportation.Transaction = trans;

                selectTravel.ExecuteNonQuery();
                selectTravelStep.ExecuteNonQuery();
                selectTravelCost.ExecuteNonQuery();
                selectTravelDetail.ExecuteNonQuery();

                if (travelViewpointFlag)
                {
                    selectTravelViewpoint.Transaction = trans;
                    selectTravelViewpoint.ExecuteNonQuery();
                }

                if (travelHotelFlag)
                {
                    selectTravelHotel.Transaction = trans;
                    selectTravelHotel.ExecuteNonQuery();
                }

                if (travelBoardingFlag)
                {
                    selectTravelBoaeding.Transaction = trans;
                    selectTravelBoaeding.ExecuteNonQuery();
                }

                if (travelMealFlag)
                {
                    selectTravelMeal.Transaction = trans;
                    selectTravelMeal.ExecuteNonQuery();
                }


                selectTravelTransportation.ExecuteNonQuery();
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
                selectTravel.Dispose();
                selectTravelStep.Dispose();
                selectTravelCost.Dispose();
                selectTravelDetail.Dispose();
                selectTravelTransportation.Dispose();
                if (travelViewpointFlag)
                {
                    selectTravelViewpoint.Dispose();
                }

                if (travelHotelFlag)
                {
                    selectTravelHotel.Dispose();
                }

                if (travelBoardingFlag)
                {
                    selectTravelBoaeding.Dispose();
                }

                if (travelMealFlag)
                {
                    selectTravelMeal.Dispose();
                }
                connection.Dispose();
            } // finally
            return pass;
        }


        [HttpPost]
        public async Task<ActionResult> UpdateTravelInfo(int travelId, string travelName, string travelContent, int travelCost, int travelNum, string travelStime, string travelEtime, string travelDetailSdate, string transportationInfo, string eatInfo, string liveInfo, string actionInfo, string insuranceInfo, string nearInfo)
        {
            // 取得旅遊資訊
            TravelData travelData = await this._getTravelInfo(travelId.ToString());

            // 檢查選擇時間
            List<KeyValuePair<string, string>> keyValuePairs = await this._checkTravelStepStartDate(travelId, travelDetailSdate);
            string addStartDates = keyValuePairs[0].Value;
            string delStartDates = keyValuePairs[1].Value;
            int stepCount = travelData.travelStepCount;

            // SQL Connection
            SqlConnection connection = new SqlConnection(this.sql_DB);
            //開啟資料庫連線
            await connection.OpenAsync();

            if (addStartDates != "")
            {
                int addStepCountt = this._addTravelStep(travelData.travelCode, travelId, addStartDates, travelCost, travelData.travelNum, travelData.travelDateNum, connection, stepCount);
                stepCount = addStepCountt;
            }
            if (delStartDates != "")
            {
                this._delTravelStep(travelId, delStartDates, connection);
            }

            // 更新注意事項
            this._updateTravelCost(travelId, transportationInfo, eatInfo, liveInfo, actionInfo, insuranceInfo, nearInfo, connection);

            DateTime stime = DateTime.Parse(travelStime);
            DateTime etime = DateTime.Parse(travelEtime);
            string format = "yyyy-MM-dd HH:mm:ss";

            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE travel_list SET travel_name = @travelName, travel_content = @travelContent, travel_viewpoint_info = @travelViewpointInfo,  travel_s_time = '" + stime.ToString(format) + "', travel_e_time = '" + etime.ToString(format) + "', travel_cost = @travelCost, travel_num = @travelNum, travel_step_count = @travelStepCount  WHERE travel_id = @travelId", connection);
            select.Parameters.AddWithValue("@travelId", travelId);
            select.Parameters.Add("@travelName", SqlDbType.NVarChar).Value = travelName;
            select.Parameters.Add("@travelContent", SqlDbType.NVarChar).Value = travelContent;
            select.Parameters.Add("@travelViewpointInfo", SqlDbType.NVarChar).Value = travelContent;
            select.Parameters.Add("@travelCost", SqlDbType.Int).Value = travelCost;
            select.Parameters.Add("@travelNum", SqlDbType.Int).Value = travelNum;
            select.Parameters.Add("@travelStepCount", SqlDbType.Int).Value = stepCount;

            select.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Update Success", "更新成功"));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTravel(int travelId)
        {

            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("DELETE travel_list WHERE travel_id = @travelId", connection);
            select.Parameters.AddWithValue("@travelId", travelId);
            //開啟資料庫連線
            await connection.OpenAsync();
            select.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Delete Success", "刪除成功"));
        }

        [HttpPost]
        public async Task<ActionResult> GetTravelDetailInfo(int travelDetailId)
        {
            DateTravelPicList getTravelDetailInfo = await this._getTravelDetailInfo(travelDetailId);
            return Json(new ApiResult<DateTravelPicList>(getTravelDetailInfo));
        }

        //產生旅遊梯次
        private int _addTravelStep(string travelCode, int travelId, string travelDetailSdate, int travelCost, int travelNum, int travelDayNum, SqlConnection connection, int defaultStepCount = 0)
        {
            string[] travelDetailSdateArr = travelDetailSdate.Split(',');
            Array.Sort(travelDetailSdateArr);
            int stepCount = 1;
            if (defaultStepCount > 0)
            {
                stepCount = defaultStepCount + 1;
            }

            foreach (string travelDetailSdateVal in travelDetailSdateArr)
            {
                if (!travelDetailSdateVal.Equals(""))
                {
                    Console.WriteLine(travelDetailSdateVal);
                    DateTime sDate1 = DateTime.Parse(travelDetailSdateVal);
                    DateTime eDate1 = sDate1.AddDays(travelDayNum - 1);
                    string sFormat = "yyyy-MM-dd 00:00:00";
                    string eFormat = "yyyy-MM-dd 23:59:00";

                    // SQL Command
                    String stepNumNew = stepCount.ToString().PadLeft(3, '0');
                    String stepCode = travelCode + stepNumNew;
                    SqlCommand select = new SqlCommand("INSERT INTO travel_step_list (travel_id, travel_num, remain_seat_num, travel_cost, travel_s_time, travel_e_time, travel_step_code) " +
                    "VALUES (" + travelId + ", " + travelNum + ", " + travelNum + ", " + travelCost + ", '" + sDate1.ToString(sFormat) + "', '" + eDate1.ToString(eFormat) + "', '" + stepCode + "') ; SELECT SCOPE_IDENTITY() ; ", connection);
                    select.ExecuteNonQuery();
                    stepCount++;
                }
            }
            return stepCount;
        }

        //產生旅遊客製化上車
        private void _addTravelBoarding(int travelId, string boardingIds, SqlConnection connection)
        {
            string[] boardingIdArr = boardingIds.Split(',');

            foreach (string boardingId in boardingIdArr)
            {
                if (!boardingId.Equals(""))
                {
                    Console.WriteLine(boardingId);
                    SqlCommand select = new SqlCommand("INSERT INTO travel_boarding_list (travel_id, boarding_id) " +
                    "VALUES (" + travelId + ", " + boardingId + ")", connection);
                    select.ExecuteNonQuery();
                }
            }
        }

        //解除旅遊梯次
        private bool _delTravelBoarding(int travelId, SqlConnection connection)
        {
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE travel_boarding_list SET status = 0 where travel_id = @travelId and status = 1", connection);
            select.Parameters.AddWithValue("@travelId", travelId);
            select.ExecuteNonQuery();
            return true;
        }

        //解除旅遊梯次
        private bool _delTravelStep(int travelId, string travelDetailSdate, SqlConnection connection)
        {
            string[] travelDetailSdateArr = travelDetailSdate.Split(',');
            Array.Sort(travelDetailSdateArr);

            foreach (string travelDetailSdateVal in travelDetailSdateArr)
            {
                if (!travelDetailSdateVal.Equals(""))
                {
                    Console.WriteLine("del");
                    Console.WriteLine(travelDetailSdateVal);
                    DateTime sDate1 = DateTime.Parse(travelDetailSdateVal);
                    string sFormat = "yyyy-MM-dd 00:00:00";

                    // SQL Command
                    SqlCommand select = new SqlCommand("UPDATE travel_step_list SET status = 0 where travel_id = @travelId and status = 1 and travel_s_time = '" + sDate1.ToString(sFormat) + "'", connection);
                    select.Parameters.AddWithValue("@travelId", travelId);
                    select.ExecuteNonQuery();
                }
            }
            return true;
        }

        //新增旅遊花費注意事項
        private bool _addTravelCost(int travelId, string transportationInfo, string eatInfo, string liveInfo, string actionInfo, string insuranceInfo, string nearInfo, SqlConnection connection)
        {
            SqlCommand select = new SqlCommand("INSERT INTO travel_cost_list (travel_id, transportation_info, eat_info, live_info, action_info, insurance_info, near_info) " +
            "VALUES (" + travelId + ", @transportationInfo, @eatInfo, @liveInfo, @actionInfo , @insuranceInfo,  @nearInfo) ", connection);
            select.Parameters.Add("@transportationInfo", SqlDbType.NVarChar).Value = transportationInfo;
            select.Parameters.Add("@eatInfo", SqlDbType.NVarChar).Value = eatInfo;
            select.Parameters.Add("@liveInfo", SqlDbType.NVarChar).Value = liveInfo;
            select.Parameters.Add("@actionInfo", SqlDbType.NVarChar).Value = actionInfo;
            select.Parameters.Add("@insuranceInfo", SqlDbType.NVarChar).Value = insuranceInfo;
            select.Parameters.Add("@nearInfo", SqlDbType.NVarChar).Value = nearInfo;
            select.ExecuteNonQuery();
            return true;
        }

        //刪除旅遊飯店
        private bool _updateTravelCost(int travelId, string transportationInfo, string eatInfo, string liveInfo, string actionInfo, string insuranceInfo, string nearInfo, SqlConnection connection)
        {
            SqlCommand select = new SqlCommand("UPDATE travel_cost_list SET transportation_info = @transportationInfo, eat_info = @eatInfo, action_info = @actionInfo, live_info = @liveInfo, insurance_info = @insuranceInfo, near_info = @nearInfo WHERE travel_id = @travelId", connection);
            select.Parameters.AddWithValue("@travelId", travelId);
            select.Parameters.Add("@transportationInfo", SqlDbType.NVarChar).Value = transportationInfo;
            select.Parameters.Add("@eatInfo", SqlDbType.NVarChar).Value = eatInfo;
            select.Parameters.Add("@liveInfo", SqlDbType.NVarChar).Value = liveInfo;
            select.Parameters.Add("@actionInfo", SqlDbType.NVarChar).Value = actionInfo;
            select.Parameters.Add("@insuranceInfo", SqlDbType.NVarChar).Value = insuranceInfo;
            select.Parameters.Add("@nearInfo", SqlDbType.NVarChar).Value = nearInfo;
            select.ExecuteNonQuery();
            return true;
        }

        //新增旅遊供餐
        private bool _addTravelMeal(int travelId, int travelDetailId, string breakfast, string lunch, string dinner, SqlConnection connection)
        {
            SqlCommand select = new SqlCommand("INSERT INTO travel_meal_list (travel_id, travel_detail_id, breakfast, lunch, dinner) " +
            "VALUES (" + travelId + ", " + travelDetailId + ", @breakfast, @lunch,  @dinner) ", connection);
            select.Parameters.Add("@breakfast", SqlDbType.NVarChar).Value = breakfast;
            select.Parameters.Add("@lunch", SqlDbType.NVarChar).Value = lunch;
            select.Parameters.Add("@dinner", SqlDbType.NVarChar).Value = dinner;
            select.ExecuteNonQuery();
            return true;
        }

        //刪除旅遊飯店
        private bool _delTravelMeal(int travelId, int travelDetailId, SqlConnection connection)
        {
            SqlCommand select = new SqlCommand("DELETE travel_meal_list WHERE travel_id = @travelId and travel_detail_id = @travelDetailId", connection);
            select.Parameters.AddWithValue("@travelId", travelId);
            select.Parameters.AddWithValue("@travelDetailId", travelDetailId);
            select.ExecuteNonQuery();
            return true;
        }

        //新增旅遊飯店
        private bool _addTravelHotel(int travelId, int hotelId, int travelDetailId, SqlConnection connection)
        {
            SqlCommand select = new SqlCommand("INSERT INTO travel_hotel_list (travel_id, hotel_id, travel_detail_id) " +
            "VALUES (" + travelId + ", " + hotelId + ", " + travelDetailId + ") ", connection);
            select.ExecuteNonQuery();
            return true;
        }

        //刪除旅遊飯店
        private bool _delTravelHotel(int travelId, int travelDetailId, SqlConnection connection)
        {
            SqlCommand select = new SqlCommand("DELETE travel_hotel_list WHERE travel_id = @travelId and travel_detail_id = @travelDetailId", connection);
            select.Parameters.AddWithValue("@travelId", travelId);
            select.Parameters.AddWithValue("@travelDetailId", travelDetailId);
            select.ExecuteNonQuery();
            return true;
        }

        //山ㄔㄨ旅遊景點
        private bool _addTravelViewpoint(int travelId, int viewpointId, int travelDetailId, SqlConnection connection)
        {
            SqlCommand select = new SqlCommand("INSERT INTO travel_pic_intro_list (travel_id, viewpoint_id, travel_detail_id) " +
            "VALUES (" + travelId + ", " + viewpointId + ", " + travelDetailId + ") ", connection);
            select.ExecuteNonQuery();
            return true;
        }

        //刪除旅遊景點
        private bool _delTravelViewpoint(int travelId, int travelDetailId, SqlConnection connection)
        {
            SqlCommand select = new SqlCommand("DELETE travel_pic_intro_list WHERE travel_id = @travelId and travel_detail_id = @travelDetailId", connection);
            select.Parameters.AddWithValue("@travelId", travelId);
            select.Parameters.AddWithValue("@travelDetailId", travelDetailId);
            select.ExecuteNonQuery();
            return true;
        }

        //新增旅遊每天行程係項
        private bool _addTravelDetail(int travelId, int travelDayNum, SqlConnection connection)
        {
            for (int dayNum = 1; dayNum <= travelDayNum; dayNum++)
            {
                SqlCommand select = new SqlCommand("INSERT INTO travel_detail_list (travel_id, day) " +
                "VALUES (" + travelId + ", " + dayNum + ")", connection);
                select.ExecuteNonQuery();
            }
            return true;
        }

        //取得旅遊花費資訊
        private async Task<CostInfo> _getTravelCostInfo(string travelId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select transportation_info as transportationInfo, eat_info as eatInfo, live_info as liveInfo, action_info as actionInfo, insurance_info as insuranceInfo, near_info as nearInfo from travel_cost_list where travel_id = @travelId", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelId", travelId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            CostInfo costInfo = new CostInfo();
            while (reader.Read())
            {
                costInfo.transportationInfo = reader.IsDBNull(0) ? "" : (string)reader[0]; ;
                costInfo.eatInfo = reader.IsDBNull(1) ? "" : (string)reader[1];
                costInfo.liveInfo = reader.IsDBNull(2) ? "" : (string)reader[2];
                costInfo.actionInfo = reader.IsDBNull(3) ? "" : (string)reader[3];
                costInfo.insuranceInfo = reader.IsDBNull(4) ? "" : (string)reader[4];
                costInfo.nearInfo = reader.IsDBNull(5) ? "" : (string)reader[5];
            }
            connection.Close();
            return costInfo;
        }

        //產生旅遊編號i
        private string createTravelNumber()
        {
            string n = DateTime.Now.ToString("yyyyMMdd") + this.RandomNumber(100000, 999999);
            return "TR" + n;
        }

        // Generates a random number within a range.      
        private int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        // 產生圖片名稱
        private string createPicName()
        {
            string n = Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            return "travel" + n;
        }

        //上車時間資訊物件
        public class BoardingDatetimeData
        {
            public int boardingId { get; set; }
            public string locationName { get; set; }
            public string boardingDatetime { get; set; }
            public static explicit operator BoardingDatetimeData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //景點資訊物件
        public class ViewpointData
        {
            public int viewpointId { get; set; }
            public string viewpointTitle { get; set; }
            public string viewpointAddress { get; set; }
            public static explicit operator ViewpointData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //飯店資訊物件
        public class HotelData
        {
            public int hotelId { get; set; }
            public string hotelName { get; set; }
            public string hotelAddress { get; set; }
            public static explicit operator HotelData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //旅遊明細物件
        public class TravelDetailData
        {
            public int travelDetailId { get; set; }
            public static explicit operator TravelDetailData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }


        //旅遊梯次物件
        public class TravelStepData
        {
            public int travelStepId { get; set; }
            public string travelStime { get; set; }
            public string travelEtime { get; set; }
            public string travelStepCode { get; set; }
            public static explicit operator TravelStepData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        //旅遊資訊物件
        public class TravelData
        {
            public int travelId { get; set; }
            public string travelCode { get; set; }
            public int travelType { get; set; }
            public string travelName { get; set; }
            public string travelContent { get; set; }
            public string travelStime { get; set; }
            public string travelEtime { get; set; }
            public string travelPicPath { get; set; }
            public int travelDateNum { get; set; }
            public int travelCost { get; set; }
            public int travelNum { get; set; }
            public string travelDetailSdate { get; set; }
            public int travelStepCount { get; set; }
            public string travelCustomBoardingFlag { get; set; }
            public string travelBoardingIds { get; set; }
            public string travelTransportationIds { get; set; }
            public List<TravelStepData> travelStepList { get; set; }
            public List<TravelDetailData> travelDetailIdList { get; set; }
            public List<DateTravelPicList> travelDetailList { get; set; }
            public CostInfo travelCostInfo { get; set; }
            public string travelDetailViewpointIds { get; set; }
            public string travelDetailHotelIds { get; set; }
            public string travelDetailMealNames { get; set; }
            public string travelViewpointInfo { get; set; }
        }

        //旅遊資訊物件
        public class TravelSql
        {
            public int travel_id { get; set; }
            public string travel_name { get; set; }
            public string travel_content { get; set; }
            public string travel_s_time { get; set; }
            public string travel_e_time { get; set; }
            public string travel_pic_path { get; set; }
            public int travel_cost { get; set; }
            public int travel_num { get; set; }
            public int travel_date_num { get; set; }
            public string travel_code { get; set; }

        }

        //旅遊資訊列表顯示物件
        public class TravelList
        {
            public int travelId { get; set; }
            public string travelCode { get; set; }
            public string travelTraditionalTitle { get; set; }
            public string travelEnTitle { get; set; }
            public string startDate { get; set; }
            public int cost { get; set; }
            public int travelType { get; set; }
            public string travelPicPath { get; set; }
            public string travelUrl { get; set; }
        }

        //旅遊統計資訊顯示物件
        public class TravelStatisticList
        {
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string travelStep { get; set; }
            public int travelNum { get; set; }
            public int travelCost { get; set; }
            public int sellSeatNum { get; set; }
            public int remainSeatNum { get; set; }
            public string dest { get; set; }
            public int dayNum { get; set; }
            public int cost { get; set; }
        }

        //旅遊資訊顯示物件
        public class TravelStepInfo
        {
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string travelStep { get; set; }
            public int travelNum { get; set; }
            public int travelCost { get; set; }
            public int sellSeatNum { get; set; }
            public int remainSeatNum { get; set; }
            public int dayNum { get; set; }
        }

        //旅遊資訊顯示物件
        public class TravelStepInfoTest
        {
            public string startDate { get; set; }
            public string endDate { get; set; }
            public string travelStep { get; set; }
            public int travelNum { get; set; }
            public int travelCost { get; set; }
            public int sellSeatNum { get; set; }
            public int remainSeatNum { get; set; }
            public int dayNum { get; set; }
            public int travelDetailCode { get; set; }
        }

        //旅遊資訊顯示物件
        public class TravelInfo
        {
            public string travelId { get; set; }
            public string travelCode { get; set; }
            public string travelTitle { get; set; }
            public string travelSubject { get; set; }
            public string travelContent { get; set; }
        }

        //旅遊資訊顯示物件
        public class TravelInfoData
        {
            public string travelCode { get; set; }
            public string travelTitle { get; set; }
            public string travelSubject { get; set; }
            public string travelContent { get; set; }
        }

        //旅遊圖片介紹列表顯示物件
        public class TravelPicList
        {
            public string travelPicPath { get; set; }
            public string travelPicTraditionalTitle { get; set; }
            public string travelPicEnTitle { get; set; }
            public string travelPicTraditionalIntro { get; set; }
            public string travelPicEnIntro { get; set; }
        }

        //旅遊圖片介紹列表顯示物件
        public class TravelPicListTest
        {
            public int travelPicId { get; set; }
            public string travelDetailCode { get; set; }
            public string travelPicPath { get; set; }
            public string travelPicTraditionalTitle { get; set; }
            public string travelPicEnTitle { get; set; }
            public string travelPicTraditionalIntro { get; set; }
            public string travelPicEnIntro { get; set; }
        }

        //餐點資訊顯示物件
        public class TravelMealInfo
        {
            public string breakfast { get; set; }
            public string lunch { get; set; }
            public string dinner { get; set; }
        }

        //餐點資訊顯示物件
        public class TravelMealInfoTest
        {
            public string travelDetailCode { get; set; }
            public string breakfast { get; set; }
            public string lunch { get; set; }
            public string dinner { get; set; }
        }

        //住宿資訊顯示物件
        public class TravelHotelInfo
        {
            public string hotel { get; set; }
        }

        //住宿資訊顯示物件
        public class TravelHotelInfoTest
        {
            public int hotelId { get; set; }
            public string travelDetailCode { get; set; }
            public string hotel { get; set; }
        }

        //花費資訊顯示物件
        public class TravelCostInfoTest
        {
            public string travelDetailCode { get; set; }
            public string transportationInfo { get; set; }
            public string eatInfo { get; set; }
            public string liveInfo { get; set; }
            public string actionInfo { get; set; }
            public string insuranceInfo { get; set; }
            public string nearInfo { get; set; }
        }

        //每日旅遊圖片介紹列表顯示物件
        public class DateTravelPicList
        {
            public string date { get; set; }
            public List<TravelPicListTest> travelPicList { get; set; }
            public TravelMealInfoTest mealInfo { get; set; }
            public TravelHotelInfoTest hotelInfo { get; set; }
        }

        //費用說明資訊物件
        public class CostInfo
        {
            public string transportationInfo { get; set; }
            public string eatInfo { get; set; }
            public string liveInfo { get; set; }
            public string actionInfo { get; set; }
            public string insuranceInfo { get; set; }
            public string nearInfo { get; set; }
        }

        //費用說明資訊物件(VO)
        public class GetTravelInfoResponse
        {
            internal int count;
            public int success { get; set; }
            public List<TravelStatisticList> travelStatisticList { get; set; }
            public Dictionary<string, object> travelInfo { get; set; }
            public List<DateTravelPicList> dateTravelPicList { get; set; }
            public TravelCostInfoTest costInfo { get; set; }
            public String[] nonIncludeCostList { get; set; }
            public String[] announcementsList { get; set; }
        }

        //旅遊資訊顯示回傳物件(VO)
        public class GetTravelListResponse
        {
            public int success { get; set; }
            public int count { get; set; }
            public int page { get; set; }
            public int limit { get; set; }
            public List<TravelList> travelList { get; set; }
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

        //取得旅遊各梯次開始時間
        private async Task<string> _getTravelStepStartDateInfo(int travelId, bool flag = false)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select travel_s_time as startDate from travel_step_list where travel_id = @travelId and status = 1", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelId", travelId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            string startDates = null;
            while (reader.Read())
            {
                string format = "yyyy-MM-dd";
                if (!reader.IsDBNull(0))
                {
                    startDates += ((DateTime)reader[0]).ToString(format);
                    startDates += ",";
                }
            }
            return startDates;
        }

        // 更新旅遊梯次時間時，檢查時間是否需增加或刪除（List[0] : 增加的時間，List[1] : 刪除的時間）
        private async Task<List<KeyValuePair<string, string>>> _checkTravelStepStartDate(int travelId, string inputStartDates = "")
        {
            string delStartDates = "";
            string addStartDates = "";
            List<KeyValuePair<string, string>> keyValuePairs = new List<KeyValuePair<string, string>>();
            string startDatesFromDb = await _getTravelStepStartDateInfo(travelId);
            if ((startDatesFromDb == null || startDatesFromDb.Equals("")) && (inputStartDates == null || inputStartDates.Equals("")))
            {
                KeyValuePair<string, string> keyValuePairAdd = new KeyValuePair<string, string>("keyAddStartDates", addStartDates);
                KeyValuePair<string, string> keyValuePairDel = new KeyValuePair<string, string>("keyDelStartDates", delStartDates);
                keyValuePairs.Add(keyValuePairAdd);
                keyValuePairs.Add(keyValuePairDel);
                return keyValuePairs;
            }
            else if ((startDatesFromDb == null || startDatesFromDb.Equals("")))
            {
                addStartDates = inputStartDates;
                KeyValuePair<string, string> keyValuePairAdd = new KeyValuePair<string, string>("keyAddStartDates", addStartDates);
                KeyValuePair<string, string> keyValuePairDel = new KeyValuePair<string, string>("keyDelStartDates", delStartDates);
                keyValuePairs.Add(keyValuePairAdd);
                keyValuePairs.Add(keyValuePairDel);
                return keyValuePairs;
            }
            else if ((inputStartDates == null || inputStartDates.Equals("")))
            {
                delStartDates = startDatesFromDb;
                KeyValuePair<string, string> keyValuePairAdd = new KeyValuePair<string, string>("keyAddStartDates", addStartDates);
                KeyValuePair<string, string> keyValuePairDel = new KeyValuePair<string, string>("keyDelStartDates", delStartDates);
                keyValuePairs.Add(keyValuePairAdd);
                keyValuePairs.Add(keyValuePairDel);
                return keyValuePairs;
            }

            string[] inputStartDateArr = inputStartDates.Split(',');
            string[] dbStartDateArr = startDatesFromDb.Split(',');

            // 輸入開始日期與DB開始日期比對，db沒有input的資料，表示新選擇日期
            foreach (string inputStartDate in inputStartDateArr)
            {
                if (inputStartDate != "" && !dbStartDateArr.Contains(inputStartDate))
                {
                    addStartDates += inputStartDate;
                    addStartDates += ",";
                }
            }
            KeyValuePair<string, string> keyValuePairA = new KeyValuePair<string, string>("keyAddStartDates", addStartDates);
            keyValuePairs.Add(keyValuePairA);

            // DB開始日期與輸入開始日期比對，input沒有db的資料，表示需刪掉的日期
            foreach (string dbStartDate in dbStartDateArr)
            {
                if (dbStartDate != "" && !inputStartDateArr.Contains(dbStartDate))
                {
                    delStartDates += dbStartDate;
                    delStartDates += ",";
                }
            }
            KeyValuePair<string, string> keyValuePairD = new KeyValuePair<string, string>("keyDelStartDates", delStartDates);
            keyValuePairs.Add(keyValuePairD);
            return keyValuePairs;
        }

        //取得旅遊資訊
        private async Task<TravelData> _getTravelInfo(string travelId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select tl.travel_id, tl.travel_type, tl.travel_name, tl.travel_content, tl.travel_s_time, tl.travel_e_time, tl.travel_pic_path, tl.travel_cost, tl.travel_num, tl.travel_date_num, tl.travel_code, tl.travel_step_count from travel_list tl WHERE tl.travel_id = @travelId", connection);
            select.Parameters.AddWithValue("@travelId", travelId);
            // 開啟資料庫連線
            await connection.OpenAsync();
            TravelData travelData = new TravelData();
            SqlDataReader reader = select.ExecuteReader();
            List<KeyValuePair<int, List<TravelStepData>>> travelStepListKeyVal = await this._getTravelStepList();
            List<KeyValuePair<int, ArrayList>> sTimeArrayL = await this._getTravelStepStimeArrL();
            List<KeyValuePair<int, List<TravelDetailData>>> travelDetailkeyVal = await this._getTravelDetailList(Convert.ToInt16(travelId));
            while (reader.Read())
            {
                string format = "yyyy-MM-dd HH:mm:ss";
                travelData.travelId = (int)reader[0];
                travelData.travelName = reader.IsDBNull(2) ? "" : (string)reader[2];
                travelData.travelContent = reader.IsDBNull(3) ? "" : (string)reader[3];
                travelData.travelStime = ((DateTime)reader[4]).ToString(format);
                travelData.travelEtime = ((DateTime)reader[5]).ToString(format);
                travelData.travelCost = (int)reader[7];
                travelData.travelNum = (int)reader[8];
                travelData.travelDateNum = (int)reader[9];
                travelData.travelCode = reader.IsDBNull(10) ? "" : (string)reader[10];
                travelData.travelStepCount = reader.IsDBNull(11) ? 0 : (int)reader[11];
                List<TravelStepData> travelStepDatasGet;
                List<TravelDetailData> travelDetailDatasGet;
                ArrayList arrayListDatasGet;
                try
                {
                    // 取得索引
                    int keyValueIndexStepList = travelStepListKeyVal.FindIndex(item => item.Key == (int)reader[0]);
                    int keyValueIndexArr = sTimeArrayL.FindIndex(item => item.Key == (int)reader[0]);
                    int keyValueIndexDetailList = travelDetailkeyVal.FindIndex(item => item.Key == (int)reader[0]);
                    travelStepDatasGet = travelStepListKeyVal[keyValueIndexStepList].Value;
                    arrayListDatasGet = sTimeArrayL[keyValueIndexArr].Value;
                    travelDetailDatasGet = travelDetailkeyVal[keyValueIndexDetailList].Value;
                }
                catch (Exception e)
                {
                    arrayListDatasGet = new ArrayList();
                    travelStepDatasGet = new List<TravelStepData>();
                    travelDetailDatasGet = new List<TravelDetailData>();
                    Console.WriteLine(e);
                }

                CostInfo costInfo = await this._getTravelCostInfo(travelId);

                string[] sTimeArr = (string[])arrayListDatasGet.ToArray(typeof(string));
                string sTimes = string.Join(",", sTimeArr);
                travelData.travelDetailSdate = sTimes;
                travelData.travelStepList = travelStepDatasGet;
                travelData.travelCostInfo = costInfo;
                travelData.travelDetailIdList = travelDetailDatasGet;
            }
            connection.Close();
            return travelData;
        }

        //取得旅遊各梯次資訊
        private async Task<List<TravelStatisticList>> GetTravelStatisticList(int travelId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select travel_s_time as startDate, travel_e_time as endDate, travel_step_code as travelStep, travel_num as travelNum, travel_cost as travelCost, sell_seat_num as sellSeatNum, remain_seat_num as remainSeatNum from travel_step_list where travel_id = @travelId  and status = 1", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelId", travelId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<TravelStatisticList> travelStatisticLists = new List<TravelStatisticList>();
            while (reader.Read())
            {
                TravelStatisticList travelStepInfo = new TravelStatisticList();
                string format = "yyyy-MM-dd";
                DateTime startDate = ((DateTime)reader[0]);
                DateTime endDate = ((DateTime)reader[1]);
                travelStepInfo.startDate = startDate.ToString(format);
                travelStepInfo.endDate = endDate.ToString(format);
                travelStepInfo.travelStep = (string)reader[2];
                travelStepInfo.travelNum = (int)reader[3];
                travelStepInfo.travelCost = (int)reader[4];
                travelStepInfo.sellSeatNum = (int)reader[5];
                travelStepInfo.remainSeatNum = (int)reader[6];
                travelStepInfo.dest = "";
                travelStepInfo.dayNum = (int)endDate.Subtract(startDate).TotalDays + 1;
                travelStatisticLists.Add(travelStepInfo);
            }
            connection.Close();
            return travelStatisticLists;
        }

        //取得旅遊圖片介紹資訊
        private async Task<List<TravelPicListTest>> GetTravelPicIntroList(int travelDetailId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select tpil.viewpoint_id as travelPicId, vl.viewpoint_title as travelPicTraditionalTitle, vl.viewpoint_title as travelPicEnTitle, vl.viewpoint_content as travelPicTraditionalIntro, vl.viewpoint_content as travelPicEnIntro, vl.viewpoint_pic_path as travelPicPath, tpil.travel_detail_id as travelDetailId from travel_pic_intro_list tpil inner join viewpoint_list vl ON tpil.viewpoint_id = vl.viewpoint_id where tpil.travel_detail_id = @travelDetailId", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelDetailId", travelDetailId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<TravelPicListTest> travelPicListTests = new List<TravelPicListTest>();
            while (reader.Read())
            {
                TravelPicListTest travelPicListTest = new TravelPicListTest();
                travelPicListTest.travelPicId = reader.IsDBNull(0) ? 0 : (int)reader[0];
                travelPicListTest.travelPicTraditionalTitle = reader.IsDBNull(1) ? "" : (string)reader[1];
                travelPicListTest.travelPicEnTitle = reader.IsDBNull(2) ? "" : (string)reader[2];
                travelPicListTest.travelPicTraditionalIntro = reader.IsDBNull(3) ? "" : (string)reader[3];
                travelPicListTest.travelPicEnIntro = reader.IsDBNull(4) ? "" : (string)reader[4];
                travelPicListTest.travelPicPath = reader.IsDBNull(5) ? "" : (string)reader[5];
                travelPicListTest.travelDetailCode = "";
                travelPicListTests.Add(travelPicListTest);
            }
            connection.Close();
            return travelPicListTests;
        }

        //取得旅遊餐點資訊
        private async Task<TravelMealInfoTest> GetTravelMealInfo(int travelDetailId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select breakfast, lunch, dinner, travel_detail_id as travelDetailId from travel_meal_list where travel_detail_id = @travelDetailId", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelDetailId", travelDetailId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            TravelMealInfoTest travelMealInfoTest = new TravelMealInfoTest();
            while (reader.Read())
            {
                travelMealInfoTest.travelDetailCode = "";
                travelMealInfoTest.breakfast = reader.IsDBNull(0) ? "" : (string)reader[0];
                travelMealInfoTest.lunch = reader.IsDBNull(1) ? "" : (string)reader[1];
                travelMealInfoTest.dinner = reader.IsDBNull(2) ? "" : (string)reader[2];
            }
            connection.Close();
            return travelMealInfoTest;
        }

        //取得旅遊住宿資訊
        private async Task<TravelHotelInfoTest> GetTravelHotelInfo(int travelDetailId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select hl.hotel_id as hotelId, hl.hotel_name as hotel, thl.travel_detail_id as travelDetailId from travel_hotel_list thl inner join hotel_list hl ON thl.hotel_id = hl.hotel_id where thl.travel_detail_id = @travelDetailId", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelDetailId", travelDetailId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            TravelHotelInfoTest travelHotelInfoTest = new TravelHotelInfoTest();
            while (reader.Read())
            {
                travelHotelInfoTest.travelDetailCode = "";
                travelHotelInfoTest.hotelId = reader.IsDBNull(0) ? 0 : (int)reader[0];
                travelHotelInfoTest.hotel = reader.IsDBNull(1) ? "" : (string)reader[1];
            }
            connection.Close();
            return travelHotelInfoTest;
        }

        //取得旅遊花費資訊
        private async Task<TravelCostInfoTest> GetTravelCostInfo(int travelId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select transportation_info as transportationInfo, eat_info as eatInfo, live_info as liveInfo, action_info as actionInfo, insurance_info as insuranceInfo, near_info as nearInfo, travel_detail_id as travelDetailId from travel_cost_list where travel_id = @travelId", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelId", travelId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            TravelCostInfoTest travelCostInfoTest = new TravelCostInfoTest();
            while (reader.Read())
            {
                travelCostInfoTest.travelDetailCode = "";
                travelCostInfoTest.transportationInfo = reader.IsDBNull(0) ? "" : (string)reader[0]; ;
                travelCostInfoTest.eatInfo = reader.IsDBNull(1) ? "" : (string)reader[1];
                travelCostInfoTest.liveInfo = reader.IsDBNull(2) ? "" : (string)reader[2];
                travelCostInfoTest.actionInfo = reader.IsDBNull(3) ? "" : (string)reader[3];
                travelCostInfoTest.insuranceInfo = reader.IsDBNull(4) ? "" : (string)reader[4];
                travelCostInfoTest.nearInfo = reader.IsDBNull(5) ? "" : (string)reader[5];
            }
            connection.Close();
            return travelCostInfoTest;
        }

        //每日旅遊介紹列表
        private async Task<List<DateTravelPicList>> GetDateTravelPicList(int travelId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select travel_detail_id as travelDetailId, day, travel_id as travelId from travel_detail_list where travel_id = @travelId", connection);
            // 開啟資料庫連線
            select.Parameters.AddWithValue("@travelId", travelId);
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<DateTravelPicList> dateTravelPicLists = new List<DateTravelPicList>();
            while (reader.Read())
            {
                DateTravelPicList dateTravelPic = new DateTravelPicList();
                TravelHotelInfoTest travelHotelInfo = await this.GetTravelHotelInfo((int)reader[0]);
                TravelMealInfoTest travelMealInfo = await this.GetTravelMealInfo((int)reader[0]);
                List<TravelPicListTest> getTravelPicIntroList = await this.GetTravelPicIntroList((int)reader[0]);
                dateTravelPic.travelPicList = getTravelPicIntroList;
                dateTravelPic.hotelInfo = travelHotelInfo;
                dateTravelPic.mealInfo = travelMealInfo;
                dateTravelPic.date = "";
                dateTravelPicLists.Add(dateTravelPic);
            }
            connection.Close();
            return dateTravelPicLists;
        }

        //每日旅遊介紹列表
        private async Task<DateTravelPicList> _getTravelDetailInfo(int travelDetailId)
        {
            DateTravelPicList dateTravelPicLists = new DateTravelPicList();
            TravelHotelInfoTest travelHotelInfo = await this.GetTravelHotelInfo(travelDetailId);
            TravelMealInfoTest travelMealInfo = await this.GetTravelMealInfo(travelDetailId);
            List<TravelPicListTest> getTravelPicIntroList = await this.GetTravelPicIntroList(travelDetailId);
            dateTravelPicLists.travelPicList = getTravelPicIntroList;
            dateTravelPicLists.hotelInfo = travelHotelInfo;
            dateTravelPicLists.mealInfo = travelMealInfo;
            return dateTravelPicLists;
        }

        // 取得旅遊梯次乘車詳情
        public TripStepTransportMatchModel getTravelStepInfo(string travelStepId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "tsl.travel_step_id as travelStepId, tsl.travel_step_code as travelStepCode, tsl.travel_cost as travelCost, tsl.travel_num as travelNum, tsl.travel_s_time as travelStime, tsl.travel_e_time as travelEtime, tsl.travel_id as travelId, tsl.sell_seat_num as sellSeatNum, tsl.remain_seat_num as remainSeatNum, ttml.transportation_ids as transportationIds";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from travel_step_list as tsl inner join travel_transportation_match_list ttml ON  ttml.travel_step_id = tsl.travel_step_id WHERE tsl.travel_step_id = @travelStepId and tsl.status = 1", connection);
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
    }
}
