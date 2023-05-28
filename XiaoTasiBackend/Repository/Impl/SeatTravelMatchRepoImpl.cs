using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class SeatTravelMatchRepoImpl : SeatTravelMatchRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public SeatTravelMatchRepoImpl()
        {
        }

        public async Task<bool> CreateAsync(SeatTravelMatchEntity seatEntity)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DelSeatTravelBindAsync(int seatId, int travelStepId)
        {
            try
            {
                // SQL Command
                SqlConnection connection = new SqlConnection(this.sql_DB);
                SqlCommand select = new SqlCommand("Update seat_travel_match_list set status = -1 WHERE seat_id = @seatId and travel_step_id = @travelStepId and status = 0", connection);
                select.Parameters.AddWithValue("@seatId", seatId);
                select.Parameters.AddWithValue("@travelStepId", travelStepId);
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
            throw new NotImplementedException();
        }

        public async Task<List<SeatTravelMatchEntity>> GetAll()
        {
            throw new NotImplementedException();
        }

        public SeatTravelMatchEntity GetByCode(string code)
        {
            throw new NotImplementedException();
        }

        public SeatTravelMatchEntity GetById(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(SeatTravelMatchEntity seatEntity)
        {
            throw new NotImplementedException();
        }
    }
}
