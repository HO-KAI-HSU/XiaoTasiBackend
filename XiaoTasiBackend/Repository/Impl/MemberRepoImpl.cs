using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class MemberRepoImpl : MemberRepo
    {

        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public MemberRepoImpl()
        {
        }

        public async Task<List<MemberEntity>> GetAll()
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                // SQL Command
                string fieldSql = "select ml.member_code as memberCode, ml.name, ml.email, ml.address from member_list ml";
                SqlCommand select = new SqlCommand(fieldSql, connection);
                // 開啟資料庫連線
                await connection.OpenAsync();
                SqlDataReader reader = select.ExecuteReader();
                List<MemberEntity> memberEntities = new List<MemberEntity>();
                while (reader.Read())
                {
                    MemberEntity memberEntity = new MemberEntity();
                    memberEntity.memberCode = (string)reader[0];
                    memberEntity.name = (string)reader[1];
                    memberEntity.email = (string)reader[2];
                    memberEntity.address = (string)reader[2];
                    memberEntities.Add(memberEntity);
                }
                connection.Close();

                return memberEntities;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task<MemberEntity> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<MemberEntity> GetByCode(string code)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "member_code as memberCode, name, email, address, telephone";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from member_list WHERE member_code = @memberCode", connection);
            select.Parameters.AddWithValue("@memberCode", code);
            // 開啟資料庫連線
            await connection.OpenAsync();
            MemberEntity memberEntity = new MemberEntity();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                memberEntity.memberCode = (string)reader[0];
                memberEntity.name = (string)reader[1];
                memberEntity.email = (string)reader[2];
                memberEntity.address = (string)reader[3];
                memberEntity.telephone = (string)reader[4];
            }
            connection.Close();
            return memberEntity;
        }
    }
}
