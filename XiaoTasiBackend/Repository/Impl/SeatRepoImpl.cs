using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class SeatRepoImpl : SeatRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public SeatRepoImpl()
        {
        }

        public Task<bool> CreateAsync(SeatEntity seatEntity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateAsync(SeatEntity seatEntity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<SeatEntity>> GetAll()
        {
            throw new NotImplementedException();
        }

        public SeatEntity GetByCode(string code)
        {
            throw new NotImplementedException();
        }

        public SeatEntity GetById(int id)
        {
            throw new NotImplementedException();
        }

        public int getSeatIdByTransportationIdAndPos(int transportationId, int seatPos)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            int seatId = 0;
            string sql = "select seat_id from seat_list WHERE transportation_Id = @transportationId and seat_pos = @seatPos";
            SqlCommand select = new SqlCommand(sql, connection);
            select.Parameters.AddWithValue("@transportationId", transportationId);
            select.Parameters.AddWithValue("@seatPos", seatPos);
            // 開啟資料庫連線
            connection.Open();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                seatId = (int)reader[0];
            }
            connection.Close();
            return seatId;
        }
    }
}
