using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class BoardingRepoImpl : BoardingRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public BoardingRepoImpl()
        {

        }

        public async Task<bool> CreateAsync(BoardingEntity boardingEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                DateTime stime = DateTime.Parse(boardingEntity.boardingDatetime);
                string format = "HH:mm:ss";
                string sql = "INSERT INTO boarding_list (location_id, boarding_datetime, custom_boarding_flag, early_boarding_flag) VALUES (@locationId, @boardingTime, @customBoardingFlag, @earlyBoardingFlag)";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.Add("@locationId", SqlDbType.Int).Value = boardingEntity.locationId;
                select.Parameters.Add("@boardingTime", SqlDbType.DateTime).Value = stime.ToString(format);
                select.Parameters.Add("@customBoardingFlag", SqlDbType.Int).Value = boardingEntity.customBoardingFlag;
                select.Parameters.Add("@earlyBoardingFlag", SqlDbType.Int).Value = boardingEntity.earlyBoardingFlag;

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
                string sql = "DELETE boarding_list WHERE boarding_id = @boardingId";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@boardingId", id);

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

        public async Task<List<BoardingEntity>> GetAll()
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "select bl.boarding_id, ll.location_id, bl.boarding_datetime, bl.custom_boarding_flag, bl.early_boarding_flag from boarding_list bl inner join location_list ll ON bl.location_id = ll.location_id";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);

                // 開啟資料庫連線
                await connection.OpenAsync();
                SqlDataReader reader = select.ExecuteReader();
                List<BoardingEntity> boardingEntities = new List<BoardingEntity>();
                while (reader.Read())
                {
                    BoardingEntity boardingEntity = new BoardingEntity();
                    boardingEntity.boardingId = (int)reader[0];
                    boardingEntity.locationId = (int)reader[1];
                    string format = "HH:mm";
                    boardingEntity.boardingDatetime = ((DateTime)reader[2]).ToString(format);
                    boardingEntity.customBoardingFlag = reader[3].ToString();
                    boardingEntity.earlyBoardingFlag = reader[4].ToString();
                    boardingEntities.Add(boardingEntity);
                }
                connection.Close();

                return boardingEntities;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<BoardingEntity> GetById(int id)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                string sql = "select bl.boarding_id, ll.location_id, bl.boarding_datetime from boarding_list bl inner join location_list ll ON bl.location_id = ll.location_id WHERE bl.boarding_id = @boardingId";

                // SQL Commands
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@boardingId", id);

                // 開啟資料庫連線
                await connection.OpenAsync();
                BoardingEntity boardingEntity = new BoardingEntity();
                SqlDataReader reader = select.ExecuteReader();
                while (reader.Read())
                {
                    boardingEntity.boardingId = (int)reader[0];
                    string format = "HH:mm";
                    boardingEntity.boardingDatetime = ((DateTime)reader[2]).ToString(format);
                }
                connection.Close();

                return boardingEntity;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<bool> UpdateAsync(BoardingEntity boardingEntity)
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                DateTime stime = DateTime.Parse(boardingEntity.boardingDatetime);
                string format = "HH:mm:ss";
                string sql = "UPDATE boarding_list SET boarding_datetime = @boardingTime WHERE boarding_id = @boardingId";

                // SQL Command
                SqlCommand select = new SqlCommand(sql, connection);
                select.Parameters.AddWithValue("@boardingId", boardingEntity.boardingId);
                select.Parameters.Add("@boardingTime", SqlDbType.DateTime).Value = stime.ToString(format);

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
