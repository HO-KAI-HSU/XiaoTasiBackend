using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using travelManagement.Models;

namespace travelManagement.Controllers
{
    public class TransportTravelController : Controller
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult GetTransportTravelList(int draw = 1, int start = 0, int length = 0)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select ttml.travel_transportation_match_id, travell.travel_name, ttml.transportation_ids, ttml.travel_step_id from travel_transportation_match_list ttml inner join travel_list travell ON ttml.travel_id = travell.travel_id where ttml.status = 1", connection);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            PageControl<TransportTravelData> pageControl = new PageControl<TransportTravelData>();
            List<TransportTravelData> transportTravelDatas = new List<TransportTravelData>();
            while (reader.Read())
            {
                TransportTravelData transportTravelData = new TransportTravelData();
                transportTravelData.travelTransportationMatchId = (int)reader[0];
                transportTravelData.travelName = (string)reader[1];
                transportTravelData.transportationIds = (string)reader[2];
                transportTravelData.travelStepId = (int)reader[3];
                transportTravelDatas.Add(transportTravelData);
            }
            connection.Close();

            List<TransportTravelData> transportTravelListNew = pageControl.pageControl((start + 1), length, transportTravelDatas);

            // 整理回傳內容
            return Json(new ApiReturn<List<TransportTravelData>>(1, draw, pageControl.size, pageControl.size, transportTravelDatas));
        }

        [HttpPost]
        public ActionResult GetTransportTravelInfo(int transportTravelId)
        {
            //SqlConnection connection = new SqlConnection("Server = localhost; User ID = sa; Password = reallyStrongPwd123; Database = tasiTravel");
            //// SQL Commands
            //SqlCommand select = new SqlCommand("select * from transportTravel_list WHERE transportTravel_id = @transportTravelId", connection);
            //select.Parameters.AddWithValue("@transportTravelId", transportTravelId);
            //// 開啟資料庫連線
            //connection.Open();
            //TransportTravelData transportTravelData = new TransportTravelData();
            //SqlDataReader reader = select.ExecuteReader();
            //while (reader.Read())
            //{
            //    transportTravelData.transportTravelId = (int)reader[0];
            //    string format = "yyyy-MM-dd HH:mm:ss";
            //    transportTravelData.transportTravelDatetime = ((DateTime)reader[3]).ToString(format);
            //}
            //connection.Close();
            return Json(new ApiResult<string>("aaa"));
        }

        [HttpPost]
        public ActionResult AddTransportTravel(string travelId, string travelStepId, string transportationIds)
        {
            System.Diagnostics.Debug.Print(transportationIds);
            string[] transportationIdArr = transportationIds.Split(',');
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string sqlCommand = "INSERT INTO travel_transportation_match_list (travel_id, transportation_ids, travel_step_id) VALUES ";
            sqlCommand = sqlCommand + " (" + Int32.Parse(travelId) + ", '" + transportationIds + "', " + Int32.Parse(travelStepId) + ")";
            //foreach (string key in transportationIdArr)
            //{
            //    if (Int32.Parse(key) <= transportationIdArr.Length)
            //    {
            //        if (Int32.Parse(key).Equals(transportationIdArr.Length))
            //        {
            //            sqlCommand = sqlCommand + " (" + Int32.Parse(travelId) + ", " + transportationIdArr[Int32.Parse(key) -1] + ")";

            //        }
            //        else
            //        {
            //            sqlCommand = sqlCommand + " (" + Int32.Parse(travelId) + ", " + transportationIdArr[Int32.Parse(key) - 1] + ") , ";
            //        }
            //    }
            //}
            System.Diagnostics.Debug.Print(sqlCommand);
            SqlCommand select = new SqlCommand(sqlCommand, connection);
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Add Success", "新增成功"));
        }

        [HttpPost]
        public ActionResult UpdateTransportTravelInfo(int transportTravelId, string transportationIds)
        {

            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE travel_transportation_match_list SET transportation_ids = '" + transportationIds + "' WHERE travel_transportation_match_id = @transportTravelId", connection);
            select.Parameters.AddWithValue("@transportTravelId", transportTravelId);
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Update Success", "更新成功"));
        }

        [HttpPost]
        public ActionResult DeleteTransportTravel(int transportTravelId)
        {

            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("DELETE travel_transportation_match_list WHERE travel_transportation_match_id = @transportTravelId", connection);
            select.Parameters.AddWithValue("@transportTravelId", transportTravelId);
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Delete Success", "刪除成功"));
        }


        //群組資訊物件
        public class TransportTravelData
        {
            public int travelTransportationMatchId { get; set; }
            public string travelName { get; set; }
            public string transportationIds { get; set; }
            public int travelStepId { get; set; }
            public static explicit operator TransportTravelData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }
    }
}
