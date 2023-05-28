using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class MemberReservationRepoImpl : MemberReservationRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public MemberReservationRepoImpl()
        {
        }

        public async Task<bool> CreateAsync(MemberReservationEntity memberReservationEntity)
        {
            try
            {
                string sql = "INSERT INTO member_reservation_list (id, name, phone, birthday, meals_type, rooms_type, seat_id, boarding_id, transportation_id, travel_step_id, member_code, reservation_code, note) VALUES (@id, @name, @phone, @birthday, @mealsType, @roomsType, @seatId, @boardingId, @transportationId, @travelStepId, @memberCode, @reservationCode, @note)";

                // SQL Command
                SqlConnection connection = new SqlConnection(this.sql_DB);
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.Add("@id", SqlDbType.VarChar).Value = memberReservationEntity.id;
                select.Parameters.Add("@name", SqlDbType.NVarChar).Value = memberReservationEntity.name;
                select.Parameters.Add("@phone", SqlDbType.VarChar).Value = memberReservationEntity.phone;
                select.Parameters.Add("@birthday", SqlDbType.VarChar).Value = memberReservationEntity.birthday;
                select.Parameters.Add("@mealsType", SqlDbType.Int).Value = memberReservationEntity.mealsType;
                select.Parameters.Add("@roomsType", SqlDbType.Int).Value = memberReservationEntity.roomsType;
                select.Parameters.Add("@seatId", SqlDbType.Int).Value = memberReservationEntity.seatId;
                select.Parameters.Add("@boardingId", SqlDbType.Int).Value = memberReservationEntity.boardingId;
                select.Parameters.Add("@transportationId", SqlDbType.Int).Value = memberReservationEntity.transportationId;
                select.Parameters.Add("@travelStepId", SqlDbType.Int).Value = memberReservationEntity.travelStepId;
                select.Parameters.Add("@memberCode", SqlDbType.VarChar).Value = memberReservationEntity.memberCode;
                select.Parameters.Add("@reservationCode", SqlDbType.VarChar).Value = memberReservationEntity.reservationCode;
                select.Parameters.Add("@note", SqlDbType.Text).Value = memberReservationEntity.note;
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
                // SQL Command
                SqlConnection connection = new SqlConnection(this.sql_DB);
                SqlCommand select = new SqlCommand("DELETE member_reservation_list WHERE mr_id = @mrId and status = 1", connection);
                select.Parameters.AddWithValue("@mrId", id);
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

        public async Task<bool> DeleteBindAsync(int id)
        {
            try
            {
                // SQL Command
                SqlConnection connection = new SqlConnection(this.sql_DB);
                SqlCommand select = new SqlCommand("Update member_reservation_list set status = 0 WHERE mr_id = @mrId and status = 1", connection);
                select.Parameters.AddWithValue("@mrId", id);
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

        public async Task<bool> DeleteReservationBindAsync(string code)
        {
            try
            {
                // SQL Command
                SqlConnection connection = new SqlConnection(this.sql_DB);
                SqlCommand select = new SqlCommand("Update member_reservation_list set status = 0 WHERE reservation_code = @reservationCode and status = 1", connection);
                select.Parameters.AddWithValue("@reservationCode", code);
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

        public async Task<List<MemberReservationEntity>> GetAll()
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "mr_id as mrId, id, name, phone, birthday, meals_type as mealsType, rooms_type as roomsType, seat_id as seatId, boarding_id as boardingId, transportation_id as transportationId, travel_step_id travelStepId, member_code as memberCode, reservation_code reservationCode, note";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from member_reservation_list", connection);
            // 開啟資料庫連線
            await connection.OpenAsync();

            List<MemberReservationEntity> memberReservationEntities = new List<MemberReservationEntity>();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                MemberReservationEntity memberReservationEntity = new MemberReservationEntity
                {
                    mrId = (int)reader[0],
                    id = Convert.ToString(reader[1]),
                    name = Convert.ToString(reader[2]),
                    phone = Convert.ToString(reader[3]),
                    birthday = Convert.ToString(reader[4]),
                    mealsType = Convert.ToInt32(reader[5]),
                    roomsType = Convert.ToInt32(reader[6]),
                    seatId = Convert.ToInt32(reader[7]),
                    boardingId = Convert.ToInt32(reader[8]),
                    transportationId = Convert.ToInt32(reader[9]),
                    travelStepId = Convert.ToInt32(reader[10]),
                    memberCode = Convert.ToString(reader[11]),
                    reservationCode = Convert.ToString(reader[12]),
                    note = Convert.ToString(reader[13])
                };
                memberReservationEntities.Add(memberReservationEntity);
            }
            connection.Close();
            return memberReservationEntities;
        }

        public async Task<List<MemberReservationEntity>> GetByCode(string code)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "mr_id as mrId, id, name, phone, birthday, meals_type as mealsType, rooms_type as roomsType, seat_id as seatId, boarding_id as boardingId, transportation_id as transportationId, travel_step_id travelStepId, member_code as memberCode, reservation_code reservationCode, note, status";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from member_reservation_list WHERE reservation_code = @reservationCode", connection);
            select.Parameters.AddWithValue("@reservationCode", code);
            // 開啟資料庫連線
            await connection.OpenAsync();
            List<MemberReservationEntity> memberReservationEntities = new List<MemberReservationEntity>();

            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                MemberReservationEntity memberReservationEntity = new MemberReservationEntity();
                memberReservationEntity.mrId = (int)reader[0];
                memberReservationEntity.id = Convert.ToString(reader[1]);
                memberReservationEntity.name = Convert.ToString(reader[2]);
                memberReservationEntity.phone = Convert.ToString(reader[3]);
                memberReservationEntity.birthday = Convert.ToString(reader[4]);
                memberReservationEntity.mealsType = Convert.ToInt32(reader[5]);
                memberReservationEntity.roomsType = Convert.ToInt32(reader[6]);
                memberReservationEntity.seatId = Convert.ToInt32(reader[7]);
                memberReservationEntity.boardingId = Convert.ToInt32(reader[8]);
                memberReservationEntity.transportationId = Convert.ToInt32(reader[9]);
                memberReservationEntity.travelStepId = Convert.ToInt32(reader[10]);
                memberReservationEntity.memberCode = Convert.ToString(reader[11]);
                memberReservationEntity.reservationCode = Convert.ToString(reader[12]);
                memberReservationEntity.note = Convert.ToString(reader[13]);
                memberReservationEntity.status = Convert.ToInt32(reader[14]);
                memberReservationEntities.Add(memberReservationEntity);
            }
            connection.Close();
            return memberReservationEntities;
        }

        // ids : mrId:seatId:travelStepId
        public async Task<MemberReservationEntity> GetByIds(string ids, int type = 1)
        {
            string[] idArr = ids.Split(':');
            int mrId = Convert.ToInt32(idArr[0]);
            int seatId = Convert.ToInt32(idArr[1]);
            int travelStepId = Convert.ToInt32(idArr[2]);
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "mr_id as mrId, id, name, phone, birthday, meals_type as mealsType, rooms_type as roomsType, seat_id as seatId, boarding_id as boardingId, transportation_id as transportationId, travel_step_id travelStepId, member_code as memberCode, reservation_code reservationCode, note, status";
            string where = "";
            if (type == 2)
            {
                where = " WHERE seat_id = @seatId and travel_step_id = @travelStepId ";
            }
            else
            {
                where = " WHERE mr_id = @mrId ";
            }
            SqlCommand select = new SqlCommand("select " + fieldSql + " from member_reservation_list " + where, connection);
            select.Parameters.AddWithValue("@mrId", mrId);
            select.Parameters.AddWithValue("@seatId", seatId);
            select.Parameters.AddWithValue("@travelStepId", travelStepId);
            // 開啟資料庫連線
            await connection.OpenAsync();
            MemberReservationEntity memberReservationEntity = new MemberReservationEntity();
            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                memberReservationEntity.mrId = (int)reader[0];
                memberReservationEntity.id = Convert.ToString(reader[1]);
                memberReservationEntity.name = Convert.ToString(reader[2]);
                memberReservationEntity.phone = Convert.ToString(reader[3]);
                memberReservationEntity.birthday = Convert.ToString(reader[4]);
                memberReservationEntity.mealsType = Convert.ToInt32(reader[5]);
                memberReservationEntity.roomsType = Convert.ToInt32(reader[6]);
                memberReservationEntity.seatId = Convert.ToInt32(reader[7]);
                memberReservationEntity.boardingId = Convert.ToInt32(reader[8]);
                memberReservationEntity.transportationId = Convert.ToInt32(reader[9]);
                memberReservationEntity.travelStepId = Convert.ToInt32(reader[10]);
                memberReservationEntity.memberCode = Convert.ToString(reader[11]);
                memberReservationEntity.reservationCode = Convert.ToString(reader[12]);
                memberReservationEntity.note = Convert.ToString(reader[13]);
                memberReservationEntity.status = Convert.ToInt32(reader[14]);
            }
            connection.Close();
            return memberReservationEntity;
        }

        public async Task<List<MemberReservationEntity>> GetByMemberCode(string code)
        {
            SqlConnection connection = new SqlConnection(this.sql_DB);
            // SQL Command
            string fieldSql = "mr_id as mrId, id, name, phone, birthday, meals_type as mealsType, rooms_type as roomsType, seat_id as seatId, boarding_id as boardingId, transportation_id as transportationId, travel_step_id travelStepId, member_code as memberCode, reservation_code reservationCode, note, status";
            SqlCommand select = new SqlCommand("select " + fieldSql + " from member_reservation_list WHERE member_code = @memberCode", connection);
            select.Parameters.AddWithValue("@memberCode", code);
            // 開啟資料庫連線
            await connection.OpenAsync();
            List<MemberReservationEntity> memberReservationEntities = new List<MemberReservationEntity>();

            SqlDataReader reader = select.ExecuteReader();
            while (reader.Read())
            {
                MemberReservationEntity memberReservationEntity = new MemberReservationEntity();
                memberReservationEntity.mrId = (int)reader[0];
                memberReservationEntity.id = Convert.ToString(reader[1]);
                memberReservationEntity.name = Convert.ToString(reader[2]);
                memberReservationEntity.phone = Convert.ToString(reader[3]);
                memberReservationEntity.birthday = Convert.ToString(reader[4]);
                memberReservationEntity.mealsType = Convert.ToInt32(reader[5]);
                memberReservationEntity.roomsType = Convert.ToInt32(reader[6]);
                memberReservationEntity.seatId = Convert.ToInt32(reader[7]);
                memberReservationEntity.boardingId = Convert.ToInt32(reader[8]);
                memberReservationEntity.transportationId = Convert.ToInt32(reader[9]);
                memberReservationEntity.travelStepId = Convert.ToInt32(reader[10]);
                memberReservationEntity.memberCode = Convert.ToString(reader[11]);
                memberReservationEntity.reservationCode = Convert.ToString(reader[12]);
                memberReservationEntity.note = Convert.ToString(reader[13]);
                memberReservationEntity.status = Convert.ToInt32(reader[14]);
                memberReservationEntities.Add(memberReservationEntity);
            }
            connection.Close();
            return memberReservationEntities;
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateAsync(MemberReservationEntity memberReservationEntity)
        {
            throw new NotImplementedException();
        }
    }
}
