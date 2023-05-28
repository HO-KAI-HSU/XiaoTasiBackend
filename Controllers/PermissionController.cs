using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using travelManagement.Models;
using static travelManagement.Controllers.LoginController;

namespace travelManagement.Controllers
{
    public class PermissionController : Controller
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetPermissionList()
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select page_permission_id, group_ids from page_permission_list", connection);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            List<PagePermissionData> pagePermissionDatas = new List<PagePermissionData>();
            while (reader.Read())
            {
                PagePermissionData pagePermissionData = new PagePermissionData();
                pagePermissionData.pagePermissionId = (int)reader[0];
                pagePermissionData.groupIds = reader.IsDBNull(1) ? "" : (string)reader[1];
                pagePermissionDatas.Add(pagePermissionData);
            }
            connection.Close();
            return Json(new ApiResult<List<PagePermissionData>>(pagePermissionDatas), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetPermissionInfo(int pagePermissionId)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select page_permission_id, group_ids from page_permission_list WHERE page_permission_id = @pagePermissionId", connection);
            select.Parameters.AddWithValue("@pagePermissionId", pagePermissionId);
            //// 開啟資料庫連線
            connection.Open();
            PagePermissionData pagePermissionData = new PagePermissionData();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                pagePermissionData.pagePermissionId = (int)reader[0];
                pagePermissionData.groupIds = reader.IsDBNull(1) ? "" : (string)reader[1];
            }
            connection.Close();
            return Json(new ApiResult<PagePermissionData>(pagePermissionData));
        }

        [HttpPost]
        public ActionResult UpdatePermissionInfo(int pagePermissionId, string groupIds)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand selectPermissionSql = new SqlCommand("select * from page_permission_list WHERE page_permission_id = @pagePermissionId", connection);
            selectPermissionSql.Parameters.AddWithValue("@pagePermissionId", pagePermissionId);
            connection.Open();
            SqlDataReader reader = selectPermissionSql.ExecuteReader();
            if (reader.HasRows)
            {
                SqlCommand updatePermissionSql = new SqlCommand("UPDATE page_permission_list SET group_ids = @groupIds WHERE page_permission_id = @pagePermissionId", connection);
                updatePermissionSql.Parameters.AddWithValue("@pagePermissionId", pagePermissionId);
                updatePermissionSql.Parameters.Add("@groupIds", SqlDbType.Text).Value = groupIds;
                updatePermissionSql.ExecuteNonQuery();
            }
            else
            {
                SqlCommand addPermissionSql = new SqlCommand("INSERT INTO page_permission_list (page_permission_id, group_ids)" +
                "VALUES (" + pagePermissionId + ", '" + groupIds + "')", connection);
                addPermissionSql.ExecuteNonQuery();
            }
            connection.Close();
            return Json(new ApiResult<string>("Update Success", "更新成功"));
        }


        //群組資訊物件
        public class PagePermissionData
        {
            public int pagePermissionId { get; set; }
            public string groupIds { get; set; }
            public static explicit operator PagePermissionData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }
    }
}
