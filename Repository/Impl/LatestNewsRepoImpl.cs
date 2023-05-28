using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class LatestNewsRepoImpl : LatestNewsRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public LatestNewsRepoImpl()
        {
        }

        public async Task<bool> CreateAsync(LatestNewsEntity latestNewsEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);

                // SQL Command
                DateTime stime = DateTime.Parse(latestNewsEntity.publishStime);
                DateTime etime = DateTime.Parse(latestNewsEntity.publishEtime);

                string format = "yyyy-MM-dd HH:mm:ss";
                string sql = "INSERT INTO latest_news_list (latest_news_title, latest_news_content, publish_s_time, publish_e_time, latest_news_pic_path) " +
                    "VALUES (@latestNewsTitle,  @latestNewsContent, @latestNewsStime, @latestNewsEtime, @latestNewsPicPath)";

                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.Add("@latestNewsStime", SqlDbType.DateTime).Value = stime.ToString(format);
                select.Parameters.Add("@latestNewsEtime", SqlDbType.DateTime).Value = etime.ToString(format);
                select.Parameters.Add("@latestNewsPicPath", SqlDbType.VarChar).Value = latestNewsEntity.latestNewsPicPath;
                select.Parameters.Add("@latestNewsTitle", SqlDbType.NVarChar).Value = latestNewsEntity.latestNewsTitle;
                select.Parameters.Add("@latestNewsContent", SqlDbType.NVarChar).Value = latestNewsEntity.latestNewsContent;

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

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "DELETE latest_news_list WHERE latest_news_id = @latestNewsId";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@latestNewsId", id);

                //開啟資料庫連線
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

        public async Task<List<LatestNewsEntity>> GetAll()
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "select latest_news_id, latest_news_title, latest_news_content, latest_news_pic_path, f_date, publish_s_time as publishStime, publish_e_time as publishEtime from latest_news_list";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);

                // 開啟資料庫連線
                await connection.OpenAsync();
                SqlDataReader reader = select.ExecuteReader();
                List<LatestNewsEntity> latestNewsEntities = new List<LatestNewsEntity>();
                while (reader.Read())
                {
                    LatestNewsEntity latestNewsEntity = new LatestNewsEntity();
                    latestNewsEntity.latestNewsId = (int)reader[0];
                    latestNewsEntity.latestNewsTitle = reader.IsDBNull(1) ? "" : (string)reader[1];
                    latestNewsEntity.latestNewsContent = reader.IsDBNull(2) ? "" : (string)reader[2];
                    latestNewsEntity.latestNewsPicPath = reader.IsDBNull(3) ? "" : (string)reader[3];
                    string format = "yyyy-MM-dd HH:mm:ss";
                    latestNewsEntity.fDate = ((DateTime)reader[4]).ToString(format);
                    latestNewsEntity.publishStime = reader.IsDBNull(5) ? "" : ((DateTime)reader[5]).ToString(format);
                    latestNewsEntity.publishEtime = reader.IsDBNull(6) ? "" : ((DateTime)reader[6]).ToString(format);
                    latestNewsEntities.Add(latestNewsEntity);
                }
                connection.Close();

                return latestNewsEntities;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task<LatestNewsEntity> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(LatestNewsEntity latestNewsEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);

                // SQL Command
                DateTime stime = DateTime.Parse(latestNewsEntity.publishStime);
                DateTime etime = DateTime.Parse(latestNewsEntity.publishEtime);

                string format = "yyyy-MM-dd HH:mm:ss";
                string sql = "UPDATE latest_news_list SET latest_news_title = @latestNewsTitle, latest_news_content = @latestNewsContent, publish_s_time = @latestNewsStime, publish_e_time = @latestNewsEtime, latest_news_pic_path = @latestNewsPicPath WHERE latest_news_id = @latestNewsId";

                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@latestNewsId", latestNewsEntity.latestNewsId);
                select.Parameters.Add("@latestNewsStime", SqlDbType.DateTime).Value = stime.ToString(format);
                select.Parameters.Add("@latestNewsEtime", SqlDbType.DateTime).Value = etime.ToString(format);
                select.Parameters.Add("@latestNewsPicPath", SqlDbType.VarChar).Value = latestNewsEntity.latestNewsPicPath;
                select.Parameters.Add("@latestNewsTitle", SqlDbType.NVarChar).Value = latestNewsEntity.latestNewsTitle;
                select.Parameters.Add("@latestNewsContent", SqlDbType.NVarChar).Value = latestNewsEntity.latestNewsContent;

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
