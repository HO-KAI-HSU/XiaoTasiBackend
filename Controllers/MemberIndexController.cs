using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using System.Web.Mvc;
using travelManagement.Models;
using static travelManagement.Controllers.MemberController;

namespace travelManagement.Controllers
{
    public class MemberIndexController : Controller
    {
        LoginController loginController = new LoginController();
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetMemberInfo(string memberCode)
        {
            if ((memberCode == null || memberCode.Length == 0))
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必需参数缺失！"));
            }
            //bool res = loginController._authProcess("sessionkey" + memberCode);
            //if (!res)
            //{
            //    return RedirectToAction("Index", "Home", null);
            //}
            //SqlConnection connection = new SqlConnection("Server = localhost; User ID = sa; Password = reallyStrongPwd123; Database = tasiTravel");
            //// SQL Command
            //SqlCommand select = new SqlCommand("select ml.member_code, al.username, al.password, ml.name, ml.email, ml.address, al.phone, al.status from member_list AS ml JOIN account_list al ON ml.member_code = al.member_code WHERE ml.member_code = @memberCode", connection);
            //select.Parameters.AddWithValue("@memberCode", memberCode);
            //// 開啟資料庫連線
            //connection.Open();
            //MemberData memberData = new MemberData();
            MemberData member = this._getMemberInfo(memberCode);
            //SqlDataReader reder = select.ExecuteReader();
            if (member == null)
            {
                return Json(new ApiError(0006, "No Such Staff!", "無此人員資料！"));
            }
            //else
            //{
            //    memberData.memberCode = (string)reader[0];
            //    memberData.username = (string)reader[1];
            //    memberData.password = (string)reader[2];
            //    memberData.name = reader.IsDBNull(3) ? "" : reader[3].ToString();
            //    memberData.email = reader.IsDBNull(4) ? "" : reader[4].ToString();
            //    memberData.address = reader.IsDBNull(5) ? "" : reader[5].ToString();
            //    memberData.phone = reader.IsDBNull(6) ? "" : reader[6].ToString();
            //    memberData.status = (int)reader[7];
            //}
            //connection.Close();
            return Json(new ApiResult<object>(member));
        }

        [HttpPost]
        public ActionResult UpdateMemberInfo(string memberCode, string memberName, string email, string address)
        {
            if ((memberCode == null || memberCode.Length == 0))
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必需参数缺失！"));
            }
            bool res = loginController._authProcess("sessionkey" + memberCode);
            if (!res)
            {
                return RedirectToAction("Index", "Home", null);
            }
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE member_list SET name = @memberName, email = @email, address = @address WHERE member_Code = @memberCode", connection);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            select.Parameters.Add("@memberName", SqlDbType.NVarChar).Value = memberName;
            select.Parameters.Add("@email", SqlDbType.NVarChar).Value = email;
            select.Parameters.Add("@address", SqlDbType.NVarChar).Value = address;
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Update Success", "更新成功"));
        }


        public MemberData _getMemberInfo(string memberCode)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select ml.member_code, al.username, al.password, ml.name, ml.email, ml.address, al.phone, al.status from member_list AS ml JOIN account_list al ON ml.member_code = al.member_code WHERE ml.member_code = @memberCode", connection);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            // 開啟資料庫連線
            connection.Open();
            MemberData memberData = new MemberData();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                memberData.memberCode = (string)reader[0];
                memberData.username = (string)reader[1];
                memberData.password = (string)reader[2];
                memberData.name = reader.IsDBNull(3) ? "" : reader[3].ToString();
                memberData.email = reader.IsDBNull(4) ? "" : reader[4].ToString();
                memberData.address = reader.IsDBNull(5) ? "" : reader[5].ToString();
                memberData.phone = reader.IsDBNull(6) ? "" : reader[6].ToString();
                memberData.status = (int)reader[7];
            }
            connection.Close();
            return memberData;
        }

        ////費用說明資訊物件(VO)
        //public class GetMemberInfoResponse
        //{
        //    internal int count;

        //    public int success { get; set; }
        //    public List<UserInfo> userInfo { get; set; }
        //}
    }
}
