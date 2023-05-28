using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using XiaoTasiBackend.Models;
using XiaoTasiBackend.Models.Dto;
using XiaoTasiBackend.Service;
using XiaoTasiBackend.Service.Impl;

namespace XiaoTasiBackend.Controllers
{
    public class LoginController : Controller
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        ManagerService _managerService;

        public LoginController()
        {
            _managerService = new ManagerServiceImpl();
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginDto loginDto)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            if (string.IsNullOrWhiteSpace(loginDto.username) ||
                string.IsNullOrWhiteSpace(loginDto.password))
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必傳參數缺失！"));
            }

            var managerEntity = await _managerService.GetManagerByUsername(loginDto.username);
            if (managerEntity == null)
            {
                return Json(new ApiError(1002, "password error!", "此帳號密碼有誤，請重新輸入！"));
            }

            var password = managerEntity.password;
            if (!password.Equals(loginDto.password))
            {
                return Json(new ApiError(1002, "password error!", "此帳號密碼有誤，請重新輸入！"));
            }

            ManagerLoginDto managerLoginDto = new ManagerLoginDto
            {
                id = managerEntity.id,
                username = managerEntity.username,
                memberCode = managerEntity.memberCode,
                email = managerEntity.email,
                name = managerEntity.name,
                role = managerEntity.roleId,
                modify = managerEntity.isModify,
                status = managerEntity.status,
                token = managerEntity.token,
                phone = managerEntity.phone
            };

            Session.Add("sessionKey", managerLoginDto);
            string sessionId = Session.SessionID;
            HttpCookie httpCookie = new HttpCookie("sessionKey");
            httpCookie.Value = Server.UrlEncode(sessionId);
            httpCookie.Expires = DateTime.Now.AddHours(1);

            //寫到用戶端
            Response.Cookies.Add(httpCookie);
            connection.Close();
            return Json(new ApiResult<object>(managerLoginDto));
        }


        [HttpPost]
        public ActionResult Logout()
        {
            Session.Abandon();
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }
            return Json(new ApiResult<string>("Logout Success", "登出成功"));
        }


        [HttpPost]
        public ActionResult authProcess()
        {
            //if (Request.Cookies["ASP.NET_SessionId"] != null)
            //{
            //    return RedirectToAction("Index", "Member", null);
            //}
            //else
            //{
            //    return RedirectToAction("Index", "Home", null);
            //}
            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                return Json(new ApiResult<bool>(true));
            }
            else
            {
                return Json(new ApiResult<bool>(false));
            }
        }

        //登入資訊物件
        public class LoginData
        {
            public int id { get; set; }
            public string username { get; set; }
            public string memberCode { get; set; }
            public string email { get; set; }
            public string name { get; set; }
            public int role { get; set; }
            public bool modify { get; set; }
            public bool status { get; set; }
            public int groupId { get; set; }
            public string token { get; set; }
            public string phone { get; set; }

            public static explicit operator LoginData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }



        public bool _authProcess(string sessionKeyId)
        {

            if (Request.Cookies[sessionKeyId] != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
