using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class ManagerRepoImpl : ManagerRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public ManagerRepoImpl()
        {
        }

        public async Task<ManagerEntity> GetByUsername(string username)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "select id, username, password, name, email, role_id as roleId, is_modify as isModify, status from management_user WHERE username = @username";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@username", username);

                // 開啟資料庫連線
                await connection.OpenAsync();
                ManagerEntity managerEntity = new ManagerEntity();
                SqlDataReader reader = select.ExecuteReader();
                while (reader.Read())
                {
                    managerEntity.id = (int)reader[0];
                    string pwd = (string)reader[2];
                    managerEntity.username = (string)reader[1];
                    managerEntity.password = (string)pwd;
                    managerEntity.name = (string)reader[3];
                    managerEntity.email = (string)reader[4];
                    managerEntity.roleId = (int)reader[5];
                    managerEntity.isModify = (bool)reader[6];
                    managerEntity.status = (bool)reader[7];
                }
                connection.Close();

                return managerEntity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
