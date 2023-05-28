using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class HotelRepoImpl : HotelRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public HotelRepoImpl()
        {

        }

        public async Task<bool> CreateAsync(HotelEntity hotelEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "INSERT INTO hotel_list (hotel_name, hotel_content, hotel_city, hotel_area, hotel_address, hotel_pic_path, f_date, e_date) VALUES (@hotelName, @hotelContent, @hotelCity, @hotelArea, @hotelAddress, @hotelPicPath, @fDate, @eDate)";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.Add("@hotelName", SqlDbType.NVarChar).Value = hotelEntity.hotelName;
                select.Parameters.Add("@hotelContent", SqlDbType.NVarChar).Value = hotelEntity.hotelContent;
                select.Parameters.Add("@hotelCity", SqlDbType.NVarChar).Value = hotelEntity.hotelCity;
                select.Parameters.Add("@hotelArea", SqlDbType.NVarChar).Value = hotelEntity.hotelArea;
                select.Parameters.Add("@hotelAddress", SqlDbType.NVarChar).Value = hotelEntity.hotelAddress;
                select.Parameters.Add("@hotelPicPath", SqlDbType.NVarChar).Value = hotelEntity.hotelPicPath;
                select.Parameters.Add("@fDate", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                select.Parameters.Add("@eDate", SqlDbType.DateTime).Value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
                string sql = "DELETE hotel_list WHERE hotel_id = @hotelId";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@hotelId", id);

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

        public async Task<List<HotelEntity>> GetAll()
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "select hotel_id, hotel_name, hotel_content, hotel_city, hotel_area, hotel_address, f_date, e_date from hotel_list";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);

                // 開啟資料庫連線
                await connection.OpenAsync();
                SqlDataReader reader = select.ExecuteReader();
                List<HotelEntity> hotelEntities = new List<HotelEntity>();
                while (reader.Read())
                {
                    HotelEntity hotelEntity = new HotelEntity();
                    hotelEntity.hotelId = (int)reader[0];
                    hotelEntity.hotelName = reader[1].ToString();
                    hotelEntity.hotelContent = reader[2].ToString();
                    hotelEntity.hotelCity = reader[3].ToString();
                    hotelEntity.hotelArea = reader[4].ToString();
                    hotelEntity.hotelAddress = reader[5].ToString();
                    string format = "yyyy-MM-dd HH:mm:ss";
                    hotelEntity.fDate = reader.IsDBNull(6) ? string.Empty : ((DateTime)reader[6]).ToString(format);
                    hotelEntity.eDate = reader.IsDBNull(7) ? string.Empty : ((DateTime)reader[7]).ToString(format);
                    hotelEntities.Add(hotelEntity);
                }
                connection.Close();

                return hotelEntities;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<HotelEntity> GetById(int id)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "select hotel_id, hotel_name, hotel_content, hotel_city, hotel_area, hotel_address, f_date, e_date from hotel_list WHERE hotel_id = @hotelId";

                // SQL Commands
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@hotelId", id);

                // 開啟資料庫連線
                await connection.OpenAsync();
                HotelEntity hotelEntity = new HotelEntity();
                SqlDataReader reader = select.ExecuteReader();
                while (reader.Read())
                {
                    hotelEntity.hotelId = (int)reader[0];
                    hotelEntity.hotelName = reader[1].ToString();
                    hotelEntity.hotelContent = reader[2].ToString();
                    hotelEntity.hotelCity = reader[3].ToString();
                    hotelEntity.hotelArea = reader[4].ToString();
                    hotelEntity.hotelAddress = reader[5].ToString();
                    string format = "yyyy-MM-dd HH:mm:ss";
                    hotelEntity.fDate = reader.IsDBNull(6) ? string.Empty : ((DateTime)reader[6]).ToString(format);
                    hotelEntity.eDate = reader.IsDBNull(7) ? string.Empty : ((DateTime)reader[7]).ToString(format);
                }
                connection.Close();

                return hotelEntity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> UpdateAsync(HotelEntity hotelEntity)
        {
            try
            {
                // SQL Command
                SqlConnection connection = new SqlConnection(this.sql_DB);
                SqlCommand select;
                if (!string.IsNullOrEmpty(hotelEntity.hotelPicPath))
                {
                    select = new SqlCommand("UPDATE hotel_list SET hotel_name = @hotelName, hotel_city = @hotelCity, hotel_area = @hotelArea, hotel_address = @hotelAddress, hotel_content = @hotelContent, hotel_pic_path = @hotelPicPath, e_date = @eDate WHERE hotel_id = @hotelId", connection);
                    select.Parameters.Add("@hotelPicPath", SqlDbType.NVarChar).Value = hotelEntity.hotelPicPath;
                }
                else
                {
                    select = new SqlCommand("UPDATE hotel_list SET hotel_name = @hotelName, hotel_city = @hotelCity, hotel_area = @hotelArea, hotel_address = @hotelAddress, hotel_content = @hotelContent, e_date = @eDate WHERE hotel_id = @hotelId", connection);
                }

                select.Parameters.AddWithValue("@hotelId", hotelEntity.hotelId);
                select.Parameters.Add("@hotelName", SqlDbType.NVarChar).Value = hotelEntity.hotelName;
                select.Parameters.Add("@hotelContent", SqlDbType.NVarChar).Value = hotelEntity.hotelContent;
                select.Parameters.Add("@hotelCity", SqlDbType.NVarChar).Value = hotelEntity.hotelCity;
                select.Parameters.Add("@hotelArea", SqlDbType.NVarChar).Value = hotelEntity.hotelArea;
                select.Parameters.Add("@hotelAddress", SqlDbType.NVarChar).Value = hotelEntity.hotelAddress;
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
