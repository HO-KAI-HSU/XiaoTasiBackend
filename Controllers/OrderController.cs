using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using travelManagement.Models;

namespace travelManagement.Controllers
{
    public class OrderController : Controller
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult createTravelOrder(int orderNum, int payAmt)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            // 產生訂單唯一編碼
            string orderCode = this.createOrderNumber();
            // member_code 會員唯一編碼
            string memberCode = "mem0000001";
            string tradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//宣告一個目前的時間
            string sqlCommand = "INSERT INTO order_list (order_code, member_code, reservation_num, trade_amt, trade_date) VALUES ";
            sqlCommand = sqlCommand + " (" + "'" + orderCode + "', '" + memberCode + "', " + orderNum + ", " + payAmt + ",'" + tradeDate + "')";
            System.Diagnostics.Debug.Print(sqlCommand);
            SqlCommand select = new SqlCommand(sqlCommand, connection);

            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            EcPay ecpay = new EcPay();
            Dictionary<string, string> keyValuePairs = ecpay.sendEcPay(orderCode, "ALL");
            return Json(keyValuePairs);
        }

        [HttpPost]
        public string createTravelOrderTest(int orderNum, int payAmt)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            // 產生訂單唯一編碼
            string orderCode = this.createOrderNumber();
            // member_code 會員唯一編碼
            string memberCode = "mem0000001";
            string tradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//宣告一個目前的時間
            string sqlCommand = "INSERT INTO order_list (order_code, member_code, reservation_num, trade_amt, trade_date) VALUES ";
            sqlCommand = sqlCommand + " (" + "'" + orderCode + "', '" + memberCode + "', " + orderNum + ", " + payAmt + ",'" + tradeDate + "')";
            System.Diagnostics.Debug.Print(sqlCommand);
            SqlCommand select = new SqlCommand(sqlCommand, connection);

            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            EcPay ecpay = new EcPay();
            string keyValuePairs = ecpay.sendEcPayTest(orderCode, "ALL");
            return keyValuePairs;
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
        public ActionResult AddTransportTravel(string travelId, string transportationIds)
        {
            System.Diagnostics.Debug.Print(transportationIds);
            string[] transportationIdArr = transportationIds.Split(',');
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string sqlCommand = "INSERT INTO travel_transportation_match_list (travel_id, transportation_ids) VALUES ";
            sqlCommand = sqlCommand + " (" + Int32.Parse(travelId) + ", '" + transportationIds + "')";
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

        [HttpPost]
        public ActionResult getEcpayResponse(int transportTravelId)
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
            public static explicit operator TransportTravelData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }


        //產生訂單編號
        private string createOrderNumber()
        {
            string n = DateTime.Now.ToString("yyyyMMddHHmmss");
            return "OR" + n;
        }

        // 建立訂單
        public Dictionary<string, string> createOrder(string orderCode, int orderNum, int payAmt)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            // member_code 會員唯一編碼
            string memberCode = "mem0000001";
            string tradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");//宣告一個目前的時間
            string sqlCommand = "INSERT INTO order_list (order_code, member_code, reservation_num, trade_amt, trade_date) VALUES ";
            sqlCommand = sqlCommand + " (" + "'" + orderCode + "', '" + memberCode + "', " + orderNum + ", " + payAmt + ",'" + tradeDate + "')";
            System.Diagnostics.Debug.Print(sqlCommand);
            SqlCommand select = new SqlCommand(sqlCommand, connection);

            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            EcPay ecpay = new EcPay();
            Dictionary<string, string> keyValuePairs = ecpay.sendEcPay(orderCode, "ALL");
            return keyValuePairs;
        }
    }
}
