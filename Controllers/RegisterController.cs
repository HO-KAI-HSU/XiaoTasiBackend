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
using static travelManagement.Controllers.MemberController;

namespace travelManagement.Controllers
{
    public class RegisterController : Controller
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
        public ActionResult Index()
        {
            return View ();
        }

        [HttpPost]
        [Obsolete]
        public ActionResult Register(string username, string password, string name, int? phone, string email, string address)
        {
            if (username == "" || password == "" || name == null || phone == null || email == null || address == null)
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必需参数缺失！"));
            }
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand getMemberSelect = new SqlCommand("select member_code from member_list WHERE username = @username", connection);
            SqlCommand select = new SqlCommand("INSERT INTO member_list (username, password, email, name, address, phone)" +
            "VALUES ('" + username + "', '" + password + "',  '" + email + "', @name, @address, " + phone + ")", connection);
            select.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
            select.Parameters.Add("@address", SqlDbType.NVarChar).Value = address;
            // 開啟資料庫連線
            connection.Open();
            getMemberSelect.Parameters.AddWithValue("@username", username);
            SqlDataReader getMemberReader = getMemberSelect.ExecuteReader();
            while (getMemberReader.Read())
            {
                if (!getMemberReader.IsDBNull(0))
                {
                    return Json(new ApiError(0003, "Username used!", "此帳號已被使用，請重新輸入！"));
                }
            }
            connection.Close();
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            Smtp smtp = new Smtp();
            string msg = "請點擊此網址開通帳號，網址如下：" + "http://127.0.0.1:8080/Register/authBySmtp?email='" + email + "'";
            smtp.SendEmail(email, msg);
            DateTime gtm = new DateTime(1970, 1, 1);//宣告一個GTM時間出來
            DateTime utc = DateTime.UtcNow.AddHours(8);//宣告一個目前的時間
            int timeStamp = Convert.ToInt32(((TimeSpan)utc.Subtract(gtm)).TotalSeconds);
            Session.Add("sessionName", email);
            Session.Add("time", timeStamp);
            return Json(new ApiResult<string>("Register Success", "註冊成功，請等候管理驗證"));
        }

        [HttpGet]
        [Obsolete]
        public ActionResult AuthBySmtp(string email)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            SqlCommand getMemberSelect = new SqlCommand("select member_id, status from member_list WHERE email = @email", connection);
            MemberData member = new MemberData();
            getMemberSelect.Parameters.AddWithValue("@email", email);
            connection.Open();
            SqlDataReader getMemberReader = getMemberSelect.ExecuteReader();
            //int id = (int)getMemberReader.GetValue(0);
            while (getMemberReader.Read())
            {
                if (getMemberReader.IsDBNull(0))
                {
                    return Json(new ApiError(0007, "Username used!", "無此信箱，請重新輸入！"));
                    //return Json(new ApiError("0007", "無此信箱，請重新輸入！"), JsonRequestBehavior.AllowGet);
                }
                if ((int)getMemberReader[1] == 1)
                {
                    return Json(new ApiError(0008, "Username used!", "此帳號已開通！"));
                    //return Json(new ApiError("0008", "此帳號已開通！"), JsonRequestBehavior.AllowGet);
                }
                member.id = (int)getMemberReader[0];
            }
            connection.Close();
            SqlCommand select = new SqlCommand("UPDATE member_list SET status = " + 1 + " WHERE member_id = @memberId", connection);
            select.Parameters.AddWithValue("@memberId", member.id);
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("帳號開通成功"), JsonRequestBehavior.AllowGet);
            //return Json(new ApiResult<string>(email), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        // 傳送手機驗證碼
        public ActionResult getPhoneCaptcha(string cellphone)
        {
            if (cellphone == null || cellphone.Length == 0)
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必需参数缺失！"));
            }
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // 取得帳號資訊
            SqlCommand getMemberSelect = new SqlCommand("select * from account_list WHERE phone = @cellphone and status = @status", connection);
            getMemberSelect.Parameters.AddWithValue("@cellphone", cellphone);
            getMemberSelect.Parameters.AddWithValue("@status", 1);
            connection.Open();
            SqlDataReader getMemberReader = getMemberSelect.ExecuteReader();
            while (getMemberReader.Read())
            {
                if (!getMemberReader.IsDBNull(0))
                {
                    return Json(new ApiError(1014, "Phone used!", "電話已被註冊，請重新輸入！！"));
                }
            }
            connection.Close();
            int rand = this.getTimestamp();
            string captcha = rand.ToString().Substring(4, 6);
            Every8d every8 = new Every8d();
            string smsRes = every8.sendSMS("您的驗證碼為" + captcha, cellphone);
            Session.Add("phone", cellphone);
            Session.Add("captcha_time", rand);
            Session.Add("captcha", captcha);
            return Json(new ApiResult<string>(smsRes));
        }

        [HttpPost]
        // 傳送手機驗證碼
        public ActionResult getPhoneCaptchaTest1(string phone)
        {
            if (phone == null || phone.Length == 0)
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必需参数缺失！"));
            }
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // 取得帳號資訊
            SqlCommand getMemberSelect = new SqlCommand("select * from account_list WHERE phone = @phone and status = @status", connection);
            getMemberSelect.Parameters.AddWithValue("@phone", phone);
            getMemberSelect.Parameters.AddWithValue("@status", 1);
            connection.Open();
            SqlDataReader getMemberReader = getMemberSelect.ExecuteReader();
            while (getMemberReader.Read())
            {
                if (!getMemberReader.IsDBNull(0))
                {
                    return Json(new ApiError(1014, "Phone used!", "電話已被註冊，請重新輸入！"));
                }
            }
            connection.Close();
            int rand = this.getTimestamp();
            string captcha = rand.ToString().Substring(4, 6);
            Every8d every8 = new Every8d();
            string smsRes = every8.sendSMS("您的驗證碼為" + captcha, phone);
            Session.Add("phone", phone);
            Session.Add("captcha_time", rand);
            Session.Add("captcha", captcha);
            return Json(new ApiResult<string>(smsRes));
        }


        [HttpPost]
        // 傳送手機驗證碼
        public string getPhoneCaptchaTest(string phone)
        {
            HttpClientTest httpClientTest = new HttpClientTest();
            string smsRes = httpClientTest.httpClientPost("http://127.0.0.1:8080/Register/getPhoneCaptcha", phone);
            return smsRes;
        }

        [HttpPost]
        // 驗證手機驗證碼
        public ActionResult verifyPhoneCaptcha(string cellphone, string captcha)
        {
            if ((cellphone == null || cellphone.Length == 0) || (captcha == null || captcha.Length == 0))
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必需参数缺失！"));
            }
            if (Session["phone"] == null || !Session["phone"].Equals(cellphone))
            {
                return Json(new ApiError(1010, "Error phone!", "電話輸入錯誤或尚未取得驗證碼，請重新輸入或重新取得驗證碼！"));
            }
            if (Session["captcha"] == null || !Session["captcha"].Equals(captcha))
            {
                return Json(new ApiError(1011, "Error captcha!", "驗證碼輸入錯誤或尚未取得驗證碼，請重新輸入或重新取得驗證碼！"));
            }
            int timpStamp = this.getTimestamp();
            string phoneSess = Session["phone"].ToString();
            string captchaSess = Session["captcha"].ToString();
            int timpStampSess = (int)Session["captcha_time"];
            int diff = timpStamp - timpStampSess;
            if (diff > 55)
            {
                return Json(new ApiError(1012, "Captcha Expired!", "驗證碼已過期，請重新取得驗證碼！"));
            }
            this._updateAccountRegisterStatus(cellphone);
            return Json(new ApiResult<string>("Auth Success", "開通成功，請重新登入"));
        }

        [HttpPost]
        public ActionResult registerByPhone(string phone, string password, string username)
        {
            if ((phone == null || phone.Length == 0) || (username == null || username.Length == 0) || (password == null || password.Length == 0))
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必需参数缺失！"));
            }
            int timpStamp = this.getTimestamp();
            string memberCode = "mem" + timpStamp.ToString().Substring(2, 8);
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // 開啟資料庫連線

            // 取得帳號資訊
            SqlCommand getMemberSelect = new SqlCommand("select * from account_list WHERE username = @username", connection);
            getMemberSelect.Parameters.AddWithValue("@username", username);
            connection.Open();
            SqlDataReader getMemberReader = getMemberSelect.ExecuteReader();
            while (getMemberReader.Read())
            {
                if (!getMemberReader.IsDBNull(0))
                {
                    return Json(new ApiError(0003, "Username used!", "此身分證號已被使用，請重新輸入！"));
                }
            }
            connection.Close();

            // 新增會員資訊
            SqlCommand addMember = new SqlCommand("INSERT INTO member_list (member_code)" +
            "VALUES ('" + memberCode + "')", connection);
            connection.Open();
            addMember.ExecuteNonQuery();
            connection.Close();

            // 新增帳號資訊
            SqlCommand addAccount = new SqlCommand("INSERT INTO account_list (username, password, phone, member_code, status)" +
            "VALUES ('" + username + "', '" + password + "',  '" + phone + "',  '" + memberCode + "',  " + 1 + ")", connection);
            connection.Open();
            addAccount.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Register Success", "註冊成功，請重新登入"));
        }


        [HttpPost]
        public ActionResult registerByPhoneTest(string password, string username, string name, string email, string address, string birthday, string telephone, string cellphone, string emerContactName, string emerContactPhone)
        {
            if ((cellphone == null || cellphone.Length == 0) || (username == null || username.Length == 0) || (password == null || password.Length == 0) || (name == null || name.Length == 0) || (email == null || email.Length == 0) || (address == null || address.Length == 0))
            {
                return Json(new ApiError(1001, "Required field(s) is missing!", "必需参数缺失！"));
            }
            int timpStamp = this.getTimestamp();
            string memberCode = "mem" + timpStamp.ToString().Substring(2, 8);
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // 開啟資料庫連線

            // 取得帳號資訊
            SqlCommand getMemberSelect = new SqlCommand("select * from account_list WHERE username = @username", connection);
            getMemberSelect.Parameters.AddWithValue("@username", username);
            connection.Open();
            SqlDataReader getMemberReader = getMemberSelect.ExecuteReader();
            while (getMemberReader.Read())
            {
                if (!getMemberReader.IsDBNull(0))
                {
                    return Json(new ApiError(0003, "Username used!", "此身分證號已被使用，請重新輸入！"));
                }
            }
            connection.Close();

            // 新增會員資訊
            SqlCommand addMember = new SqlCommand("INSERT INTO member_list (member_code, email, name, address, birthday, telephone, emer_contact_name, emer_contact_phone )" +
            "VALUES ('" + memberCode + "', '" + email + "',  @name, @address, @birthday, @telephone, @emerContactName, @emerContactPhone)", connection);
            addMember.Parameters.Add("@birthday", SqlDbType.NVarChar).Value = birthday == null ? "" : birthday;
            addMember.Parameters.Add("@telephone", SqlDbType.NVarChar).Value = telephone == null ? "" : telephone;
            addMember.Parameters.Add("@emerContactName", SqlDbType.NVarChar).Value = emerContactName == null ? "" : emerContactName;
            addMember.Parameters.Add("@emerContactPhone", SqlDbType.NVarChar).Value = emerContactPhone == null ? "" : emerContactPhone;
            addMember.Parameters.Add("@name", SqlDbType.NVarChar).Value = name;
            addMember.Parameters.Add("@address", SqlDbType.NVarChar).Value = address == null ? "" : address;
            connection.Open();
            addMember.ExecuteNonQuery();
            connection.Close();

            // 新增帳號資訊
            SqlCommand addAccount = new SqlCommand("INSERT INTO account_list (username, password, phone, member_code, status)" +
            "VALUES ('" + username + "', '" + password + "',  '" + cellphone + "',  '" + memberCode + "',  " + 0 + ")", connection);
            connection.Open();
            addAccount.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Register Success, please auth by phone", "註冊成功，請完成手機驗證開通程序"));
        }

        //產生亂數（使用時間戳）
        private int getTimestamp()
        {
            DateTime gtm = new DateTime(1970, 1, 1);//宣告一個GTM時間出來
            DateTime utc = DateTime.UtcNow.AddHours(8);//宣告一個目前的時間
            int timeStamp = Convert.ToInt32(((TimeSpan)utc.Subtract(gtm)).TotalSeconds);
            return timeStamp;
        }

        //更新註冊狀態
        private bool _updateAccountRegisterStatus(string cellphone)
        {
            string updateDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");//宣告一個目前的時間
            SqlConnection connection = new SqlConnection(this.sql_DB);
            SqlCommand select = new SqlCommand("UPDATE account_list SET status = " + 1 + ", e_date = '" + updateDate + "'  WHERE phone = @cellphone", connection);
            select.Parameters.AddWithValue("@cellphone", cellphone);
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return true;
        }

        private void checkId(string user_id, string state) //檢查身分證字號
        {
            int[] uid = new int[10]; //數字陣列存放身分證字號用
            int chkTotal; //計算總和用

            if (user_id.Length == 10) //檢查長度
            {
                user_id = user_id.ToUpper(); //將身分證字號英文改為大寫

                //將輸入的值存入陣列中
                for (int i = 1; i < user_id.Length; i++)
                {
                    uid[i] = Convert.ToInt32(user_id.Substring(i, 1));
                }
                //將開頭字母轉換為對應的數值
                switch (user_id.Substring(0, 1).ToUpper())
                {
                    case "A": uid[0] = 10; break;
                    case "B": uid[0] = 11; break;
                    case "C": uid[0] = 12; break;
                    case "D": uid[0] = 13; break;
                    case "E": uid[0] = 14; break;
                    case "F": uid[0] = 15; break;
                    case "G": uid[0] = 16; break;
                    case "H": uid[0] = 17; break;
                    case "I": uid[0] = 34; break;
                    case "J": uid[0] = 18; break;
                    case "K": uid[0] = 19; break;
                    case "L": uid[0] = 20; break;
                    case "M": uid[0] = 21; break;
                    case "N": uid[0] = 22; break;
                    case "O": uid[0] = 35; break;
                    case "P": uid[0] = 23; break;
                    case "Q": uid[0] = 24; break;
                    case "R": uid[0] = 25; break;
                    case "S": uid[0] = 26; break;
                    case "T": uid[0] = 27; break;
                    case "U": uid[0] = 28; break;
                    case "V": uid[0] = 29; break;
                    case "W": uid[0] = 32; break;
                    case "X": uid[0] = 30; break;
                    case "Y": uid[0] = 31; break;
                    case "Z": uid[0] = 33; break;
                }
                //檢查第一個數值是否為1.2(判斷性別)
                if (uid[1] == 1 || uid[1] == 2)
                {
                    chkTotal = (uid[0] / 10 * 1) + (uid[0] % 10 * 9);

                    int k = 8;
                    for (int j = 1; j < 9; j++)
                    {
                        chkTotal += uid[j] * k;
                        k--;
                    }

                    chkTotal += uid[9];

                    if (chkTotal % 10 != 0)
                    {
                        Response.Write("身分證字號錯誤");
                    }
                }
                else
                {
                    Response.Write("身分證字號錯誤");
                }
            }
            else
            {
                Response.Write("身分證字號長度錯誤");
            }

        }



    }
}
