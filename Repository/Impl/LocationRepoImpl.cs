using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class LocationRepoImpl : LocationRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public async Task<bool> CreateAsync(LocationEntity locationEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "INSERT INTO location_list (location_name, location_address, location_pic_path) VALUES (@locationName, @locationAddress, @locationPicPath)";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.Add("@locationName", SqlDbType.NVarChar).Value = locationEntity.locationName;
                select.Parameters.Add("@locationAddress", SqlDbType.NVarChar).Value = locationEntity.locationAddress;
                select.Parameters.Add("@locationPicPath", SqlDbType.VarChar).Value = locationEntity.locationPicPath;

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
                string sql = "DELETE location_list WHERE location_id = @locationId";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@locationId", id);

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

        public async Task<List<LocationEntity>> GetAll()
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            string sql = "select location_id, location_name, location_address, location_pic_path from location_list";

            // SQL Command
            SqlCommand select = new SqlCommand(sql, connection);

            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();
            List<LocationEntity> locationEntities = new List<LocationEntity>();
            while (reader.Read())
            {
                LocationEntity locationEntity = new LocationEntity();
                locationEntity.locationId = (int)reader[0];
                locationEntity.locationName = reader.IsDBNull(1) ? "" : (string)reader[1];
                locationEntity.locationAddress = reader.IsDBNull(2) ? "" : (string)reader[2];
                locationEntity.locationPicPath = reader.IsDBNull(3) ? "" : (string)reader[3];
                locationEntities.Add(locationEntity);
            }
            connection.Close();

            return locationEntities;
        }

        public async Task<LocationEntity> GetById(int id)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "select location_id, location_address, location_name from location_list WHERE location_id = @locationId";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@locationId", id);

                // 開啟資料庫連線
                await connection.OpenAsync();
                LocationEntity locationEntity = new LocationEntity();
                SqlDataReader reader = select.ExecuteReader();
                while (reader.Read())
                {
                    locationEntity.locationId = (int)reader[0];
                    locationEntity.locationName = reader.IsDBNull(1) ? "" : (string)reader[1];
                    locationEntity.locationAddress = reader.IsDBNull(2) ? "" : (string)reader[2];
                }
                connection.Close();

                return locationEntity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> UpdateAsync(LocationEntity locationEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "UPDATE location_list SET location_name = @locationName, location_address = @locationAddress, location_pic_path = @locationPicPath WHERE location_id = @locationId";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@locationId", locationEntity.locationId);
                select.Parameters.Add("@locationName", SqlDbType.NVarChar).Value = locationEntity.locationName;
                select.Parameters.Add("@locationAddress", SqlDbType.NVarChar).Value = locationEntity.locationAddress;
                select.Parameters.Add("@locationPicPath", SqlDbType.VarChar).Value = locationEntity.locationPicPath;

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
