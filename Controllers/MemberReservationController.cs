using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.Mvc;
using travelManagement.Models;

namespace travelManagement.Controllers
{
    public class MemberReservationController : Controller
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetMemberReservationList(string memberCode)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "reservation_code as reservationCode, reservation_num as reservationNum, reservation_cost as reservationCost, travel_id as travelId, boarding_id as boardingId, status, payment_datetime as payDatetime, f_date as fDate";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from reservation_list, travel where member_code = @memberCode", connection);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<MemberReservationData> memberReservationDatas = new List<MemberReservationData>();
            while (reader.Read())
            {
                MemberReservationData memberReservationData = new MemberReservationData();
                memberReservationData.reservationCode = (string)reader[0];
                memberReservationData.reservationNum = (int)reader[1];
                memberReservationData.reservationCost = (int)reader[2];
                memberReservationData.travelId = (int)reader[3];
                memberReservationData.boardingId = (int)reader[4];
                memberReservationData.status = (int)reader[5];
                string format = "yyyy-MM-dd HH:mm:ss";
                memberReservationData.payDatetime = ((DateTime)reader[6]).ToString(format);
                memberReservationData.fDate = ((DateTime)reader[7]).ToString(format);
                memberReservationDatas.Add(memberReservationData);
            }
            connection.Close();
            return Json(new ApiResult<List<MemberReservationData>>(memberReservationDatas), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetMemberReservationDetail(string memberCode, string reservationCode)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "reservation_code as reservationCode, reservation_num as reservationNum, reservation_cost as reservationCost, travel_id as travelId, boarding_id as boardingId, status, payment_datetime as payDatetime, f_date as fDate";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from reservation_list WHERE reservation_code = @reservationCode and member_code = @memberCode", connection);
            select.Parameters.AddWithValue("@reservationCode", reservationCode);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            // 開啟資料庫連線
            connection.Open();
            MemberReservationData memberReservationData = new MemberReservationData();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                memberReservationData.reservationCode = (string)reader[0];
                memberReservationData.reservationNum = (int)reader[1];
                memberReservationData.reservationCost = (int)reader[2];
                memberReservationData.travelId = (int)reader[3];
                memberReservationData.boardingId = (int)reader[4];
                memberReservationData.status = (int)reader[5];
                string format = "yyyy-MM-dd HH:mm:ss";
                memberReservationData.payDatetime = ((DateTime)reader[6]).ToString(format);
                memberReservationData.fDate = ((DateTime)reader[7]).ToString(format);
            }
            connection.Close();
            return Json(new ApiResult<object>(memberReservationData));
        }

        [HttpPost]
        public ActionResult AddMemberReservation(int reservationNum, int reservationCost, int travelId, int boardingId, int transportationId, string seatIds, string memberCode)
        {

            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("INSERT INTO reservation_list (reservation_num, reservation_cost, travel_id, boarding_id, transportation_id, seat_ids, member_code) " +
            "VALUES (" + reservationNum + ", " + reservationCost + ", " + travelId + ", " + boardingId + ", " + transportationId + ", '" + seatIds + "', '" + memberCode + "')", connection);
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Add Success", "新增成功"));
        }

        [HttpPost]
        public ActionResult UpdateMemberReservationInfo(int memberReservationId, string memberReservationName)
        {

            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE memberReservation_list SET name = @memberReservationName WHERE memberReservation_id = @memberReservationId", connection);
            select.Parameters.AddWithValue("@memberReservationId", memberReservationId);
            select.Parameters.Add("@memberReservationName", SqlDbType.Text).Value = memberReservationName;
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();

            return Json(new ApiResult<string>("Update Success", "更新成功"));
        }

        [HttpPost]
        public ActionResult DeleteMemberReservation(int memberReservationId)
        {

            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("DELETE memberReservation_list WHERE memberReservation_id = @memberReservationId", connection);
            select.Parameters.AddWithValue("@memberReservationId", memberReservationId);
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();

            return Json(new ApiResult<string>("Delete Success", "刪除成功"));
        }


        //預約資訊模組
        public class MemberReservationData
        {
            public int memberReservationId { get; set; }
            public string memberCode { get; set; }
            public string memberName { get; set; }
            public int travelId { get; set; }
            public string reservationCode { get; set; }
            public int reservationNum { get; set; }
            public int reservationCost { get; set; }
            public string transportationName { get; set; }
            public int boardingId { get; set; }
            public string boardingDatetime { get; set; }
            public string seatIds { get; set; }
            public int status { get; set; }
            public string payDatetime { get; set; }
            public string fDate { get; set; }
            public static explicit operator MemberReservationData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }
    }
}
