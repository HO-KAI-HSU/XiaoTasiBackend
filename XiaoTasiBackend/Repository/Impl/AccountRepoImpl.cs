using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Controllers;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class AccountRepoImpl : AccountRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public Task<bool> CreateAsync(AccountEntity seatEntity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<List<AccountEntity>> GetAll()
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                // SQL Command
                string fieldSql = "member_code as memberCode, phone, status";
                SqlCommand select = new SqlCommand("select " + fieldSql + " from account_list", connection);
                // 開啟資料庫連線
                await connection.OpenAsync();
                SqlDataReader reader = select.ExecuteReader();
                List<AccountEntity> accountEntities = new List<AccountEntity>();
                while (reader.Read())
                {
                    AccountEntity accountEntity = new AccountEntity();
                    accountEntity.memberCode = (string)reader[0];
                    accountEntity.phone = (string)reader[1];
                    accountEntity.status = (int)reader[2];
                    accountEntities.Add(accountEntity);
                }
                connection.Close();

                return accountEntities;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<AccountEntity> GetByCode(string code)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "member_code as memberCode, phone, status";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from account_list WHERE member_code = @memberCode", connection);
            select.Parameters.AddWithValue("@memberCode", code);
            // 開啟資料庫連線
            await connection.OpenAsync();
            AccountEntity accountEntity = new AccountEntity();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                accountEntity.memberCode = (string)reader[0];
                accountEntity.phone = (string)reader[1];
                accountEntity.status = (int)reader[2];
            }
            connection.Close();
            return accountEntity;
        }

        public async Task<AccountEntity> GetByPhone(string phone)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "member_code as memberCode, phone, status";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from account_list WHERE phone = @phone", connection);
            select.Parameters.AddWithValue("@phone", phone);
            // 開啟資料庫連線
            await connection.OpenAsync();
            AccountEntity accountEntity = new AccountEntity();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                accountEntity.memberCode = (string)reader[0];
                accountEntity.phone = (string)reader[1];
                accountEntity.status = (int)reader[2];
            }
            connection.Close();
            return accountEntity;
        }

        public async Task<bool> UpdatePhoneAsync(AccountEntity accountEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);

                // SQL Command
                string sql = "UPDATE account_list SET phone = @phone where member_code = @memberCode";

                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@memberCode", accountEntity.memberCode);
                select.Parameters.Add("@phone", SqlDbType.VarChar).Value = accountEntity.phone;

                ////開啟資料庫連線
                await connection.OpenAsync();
                select.ExecuteNonQuery();
                connection.Close();

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> UpdateStatusAsync(AccountEntity accountEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);

                // SQL Command
                string sql = "UPDATE account_list SET phone = @phone where member_code = @memberCode";

                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@memberCode", accountEntity.memberCode);
                select.Parameters.Add("@status", SqlDbType.Int).Value = accountEntity.status;

                ////開啟資料庫連線
                await connection.OpenAsync();
                select.ExecuteNonQuery();
                connection.Close();

                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
