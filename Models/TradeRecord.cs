using System;
using System.Web.Configuration;

namespace XiaoTasiBackend.Models
{
    public class TradeRecord
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
        //Datebase dbOperation = new Datebase();

        ////新增交易資訊表列
        //public int insertTradeRecord(string[] ec)
        //{
        //    string SQL = "INSERT INTO trade_record_list (order_code, rtn_code, rtn_msg, trade_no, pay_way, bank_code, payment_no, expired_date, trade_amt) VALUES ";
        //    SQL += "('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')";
        //    string combine = String.Format(SQL, ec[0], ec[1], ec[2], ec[3], ec[4], ec[5], ec[6], ec[7], ec[8], ec[9]);
        //    return dbOperation.InsUpdTable(combine);
        //}
    }
}
