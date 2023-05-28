using System;
using System.Data;
using System.Web.Configuration;

namespace travelManagement.Models
{
    public class Order
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
        Datebase dbOperation = new Datebase();

        //新增訂單資料
        public int insertOrder(string[] order)
        {
            string SQL = "INSERT INTO order_list (order_code, member_code, reservation_num, trade_amt, trade_date) VALUES ";
            SQL += "('{0}', '{1}', '{2}', '{3}', '{4}')";
            string combine = String.Format(SQL, order[0], order[1], order[2], order[3], order[4]);
            return dbOperation.InsUpdTable(combine);
        }

        //更新訂單表某個欄位
        public int updateOrder(string column, string upd_value, string order_code)
        {
            string SQL = "UPDATE order_list SET " + column + " = '" + upd_value + "' WHERE order_code = '" + order_code + "'";
            return dbOperation.InsUpdTable(SQL);
        }

        //查詢訂單表
        public DataTable searchOrder(string order_code)
        {
            string SQL = "SELECT ol.*, tl.travel_name FROM order_list ol inner join reservation_list rl ON rl.reservation_code COLLATE Latin1_General_CS_AS = ol.order_code COLLATE Latin1_General_CS_AS inner join travel_list tl ON tl.travel_id = rl.travel_id WHERE ol.order_code = '" + order_code + "'";
            return dbOperation.SelectTable(SQL);
        }
    }
}
