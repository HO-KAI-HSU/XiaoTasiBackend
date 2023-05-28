using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class ViewpointRepoImpl : ViewpointRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public ViewpointRepoImpl()
        {

        }

        public async Task<bool> CreateAsync(ViewpointEntity viewpointEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "INSERT INTO viewpoint_list (viewpoint_title, viewpoint_content, viewpoint_city, viewpoint_area, viewpoint_address, viewpoint_pic_path, f_date, e_date) VALUES (@viewpointTitle, @viewpointContent, @viewpointCity, @viewpointArea, @viewpointAddress, @viewpointPicPath, @fDate, @eDate)";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.Add("@viewpointTitle", SqlDbType.NVarChar).Value = viewpointEntity.viewpointTitle;
                select.Parameters.Add("@viewpointContent", SqlDbType.NVarChar).Value = viewpointEntity.viewpointContent;
                select.Parameters.Add("@viewpointCity", SqlDbType.NVarChar).Value = viewpointEntity.viewpointCity;
                select.Parameters.Add("@viewpointArea", SqlDbType.NVarChar).Value = viewpointEntity.viewpointArea;
                select.Parameters.Add("@viewpointAddress", SqlDbType.NVarChar).Value = viewpointEntity.viewpointAddress;
                select.Parameters.Add("@viewpointPicPath", SqlDbType.NVarChar).Value = viewpointEntity.viewpointPicPath;
                select.Parameters.Add("@fDate", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                select.Parameters.Add("@eDate", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "DELETE viewpoint_list WHERE viewpoint_id = @viewpointId";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@viewpointId", id);

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

        public async Task<List<ViewpointEntity>> GetAll()
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "select viewpoint_id, viewpoint_title, viewpoint_content, viewpoint_city, viewpoint_area, viewpoint_address, viewpoint_pic_path, f_date, e_date from viewpoint_list";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);

                // 開啟資料庫連線
                await connection.OpenAsync();
                SqlDataReader reader = select.ExecuteReader();
                List<ViewpointEntity> virepointEntities = new List<ViewpointEntity>();
                while (reader.Read())
                {
                    ViewpointEntity virepointEntity = new ViewpointEntity();
                    virepointEntity.viewpointId = (int)reader[0];
                    virepointEntity.viewpointTitle = reader[1].ToString();
                    virepointEntity.viewpointContent = reader[2].ToString();
                    virepointEntity.viewpointCity = reader[3].ToString();
                    virepointEntity.viewpointArea = reader[4].ToString();
                    virepointEntity.viewpointAddress = reader[5].ToString();
                    virepointEntity.viewpointPicPath = reader[6].ToString();
                    string format = "yyyy-MM-dd HH:mm:ss";
                    virepointEntity.fDate = reader.IsDBNull(7) ? string.Empty : ((DateTime)reader[7]).ToString(format);
                    virepointEntity.eDate = reader.IsDBNull(8) ? string.Empty : ((DateTime)reader[8]).ToString(format);
                    virepointEntities.Add(virepointEntity);
                }
                connection.Close();

                return virepointEntities;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ViewpointEntity> GetById(int id)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "select viewpoint_id, viewpoint_title, viewpoint_content, viewpoint_city, viewpoint_area, viewpoint_address, viewpoint_pic_path, f_date, e_date from viewpoint_list WHERE hotel_id = @hotelId";

                // SQL Commands
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@hotelId", id);

                // 開啟資料庫連線
                await connection.OpenAsync();
                ViewpointEntity virepointEntity = new ViewpointEntity();
                SqlDataReader reader = select.ExecuteReader();
                while (reader.Read())
                {
                    virepointEntity.viewpointId = (int)reader[0];
                    virepointEntity.viewpointTitle = reader[1].ToString();
                    virepointEntity.viewpointContent = reader[2].ToString();
                    virepointEntity.viewpointCity = reader[3].ToString();
                    virepointEntity.viewpointArea = reader[4].ToString();
                    virepointEntity.viewpointAddress = reader[5].ToString();
                    virepointEntity.viewpointPicPath = reader[6].ToString();
                    string format = "yyyy-MM-dd HH:mm:ss";
                    virepointEntity.fDate = reader.IsDBNull(7) ? string.Empty : ((DateTime)reader[7]).ToString(format);
                    virepointEntity.eDate = reader.IsDBNull(8) ? string.Empty : ((DateTime)reader[8]).ToString(format);
                }
                connection.Close();

                return virepointEntity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> UpdateAsync(ViewpointEntity viewpointEntity)
        {
            try
            {
                // SQL Command
                SqlConnection connection = new SqlConnection(this.sql_DB);
                SqlCommand select;
                if (!string.IsNullOrEmpty(viewpointEntity.viewpointPicPath))
                {
                    select = new SqlCommand("UPDATE viewpoint_list SET viewpoint_title = @viewpointTitle, viewpoint_city = @viewpointCity, viewpoint_area = @viewpointArea, viewpoint_address = @viewpointAddress, viewpoint_content = @viewpointContent, viewpoint_pic_path = @viewpointPicPath, e_date = @eDate WHERE viewpoint_id = @viewpointId", connection);
                    select.Parameters.Add("@hotelPicPath", SqlDbType.NVarChar).Value = viewpointEntity.viewpointPicPath;
                }
                else
                {
                    select = new SqlCommand("UPDATE viewpoint_list SET viewpoint_title = @viewpointTitle, viewpoint_city = @viewpointCity, viewpoint_area = @viewpointArea, viewpoint_address = @viewpointAddress, viewpoint_content = @viewpointContent, e_date = @eDate WHERE viewpoint_id = @viewpointId", connection);
                }

                select.Parameters.AddWithValue("@viewpointId", viewpointEntity.viewpointId);
                select.Parameters.Add("@viewpointTitle", SqlDbType.NVarChar).Value = viewpointEntity.viewpointTitle;
                select.Parameters.Add("@viewpointContent", SqlDbType.NVarChar).Value = viewpointEntity.viewpointContent;
                select.Parameters.Add("@viewpointCity", SqlDbType.NVarChar).Value = viewpointEntity.viewpointCity;
                select.Parameters.Add("@viewpointArea", SqlDbType.NVarChar).Value = viewpointEntity.viewpointArea;
                select.Parameters.Add("@viewpointAddress", SqlDbType.NVarChar).Value = viewpointEntity.viewpointAddress;
                select.Parameters.Add("@viewpointPicPath", SqlDbType.NVarChar).Value = viewpointEntity.viewpointPicPath;
                select.Parameters.Add("@eDate", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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

        public async Task<bool> MultiAddAsync(string sql)
        {
            try
            {
                // SQL Command
                SqlConnection connection = new SqlConnection(this.sql_DB);
                SqlCommand select = new SqlCommand(sql, connection);

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
    }
}
