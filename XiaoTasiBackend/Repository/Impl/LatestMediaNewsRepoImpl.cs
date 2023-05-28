using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class LatestMediaNewsRepoImpl : LatestMediaNewsRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public LatestMediaNewsRepoImpl()
        {
        }

        public async Task<bool> CreateAsync(LatestMediaNewsEntity latestMediaNewsEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);

                // SQL Command
                DateTime stime = DateTime.Parse(latestMediaNewsEntity.latestMediaNewsStime);
                DateTime etime = DateTime.Parse(latestMediaNewsEntity.latestMediaNewsEtime);

                string format = "yyyy-MM-dd HH:mm:ss";
                string sql = "INSERT INTO latest_media_news_list(latest_media_news_type, latest_media_news_title, latest_media_news_en_title, latest_media_news_content, latest_media_news_en_content, publish_s_time, publish_e_time, latest_media_news_url, latest_media_news_pic_path) " +
                "VALUES (@latestMediaNewsType, @latestMediaNewsTitle,  @latestMediaNewsContent, @latestMediaNewsStime, @latestMediaNewsEtime, @latestMediaNewsUrl, @latestMediaNewsPicPath)";

                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.Add("@latestMediaNewsStime", SqlDbType.DateTime).Value = stime.ToString(format);
                select.Parameters.Add("@latestMediaNewsEtime", SqlDbType.DateTime).Value = etime.ToString(format);
                select.Parameters.Add("@indexBannerPicPath", SqlDbType.VarChar).Value = latestMediaNewsEntity.latestMediaNewsPicPath;
                select.Parameters.Add("@latestMediaNewsUrl", SqlDbType.VarChar).Value = latestMediaNewsEntity.latestMediaNewsMovieUrl;
                select.Parameters.Add("@latestMediaNewsType", SqlDbType.Int).Value = latestMediaNewsEntity.latestMediaNewsType;
                select.Parameters.Add("@latestMediaNewsTitle", SqlDbType.NVarChar).Value = !string.IsNullOrEmpty(latestMediaNewsEntity.latestMediaNewsTitle) ? HttpUtility.HtmlEncode(latestMediaNewsEntity.latestMediaNewsTitle) : string.Empty;
                select.Parameters.Add("@latestMediaNewsContent", SqlDbType.NVarChar).Value = !string.IsNullOrEmpty(latestMediaNewsEntity.latestMediaNewsContent) ? HttpUtility.HtmlEncode(latestMediaNewsEntity.latestMediaNewsContent) : string.Empty;

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
                string sql = "DELETE latest_media_news_list WHERE latest_media_news_id = @latestMediaNewsId";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@latestMediaNewsId", id);

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

        public async Task<List<LatestMediaNewsEntity>> GetAll()
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "select latest_media_news_id, latest_media_news_title, latest_media_news_content, latest_media_news_url, f_date, publish_s_time, publish_e_time, latest_media_news_type, latest_media_news_pic_path from latest_media_news_list";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);

                // 開啟資料庫連線
                await connection.OpenAsync();
                SqlDataReader reader = select.ExecuteReader();
                List<LatestMediaNewsEntity> latestMediaNewsEntities = new List<LatestMediaNewsEntity>();
                while (reader.Read())
                {
                    LatestMediaNewsEntity latestMediaNewsEntity = new LatestMediaNewsEntity();
                    latestMediaNewsEntity.latestMediaNewsId = (int)reader[0];
                    latestMediaNewsEntity.latestMediaNewsTitle = reader.IsDBNull(1) ? "" : (string)reader[1];
                    latestMediaNewsEntity.latestMediaNewsContent = reader.IsDBNull(2) ? "" : (string)reader[2];
                    string[] urlStrArr = { "", "", "", "", "", "" };
                    if (!reader.IsDBNull(3))
                    {
                        string[] arr = ((string)reader[3]).Split('/');
                        Console.WriteLine(arr.Count());
                        if (arr.Count() > 1)
                        {
                            urlStrArr = arr;
                        }
                    }
                    string url = "<iframe width = '400' height = '250' src = 'https://www.youtube.com/embed/" + urlStrArr[3] + "' title = 'YouTube video player' frameborder = '0' allow = 'accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture' allowfullscreen ></iframe>";
                    latestMediaNewsEntity.latestMediaNewsMovieUrl = reader.IsDBNull(3) ? "" : url;
                    string format = "yyyy-MM-dd HH:mm:ss";
                    latestMediaNewsEntity.fDate = ((DateTime)reader[4]).ToString(format);
                    latestMediaNewsEntity.latestMediaNewsStime = ((DateTime)reader[5]).ToString(format);
                    latestMediaNewsEntity.latestMediaNewsEtime = ((DateTime)reader[6]).ToString(format);
                    latestMediaNewsEntity.latestMediaNewsType = reader.IsDBNull(7) ? 0 : (int)reader[7];
                    latestMediaNewsEntity.latestMediaNewsPicPath = reader.IsDBNull(8) ? "" : (string)reader[8];
                    latestMediaNewsEntities.Add(latestMediaNewsEntity);
                }
                connection.Close();

                return latestMediaNewsEntities;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task<LatestMediaNewsEntity> GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(LatestMediaNewsEntity latestMediaNewsEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);

                // SQL Command
                DateTime stime = DateTime.Parse(latestMediaNewsEntity.latestMediaNewsStime);
                DateTime etime = DateTime.Parse(latestMediaNewsEntity.latestMediaNewsEtime);

                string format = "yyyy-MM-dd HH:mm:ss";
                string sql = "Update latest_media_news_list SET latest_media_news_type = @latestMediaNewsType, latest_media_news_title = @latestMediaNewsTitle, latest_media_news_content = @latestMediaNewsContent, publish_s_time = @latestMediaNewsStime, publish_e_time = @latestMediaNewsEtime, latest_media_news_url = @latestMediaNewsMovieUrl, latest_media_news_pic_path = @latestMediaNewsPicPath WHERE latest_media_news_id = @latestMediaNewsId";

                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@latestMediaNewsId", latestMediaNewsEntity.latestMediaNewsId);
                select.Parameters.Add("@latestMediaNewsStime", SqlDbType.DateTime).Value = stime.ToString(format);
                select.Parameters.Add("@latestMediaNewsEtime", SqlDbType.DateTime).Value = etime.ToString(format);
                select.Parameters.Add("@indexBannerPicPath", SqlDbType.VarChar).Value = latestMediaNewsEntity.latestMediaNewsPicPath;
                select.Parameters.Add("@latestMediaNewsUrl", SqlDbType.VarChar).Value = latestMediaNewsEntity.latestMediaNewsMovieUrl;
                select.Parameters.Add("@latestMediaNewsType", SqlDbType.Int).Value = latestMediaNewsEntity.latestMediaNewsType;
                select.Parameters.Add("@latestMediaNewsTitle", SqlDbType.NVarChar).Value = latestMediaNewsEntity.latestMediaNewsTitle;
                select.Parameters.Add("@latestMediaNewsContent", SqlDbType.NVarChar).Value = latestMediaNewsEntity.latestMediaNewsContent;

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
