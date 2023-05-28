using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;
using static travelManagement.Controllers.MemberController;

namespace travelManagement.Models
{
    public class PayResult
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
        Datebase dbOperation = new Datebase();


        //新增付款結果到資料庫
        public int insertPaymentRes(string[] ec)
        {
            string SQL = "INSERT INTO pay_result_list (order_code, rtn_code, rtn_msg, trade_no, trade_amt, charge_fee, trade_date, simulate_paid) VALUES ";
            SQL += "('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}')";
            string combine = String.Format(SQL, ec[0], ec[1], ec[2], ec[3], ec[4], ec[5], ec[6], ec[7]);
            return dbOperation.InsUpdTable(combine);
        }
    }
}
