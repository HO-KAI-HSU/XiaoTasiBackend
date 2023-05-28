using System;
using System.Data;
using System.Web.Configuration;

namespace travelManagement.Models
{
    public class Member
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;
        Datebase dbOperation = new Datebase();

        //尋找會員
        public DataTable SearchMember(int id)
        {
            string SQL = "SELECT * FROM member WHERE id = '" + id + "'";
            return dbOperation.SelectTable(SQL);
        }
    }
}
