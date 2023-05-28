using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using XiaoTasiBackend.Models;
using static XiaoTasiBackend.Controllers.LoginController;

namespace XiaoTasiBackend.Controllers
{
    public class LogController : Controller
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetGroupList()
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            //SqlConnection connection = new SqlConnection("Server=tcp:xiaotasi.database.windows.net,1433;Database=tasiTravel;Uid=xiaotasi;Pwd=George128458162!!;Encrypt=yes;TrustServerCertificate=no;Connection Timeout=30;");
            // SQL Command
            SqlCommand select = new SqlCommand("select * from group_list", connection);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<GroupData> groupDatas = new List<GroupData>();
            while (reader.Read())
            {
                GroupData groupData = new GroupData();
                groupData.groupId = (int)reader[0];
                groupData.name = (string)reader[2];
                groupDatas.Add(groupData);
            }
            connection.Close();
            return Json(new ApiResult<List<GroupData>>(groupDatas), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetGroupInfo(int groupId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select * from group_list WHERE group_id = @groupId", connection);
            select.Parameters.AddWithValue("@groupId", groupId);
            // 開啟資料庫連線
            connection.Open();
            GroupData groupData = new GroupData();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                groupData.groupId = (int)reader[0];
                groupData.name = (string)reader[2];
            }
            connection.Close();
            return Json(new ApiResult<object>(groupData));
        }

        [HttpPost]
        public ActionResult AddGroup(string groupName)
        {

            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("INSERT INTO group_list (name) " +
            "VALUES ('" + groupName + "')", connection);
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Add Success", "新增成功"));
        }

        [HttpPost]
        public ActionResult UpdateGroupInfo(int groupId, string groupName)
        {

            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE group_list SET name = @groupName WHERE group_id = @groupId", connection);
            select.Parameters.AddWithValue("@groupId", groupId);
            select.Parameters.Add("@groupName", SqlDbType.Text).Value = groupName;
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Update Success", "更新成功"));
        }

        [HttpPost]
        public ActionResult DeleteGroup(int groupId)
        {

            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("DELETE group_list WHERE group_id = @groupId", connection);
            select.Parameters.AddWithValue("@groupId", groupId);
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Delete Success", "刪除成功"));
        }


        //群組資訊物件
        public class GroupData
        {
            public int groupId { get; set; }
            public string name { get; set; }
            public static explicit operator GroupData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }
    }
}
