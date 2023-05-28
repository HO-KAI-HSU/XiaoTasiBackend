using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Web.Mvc;
using XiaoTasiBackend.Models;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Service;
using XiaoTasiBackend.Service.Impl;

namespace XiaoTasiBackend.Controllers
{
    public class MemberController : Controller
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        AccountService _accountService;

        public MemberController()
        {
            _accountService = new AccountServiceImpl();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetMemberList(int draw = 1, int start = 0, int length = 0)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select ml.member_code as memberCode, al.username, al.password, ml.name, ml.email, ml.address, al.phone, al.status from member_list AS ml JOIN account_list al ON ml.member_code = al.member_code", connection);
            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<MemberData> memberDatas = new List<MemberData>();
            PageControl<MemberData> pageControl = new PageControl<MemberData>();
            GetMemberListApi getMemberListApi = new GetMemberListApi();
            while (reader.Read())
            {
                MemberData memberData = new MemberData();
                memberData.memberCode = reader.IsDBNull(0) ? "" : (string)reader[0];
                memberData.username = reader.IsDBNull(1) ? "" : (string)reader[1];
                memberData.email = reader.IsDBNull(4) ? "" : (string)reader[4];
                memberData.address = reader.IsDBNull(5) ? "" : (string)reader[5];
                memberData.name = reader.IsDBNull(3) ? "" : (string)reader[3];
                memberData.phone = reader.IsDBNull(6) ? "" : (string)reader[6];
                memberData.status = (int)reader[7];
                memberDatas.Add(memberData);
            }
            connection.Close();
            List<MemberData> mmemberListNew = pageControl.pageControl((start + 1), length, memberDatas);

            // 整理回傳內容
            getMemberListApi.success = 1;
            getMemberListApi.draw = draw;
            getMemberListApi.recordsTotal = pageControl.size;
            getMemberListApi.recordsFiltered = pageControl.size;
            getMemberListApi.data = memberDatas;
            return Json(getMemberListApi);
        }

        [HttpPost]
        public async Task<ActionResult> GetMemberInfo(string memberCode)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select ml.member_code as memberCode, al.username, ml.name, ml.email, ml.address, ml.birthday, ml.telephone, al.phone as cellPhone, ml.emer_contact_name as emerContactName, ml.emer_contact_phone as emerContactPhone, al.status from member_list AS ml JOIN account_list al ON ml.member_code = al.member_code WHERE ml.member_code = @memberCode and al.status = 1", connection);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            // 開啟資料庫連線
            await connection.OpenAsync();
            MemberData memberData = new MemberData();
            GetMemberInfoApi getMemberInfoApi = new GetMemberInfoApi();
            SqlDataReader reader = select.ExecuteReader();
            if (!reader.Read())
            {
                return Json(new ApiError(0006, "No Such Staff!", "無此人員資料！"));
            }
            else
            {
                memberData.memberCode = reader.IsDBNull(0) ? "" : (string)reader[0];
                memberData.username = reader.IsDBNull(1) ? "" : (string)reader[1];
                memberData.name = reader.IsDBNull(2) ? "" : (string)reader[2];
                memberData.email = reader.IsDBNull(3) ? "" : (string)reader[3];
                memberData.address = reader.IsDBNull(4) ? "" : (string)reader[4];
                memberData.birthday = reader.IsDBNull(5) ? "" : ((DateTime)reader[5]).ToString("yyyy-mm-dd");
                memberData.telephone = reader.IsDBNull(6) ? "" : reader[6].ToString();
                memberData.phone = reader.IsDBNull(7) ? "" : reader[7].ToString();
                memberData.emerContactName = reader.IsDBNull(8) ? "" : reader[8].ToString();
                memberData.emerContactPhone = reader.IsDBNull(9) ? "" : reader[9].ToString();
                memberData.status = (int)reader[10];
            }
            connection.Close();

