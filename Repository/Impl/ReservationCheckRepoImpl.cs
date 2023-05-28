using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class ReservationCheckRepoImpl : ReservationCheckRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public ReservationCheckRepoImpl()
        {
        }

        public async Task<List<ReservationCheckEntity>> GetAll()
        {
            try
            {
                SqlConnection connection = new SqlConnection(this.sql_DB);
                // SQL Command
                string fieldSql = "select reservation_code as reservationCode, member_code as memberCode, bankbook_account_code as bankbookAccountCode, reservation_check_pic_path as reservationCheckPicPath, status from reservation_check_list";
                SqlCommand select = new SqlCommand(fieldSql, connection);
                // 開啟資料庫連線
                await connection.OpenAsync();
                SqlDataReader reader = select.ExecuteReader();
                List<ReservationCheckEntity> reservationCheckEntities = new List<ReservationCheckEntity>();
                while (reader.Read())
                {
                    ReservationCheckEntity reservationCheckEntity = new ReservationCheckEntity();
                    reservationCheckEntity.reservationCode = (string)reader[0];
                    reservationCheckEntity.memberCode = (string)reader[1];
                    reservationCheckEntity.bankbookAccountCode = (string)reader[2];
                    reservationCheckEntity.reservationCheckPicPath = (string)reader[3];
                    reservationCheckEntity.status = (int)reader[4];
                    reservationCheckEntities.Add(reservationCheckEntity);
                }
                connection.Close();

                return reservationCheckEntities;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Task<ReservationCheckEntity> GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
