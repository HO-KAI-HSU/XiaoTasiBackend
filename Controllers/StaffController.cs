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
    public class StaffController : Controller
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
        public ActionResult Index()
        {
            return View();
        }


        //    [HttpGet]
        //    public ActionResult GetMemberList()
        //    {
        //        SqlConnection connection = new SqlConnection(this.sql_DB);
        //        // SQL Command
        //        SqlCommand select = new SqlCommand("select manager_id, username, name, email, role, modify, status, group_id from manager_list", connection);
        //        // 開啟資料庫連線
        //        connection.Open();
        //        SqlDataReader reader = select.ExecuteReader();
        //        List<LoginData> loginDatas = new List<LoginData>();
        //        while (reader.Read())
        //        {
        //            LoginData loginData = new LoginData();
        //            loginData.id = (int)reader[0];
        //            loginData.username = reader.IsDBNull(1) ? "" : (string)reader[1];
        //            loginData.name = reader.IsDBNull(2) ? "" : (string)reader[2];
        //            loginData.email = reader.IsDBNull(3) ? "" : (string)reader[3];
        //            loginData.role = (int)reader[4];
        //            loginData.modify = (int)reader[5];
        //            loginData.status = (int)reader[6];
        //            loginData.groupId = (int)reader[7];
        //            loginDatas.Add(loginData);
        //        }
        //        connection.Close();
        //        return Json(new ApiResult<List<LoginData>>(loginDatas), JsonRequestBehavior.AllowGet);
        //    }

        //    [HttpPost]
        //    public ActionResult GetMemberInfo(int id)
        //    {
        //        SqlConnection connection = new SqlConnection(this.sql_DB);
        //        // SQL Command
        //        SqlCommand select = new SqlCommand("select manager_id, username, name, email, role, modify, status, group_id from manager_list WHERE manager_id = @id", connection);
        //        select.Parameters.AddWithValue("@id", id);
        //        // 開啟資料庫連線
        //        connection.Open();
        //        AccountData loginData = new AccountData();
        //        SqlDataReader reader = select.ExecuteReader();
        //        while (reader.Read())
        //        {
        //            loginData.id = (int)reader[0];
        //            loginData.username = reader.IsDBNull(1) ? "" : (string)reader[1];
        //            loginData.name = reader.IsDBNull(2) ? "" : (string)reader[2];
        //            loginData.email = reader.IsDBNull(3) ? "" : (string)reader[3];
        //            loginData.role = (int)reader[4];
        //            loginData.modify = (int)reader[5];
        //            loginData.status = (int)reader[6];
        //            loginData.groupId = (int)reader[7];
        //        }
        //        connection.Close();
        //        return Json(new ApiResult<object>(loginData));
        //    }

        //    [HttpPost]
        //    public ActionResult AddMember(string username, string password, string email, string name, int modify, int groupId)
        //    {

        //        SqlConnection connection = new SqlConnection(this.sql_DB);
        //        // SQL Command
        //        SqlCommand select = new SqlCommand("INSERT INTO manager_list (username, password, email, name, modify, group_id)" +
        //        "VALUES ('" + username + "', '" + password + "',  '" + email + "',  @name, " + modify + ", " + groupId + ")", connection);

        //        //select.Parameters.Add("@username", SqlDbType.Text).Value = username;
        //        //select.Parameters.Add("@password", SqlDbType.Text).Value = password;
        //        //select.Parameters.Add("@email", SqlDbType.Text).Value = email;
        //        select.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;

        //        //開啟資料庫連線
        //        connection.Open();
        //        select.ExecuteNonQuery();
        //        connection.Close();

        //        return Json(new ApiResult<string>("Add Success", "新增成功"));
        //    }

        //    [HttpPost]
        //    public ActionResult UpdateMemberInfo(int managerId, string password, string email)
        //    {

        //        SqlConnection connection = new SqlConnection(this.sql_DB);
        //        // SQL Command
        //        SqlCommand select = new SqlCommand("UPDATE manager_list SET password = @password, email = @email WHERE manager_id = @managerId", connection);
        //        select.Parameters.AddWithValue("@managerId", managerId);
        //        select.Parameters.Add("@password", SqlDbType.Text).Value = password;
        //        select.Parameters.Add("@email", SqlDbType.Text).Value = email;

        //        //開啟資料庫連線
        //        connection.Open();
        //        select.ExecuteNonQuery();
        //        connection.Close();

        //        return Json(new ApiResult<string>("Update Success", "更新成功"));
        //    }

        //    [HttpPost]
        //    public ActionResult DeleteMember(int managerId)
        //    {

        //        SqlConnection connection = new SqlConnection(this.sql_DB);
        //        // SQL Command
        //        SqlCommand select = new SqlCommand("DELETE manager_list WHERE manager_id = @managerId", connection);
        //        select.Parameters.AddWithValue("@managerId", managerId);
        //        //開啟資料庫連線
        //        connection.Open();
        //        select.ExecuteNonQuery();
        //        connection.Close();

        //        return Json(new ApiResult<string>("Delete Success", "刪除成功"));
        //    }


        //    //登入資訊物件
        //    public class AccountData
        //    {
        //        public int id { get; set; }
        //        public string username { get; set; }
        //        public string password { get; set; }
        //        public string email { get; set; }
        //        public string name { get; set; }
        //        public int role { get; set; }
        //        public int modify { get; set; }
        //        public int status { get; set; }
        //        public int groupId { get; set; }

        //        public static explicit operator AccountData(SqlDataReader v)
        //        {
        //            throw new NotImplementedException();
        //        }
        //    }
    }
}
