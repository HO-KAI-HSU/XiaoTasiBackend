using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using Dapper;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class TransportationRepoImpl : TransportationRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public TransportationRepoImpl()
        {
        }

        public TransportationEntity GetById(int id)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select transportation_id as transportationId, transportation_name as transportationName, transportation_licenses_number as transportationLicensesNumber, transportation_interior_pic_path as transportationInteriorPicPath from transportation_list WHERE transportation_id = @transportationId", connection);
            select.Parameters.AddWithValue("@transportationId", id);
            // 開啟資料庫連線
            connection.Open();
            TransportationEntity transportationEntity = new TransportationEntity();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                transportationEntity.transportationId = (int)reader[0];
                transportationEntity.transportationName = reader.IsDBNull(1) ? "" : (string)reader[1];
                transportationEntity.transportationLicensesNumber = reader.IsDBNull(2) ? "" : (string)reader[2];
                transportationEntity.transportationInteriorPicPath = reader.IsDBNull(3) ? "" : (string)reader[3];
            }
            connection.Close();
            return transportationEntity;
        }

        public TransportationEntity GetByCode(string code)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            SqlCommand select = new SqlCommand("select transportation_id as transportationId, transportation_name as transportationName, transportation_licenses_number as transportationLicensesNumber, transportation_interior_pic_path as transportationInteriorPicPath from transportation_list WHERE transportation_licenses_number = @transportationCode", connection);
            select.Parameters.AddWithValue("@transportationCode", code);
            // 開啟資料庫連線
            connection.Open();
            TransportationEntity transportationEntity = new TransportationEntity();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                transportationEntity.transportationId = (int)reader[0];
                transportationEntity.transportationName = reader.IsDBNull(1) ? "" : (string)reader[1];
                transportationEntity.transportationLicensesNumber = reader.IsDBNull(2) ? "" : (string)reader[2];
                transportationEntity.transportationInteriorPicPath = reader.IsDBNull(3) ? "" : (string)reader[3];
            }
            connection.Close();
            return transportationEntity;
        }

        public async Task<List<TransportationEntity>> GetAll()
        {
            List<TransportationEntity> results = null;
            using (SqlConnection conn = new SqlConnection(this.sql_DB))
            {
                await conn.OpenAsync();
                string strSql = "select transportation_id as transportationId, transportation_name as transportationName, transportation_licenses_number as transportationLicensesNumber, transportation_interior_pic_path as transportationInteriorPicPath from transportation_list";
                var data = await conn.QueryAsync<TransportationEntity>(strSql);
                results = data.ToList();
            }
            return results;
        }

        public async Task<bool> UpdateAsync(TransportationEntity transportationEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                // SQL Command
                SqlCommand select = new SqlCommand("UPDATE transportation_list SET transportation_name = @transportationName, transportation_licenses_number = @transportationLicensesNumber  WHERE transportation_id = @transportationId", connection);
                select.Parameters.AddWithValue("@transportationId", transportationEntity.transportationId);
                select.Parameters.Add("@transportationName", SqlDbType.NVarChar).Value = transportationEntity.transportationName;
                select.Parameters.Add("@transportationLicensesNumber", SqlDbType.VarChar).Value = transportationEntity.transportationLicensesNumber;
                //開啟資料庫連線s
                await connection.OpenAsync();
                select.ExecuteNonQuery();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> CreateAsync(TransportationEntity transportationEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                // SQL Command
                SqlCommand select = new SqlCommand("INSERT INTO transportation_list (transportation_name, transportation_licenses_number, transportation_interior_pic_path) " +
                "VALUES (@transportationName, @transportationLicensesNumber, @transportationInteriorPicPath)", connection);
                select.Parameters.Add("@transportationName", SqlDbType.NVarChar).Value = transportationEntity.transportationName;
                select.Parameters.Add("@transportationLicensesNumber", SqlDbType.VarChar).Value = transportationEntity.transportationLicensesNumber;
                select.Parameters.Add("@transportationInteriorPicPath", SqlDbType.VarChar).Value = "";
                //開啟資料庫連線
                await connection.OpenAsync();
                select.ExecuteNonQuery();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                // SQL Command
                SqlCommand select = new SqlCommand("DELETE transportation_list WHERE transportation_id = @transportationId", connection);
                select.Parameters.AddWithValue("@transportationId", id);
                //開啟資料庫連線
                await connection.OpenAsync();
                select.ExecuteNonQuery();
                connection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
    }
}