using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using travelManagement.Models;
using travelManagement.Models.Entity;
using static travelManagement.Controllers.BoardingController;
using static travelManagement.Controllers.LoginController;
using static travelManagement.Controllers.TransportationController;

namespace travelManagement.Controllers
{
    public class MemberTravelController : Controller
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetMemberTravelList()
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "travel_id as travelId, travel_name as travelName, travel_pic_path as travelPicPath, travel_url as travelUrl, travel_s_time as travelStime, travel_e_time as travelEtime";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from travel_list", connection);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<MemberTravelData> memberTravelDatas = new List<MemberTravelData>();
            while (reader.Read())
            {
                MemberTravelData memberTravelData = new MemberTravelData();
                memberTravelData.travelId = (int)reader[0];
                memberTravelData.travelName = (string)reader[1];
                memberTravelData.travelPicPath = (string)reader[2];
                memberTravelData.travelUrl = (string)reader[3];
                string format = "yyyy-MM-dd HH:mm:ss";
                memberTravelData.travelStime = ((DateTime)reader[4]).ToString(format);
                memberTravelData.travelEtime = ((DateTime)reader[5]).ToString(format);
                memberTravelDatas.Add(memberTravelData);
            }
            connection.Close();
            return Json(new ApiResult<List<MemberTravelData>>(memberTravelDatas), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetMemberTravelDetail(int travelId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "travel_id as travelId, travel_name as travelName, travel_content as travelContent, travel_s_time as travelStime, travel_e_time as travelEtime, travel_pic_path as travelPicPath, travel_url as travelUrl, travel_cost as travelCost, travel_num as travelNum";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from travel_list WHERE travelId_id = @travelId", connection);
            select.Parameters.AddWithValue("@travelId", travelId);
            // 開啟資料庫連線
            connection.Open();
            MemberTravelData memberTravelData = new MemberTravelData();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                memberTravelData.travelId = (int)reader[0];
                memberTravelData.travelName = (string)reader[1];
                memberTravelData.travelContent = (string)reader[2];
                string format = "yyyy-MM-dd HH:mm:ss";
                memberTravelData.travelStime = ((DateTime)reader[3]).ToString(format);
                memberTravelData.travelEtime = ((DateTime)reader[4]).ToString(format);
                memberTravelData.travelPicPath = (string)reader[5];
                memberTravelData.travelUrl = (string)reader[6];
                memberTravelData.travelCost = (int)reader[7];
                memberTravelData.travelNum = (int)reader[8];
            }
            connection.Close();
            return Json(new ApiResult<object>(memberTravelData));
        }


        //群組資訊物件
        public class MemberTravelData
        {
            public int travelId { get; set; }
            public string travelName { get; set; }
            public string travelContent { get; set; }
            public string travelStime { get; set; }
            public string travelEtime { get; set; }
            public string travelPicPath { get; set; }
            public string travelUrl { get; set; }
            public int travelCost { get; set; }
            public int travelNum { get; set; }
            public List<BoardingEntity> boardingList { get; set; }
            public List<TransportationData> transportationList { get; set; }
            public static explicit operator MemberTravelData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }
    }
}