            // 整理回傳內容
            getMemberInfoApi.success = 1;
            getMemberInfoApi.memberInfo = memberData;
            return Json(getMemberInfoApi);
        }


        [HttpPost]
        public async Task<ActionResult> UpdateMemberInfo(string memberCode, string memberName, string memberEmail, string memberTelephone, string memberPhone, string memberBirthday, string memberAddress, string memberEmerContactName, string memberEmerContactPhone)
        {

            bool res = await CheckMemberPhoneIsUsed(memberCode, memberPhone);
            if (res)
            {
                return Json(new ApiError(1014, "Phone used!", "電話已被註冊，請重新輸入！！"));
            }

            res = await CheckMemberIsExisted(memberCode);
            if (!res)
            {
                return Json(new ApiError(0006, "No Such Staff!", "無此人員資料！"));
            }

            AccountEntity accountEntity = new AccountEntity()
            {
                memberCode = memberCode,
                phone = memberPhone,
            };
            await _accountService.UpdateAccountPhoneAsync(accountEntity);

            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE member_list SET name = @name, email = @email, telephone = @telephone, address = @address, birthday = @birthday, emer_contact_name = @emerContactName, emer_contact_phone = @emerContactPhone WHERE member_code = @memberCode", connection);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            select.Parameters.Add("@name", SqlDbType.NVarChar).Value = memberName;
            select.Parameters.Add("@email", SqlDbType.NVarChar).Value = memberEmail;
            select.Parameters.Add("@telephone", SqlDbType.NVarChar).Value = memberTelephone;
            select.Parameters.Add("@birthday", SqlDbType.NVarChar).Value = memberBirthday;
            select.Parameters.Add("@address", SqlDbType.NVarChar).Value = memberAddress;
            select.Parameters.Add("@emerContactName", SqlDbType.NVarChar).Value = memberEmerContactName;
            select.Parameters.Add("@emerContactPhone", SqlDbType.NVarChar).Value = memberEmerContactPhone;

            //開啟資料庫連線
            await connection.OpenAsync();
            select.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Update Success", "更新成功"));
        }

        [HttpPost]
        public ActionResult DeleteMember(string memberCode)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("UPDATE account_list SET status = -1 WHERE member_code = @memberCode", connection);
            select.Parameters.AddWithValue("@memberCode", memberCode);
            //開啟資料庫連線
            connection.Open();
            select.ExecuteNonQuery();
            connection.Close();
            return Json(new ApiResult<string>("Delete Success", "刪除成功"));
        }

        private async Task<bool> CheckMemberPhoneIsUsed(string memberCode, string phone)
        {
            IEnumerable<AccountEntity> accountEntity = await _accountService.GetActivateAccountByPhoneAsync(memberCode, phone);

            bool res = false;

            if (accountEntity.Any())
            {
                res = true;
            }

            return res;
        }

        private async Task<bool> CheckMemberIsExisted(string memberCode)
        {
            IEnumerable<AccountEntity> accountEntity = await _accountService.GetAccountAsync(memberCode);

            bool res = false;

            if (accountEntity.Any())
            {
                res = true;
            }

            return res;
        }

        //登入資訊物件
        public class MemberData
        {
            public int id { get; set; }
            public string memberCode { get; set; }
            public string username { get; set; }
            public string password { get; set; }
            public string email { get; set; }
            public string address { get; set; }
            public string name { get; set; }
            public string telephone { get; set; }
            public string phone { get; set; }
            public string emerContactName { get; set; }
            public string emerContactPhone { get; set; }
            public string birthday { get; set; }
            public int status { get; set; }
            public static explicit operator MemberData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        // 管理後台取得會員列表ＡＰＩ回傳格式
        public class GetMemberListApi
        {
            public int success { get; set; }
            public int draw { get; set; }
            public int recordsTotal { get; set; }
            public int recordsFiltered { get; set; }
            public List<MemberData> data { get; set; }
            public static explicit operator GetMemberListApi(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }

        // 管理後台取得會員資訊ＡＰＩ回傳格式
        public class GetMemberInfoApi
        {
            public int success { get; set; }
            public MemberData memberInfo { get; set; }
            public static explicit operator GetMemberInfoApi(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }
    }
}
