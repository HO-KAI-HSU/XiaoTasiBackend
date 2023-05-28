using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class ReservationRepoImpl : ReservationRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public ReservationRepoImpl()
        {
        }

        public async Task<bool> CreateAsync(ReservationEntity transportationEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);

                // SQL Command
                string sql = "INSERT INTO reservation_list(reservation_code, reservation_num, reservation_cost, seat_ids, note, travel_id, travel_step_id, member_code) VALUES (@reservationCode, @reservationNum, @reservationCost, @seatIds, @note, @travelId, @travelStepId, @memberCode)";
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.Add("@reservationCode", SqlDbType.VarChar).Value = transportationEntity.reservationCode;
                select.Parameters.Add("@reservationNum", SqlDbType.Int).Value = transportationEntity.reservationNum;
                select.Parameters.Add("@reservationCost", SqlDbType.Int).Value = transportationEntity.reservationCost;
                select.Parameters.Add("@seatIds", SqlDbType.VarChar).Value = transportationEntity.seatIds;
                select.Parameters.Add("@note", SqlDbType.NVarChar).Value = "";
                select.Parameters.Add("@travelId", SqlDbType.Int).Value = transportationEntity.travelId;
                select.Parameters.Add("@travelStepId", SqlDbType.Int).Value = transportationEntity.travelStepId;
                select.Parameters.Add("@memberCode", SqlDbType.VarChar).Value = transportationEntity.memberCode;
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

                // SQL Command
                SqlCommand select = new SqlCommand("DELETE reservation_list WHERE reservation_id = @reservationId and status >= 0", connection);
                select.Parameters.AddWithValue("@reservationId", id);

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

        public async Task<bool> DeleteBindAsync(int id)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);

                // SQL Command
                SqlCommand select = new SqlCommand("UPDATE reservation_list set status = -1 WHERE reservation_id = @reservationId and status >= 0", connection);
                select.Parameters.AddWithValue("@reservationId", id);

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

        public async Task<List<ReservationEntity>> GetAll()
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                // SQL Command
                string fieldSql = "select rl.reservation_id as reservationId,  rl.reservation_code as reservationCode,  rl.member_code as memberCode,  rl.travel_id as travelId,  rl.reservation_num as reservationNum, rl.reservation_cost as reservationCost, rl.status from reservation_list rl";
                SqlCommand select = new SqlCommand(fieldSql, connection);
                // 開啟資料庫連線
                await connection.OpenAsync();
                SqlDataReader reader = select.ExecuteReader();
                List<ReservationEntity> reservationEntities = new List<ReservationEntity>();
                while (reader.Read())
                {
                    ReservationEntity reservationEntity = new ReservationEntity();
                    reservationEntity.reservationId = (int)reader[0];
                    reservationEntity.reservationCode = reader.IsDBNull(1) ? "" : (string)reader[1];
                    reservationEntity.memberCode = (string)reader[2];
                    reservationEntity.travelId = (int)reader[3];
                    reservationEntity.reservationNum = (int)reader[4];
                    reservationEntity.reservationCost = (int)reader[5];
                    reservationEntity.status = (int)reader[6];
                    reservationEntities.Add(reservationEntity);
                }
                connection.Close();

                return reservationEntities;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<ReservationEntity> GetByCode(string code)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "reservation_id as reservationId, reservation_code as reservationCode, member_code as memberCode, travel_id as travelId, reservation_num as reservationNum, reservation_cost as reservationCost, status";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from reservation_list WHERE reservation_code = @reservationCode", connection);
            select.Parameters.AddWithValue("@reservationCode", code);
            // 開啟資料庫連線
            await connection.OpenAsync();
            ReservationEntity reservationEntity = new ReservationEntity();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                reservationEntity.reservationId = (int)reader[0];
                reservationEntity.reservationCode = (string)reader[1];
                reservationEntity.memberCode = (string)reader[2];
                reservationEntity.travelId = (int)reader[3];
                reservationEntity.reservationNum = (int)reader[4];
                reservationEntity.reservationCost = (int)reader[5];
                reservationEntity.status = (int)reader[6];
            }
            connection.Close();
            return reservationEntity;
        }

        public ReservationEntity GetById(int id)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "reservation_id as reservationId, reservation_code as reservationCode, member_code as memberCode, travel_id as travelId, reservation_num as reservationNum, reservation_cost as reservationCost, status";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from reservation_list WHERE reservation_id = @reservationId", connection);
            select.Parameters.AddWithValue("@reservationId", id);
            // 開啟資料庫連線
            connection.Open();
            ReservationEntity reservationEntity = new ReservationEntity();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                reservationEntity.reservationId = (int)reader[0];
                reservationEntity.reservationCode = (string)reader[1];
                reservationEntity.memberCode = (string)reader[2];
                reservationEntity.travelId = (int)reader[3];
                reservationEntity.reservationNum = (int)reader[4];
                reservationEntity.reservationCost = (int)reader[5];
                reservationEntity.status = (int)reader[6];
            }
            connection.Close();
            return reservationEntity;
        }

        public async Task<bool> UpdateAsync(ReservationEntity transportationEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);

                // SQL Command
                SqlCommand select = new SqlCommand("UPDATE reservation_list SET reservation_num = @reservationNum, reservation_cost = @reservationCost, seat_ids = @seatIds WHERE reservation_id = @reservationId and status >= 0", connection);
                select.Parameters.AddWithValue("@reservationId", transportationEntity.reservationId);
                select.Parameters.Add("@reservationNum", SqlDbType.Int).Value = transportationEntity.reservationNum;
                select.Parameters.Add("@reservationCost", SqlDbType.Int).Value = transportationEntity.reservationCost;
                select.Parameters.Add("@seatIds", SqlDbType.VarChar).Value = transportationEntity.seatIds;

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
