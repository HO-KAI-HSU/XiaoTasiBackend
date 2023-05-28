using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class IndexBannerRepoImpl : IndexBannerRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public IndexBannerRepoImpl()
        {
        }

        public async Task<bool> CreateAsync(IndexBannerEntity indexBannerEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);

                // SQL Command
                DateTime stime = DateTime.Parse(indexBannerEntity.indexBannerStime);
                DateTime etime = DateTime.Parse(indexBannerEntity.indexBannerEtime);
                string format = "yyyy-MM-dd HH:mm:ss";
                string sql = "INSERT INTO index_banner_list (index_banner_title, index_banner_content, publish_s_time, publish_e_time, index_banner_pic_path) VALUES (@indexBannerTitle, @indexBannerContent, @publishStime, @publishEtime, @indexBannerPicPath) ";

                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.Add("@indexBannerTitle", SqlDbType.NVarChar).Value = indexBannerEntity.indexBannerTitle;
                select.Parameters.Add("@indexBannerContent", SqlDbType.NVarChar).Value = indexBannerEntity.indexBannerContent;
                select.Parameters.Add("@publishStime", SqlDbType.DateTime).Value = stime.ToString(format);
                select.Parameters.Add("@publishEtime", SqlDbType.DateTime).Value = etime.ToString(format);
                select.Parameters.Add("@indexBannerPicPath", SqlDbType.VarChar).Value = indexBannerEntity.indexBannerPicPath;

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
                string sql = "DELETE index_banner_list WHERE index_banner_id = @indexBannerId";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@indexBannerId", id);

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

        public async Task<List<IndexBannerEntity>> GetAll()
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "select index_banner_id, index_banner_pic_path, publish_s_time, publish_e_time, f_date, index_banner_title, index_banner_content from index_banner_list";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);

                // 開啟資料庫連線
                await connection.OpenAsync();
                SqlDataReader reader = select.ExecuteReader();
                List<IndexBannerEntity> indexBannerEntities = new List<IndexBannerEntity>();
                while (reader.Read())
                {
                    IndexBannerEntity indexBannerEntity = new IndexBannerEntity();
                    indexBannerEntity.indexBannerId = (int)reader[0];
                    indexBannerEntity.indexBannerTitle = reader.IsDBNull(5) ? "" : (string)reader[5];
                    indexBannerEntity.indexBannerContent = reader.IsDBNull(6) ? "" : (string)reader[6];
                    indexBannerEntity.indexBannerPicPath = reader.IsDBNull(1) ? "" : (string)reader[1];
                    string format = "yyyy-MM-dd HH:mm:ss";
                    indexBannerEntity.indexBannerStime = reader.IsDBNull(2) ? "" : ((DateTime)reader[2]).ToString(format);
                    indexBannerEntity.indexBannerEtime = reader.IsDBNull(3) ? "" : ((DateTime)reader[3]).ToString(format);
                    indexBannerEntity.fDate = ((DateTime)reader[4]).ToString(format);
                    indexBannerEntities.Add(indexBannerEntity);
                }
                connection.Close();

                return indexBannerEntities;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task<IndexBannerEntity> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(IndexBannerEntity indexBannerEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);

                // SQL Command
                DateTime stime = DateTime.Parse(indexBannerEntity.indexBannerStime);
                DateTime etime = DateTime.Parse(indexBannerEntity.indexBannerEtime);
                string format = "yyyy-MM-dd HH:mm:ss";
                string sql = "UPDATE index_banner_list SET index_banner_title = @indexBannerTitle, index_banner_content = @indexBannerContent, publish_s_time = @publishStime, publish_e_time = @publishEtime, index_banner_pic_path = @indexBannerPicPath WHERE index_banner_id = @indexBannerId";

                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@indexBannerId", indexBannerEntity.indexBannerId);
                select.Parameters.Add("@indexBannerTitle", SqlDbType.NVarChar).Value = indexBannerEntity.indexBannerTitle;
                select.Parameters.Add("@indexBannerContent", SqlDbType.NVarChar).Value = indexBannerEntity.indexBannerContent;
                select.Parameters.Add("@publishStime", SqlDbType.DateTime).Value = stime.ToString(format);
                select.Parameters.Add("@publishEtime", SqlDbType.DateTime).Value = etime.ToString(format);
                select.Parameters.Add("@indexBannerPicPath", SqlDbType.VarChar).Value = indexBannerEntity.indexBannerPicPath;

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
