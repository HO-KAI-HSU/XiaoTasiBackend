using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository.Impl
{
    public class TravelStepRepoImpl : TravelStepRepo
    {
        private string sql_DB = WebConfigurationManager.ConnectionStrings["ConnDB"].ConnectionString;

        public TravelStepRepoImpl()
        {
        }


        public async Task<TravelStepEntity> GetById(int id)
        {
            // SQL Command
            SqlConnection connection = new SqlConnection(this.sql_DB);
            SqlCommand select = new SqlCommand("select travel_id as travelId, travel_step_id as travelStepId, travel_s_time as travelStime, travel_e_time as travelEtime, travel_step_code as travelStepCode, travel_cost as travelCost from travel_step_list where travel_step_id = @travelStepId and status = 1", connection);
            select.Parameters.AddWithValue("@travelStepId", id);

            // 開啟資料庫連線
            await connection.OpenAsync();
            SqlDataReader reader = select.ExecuteReader();

            // 旅遊編號陣列
            TravelStepEntity travelStepEntity = new TravelStepEntity();
            while (reader.Read())
            {
                if (reader[0] != null)
                {
                    string format = "yyyy-MM-dd";
                    int travelStepId = (int)reader[1];
                    string stime = ((DateTime)reader[2]).ToString(format);
                    string etime = ((DateTime)reader[3]).ToString(format);
                    string travelStepCode = (string)reader[4];
                    travelStepEntity.travelStepId = travelStepId;
                    travelStepEntity.travelStime = stime;
                    travelStepEntity.travelEtime = etime;
                    travelStepEntity.travelStepCode = travelStepCode;
                    travelStepEntity.travelCost = (int)reader[5];
                }
            }
            connection.Close();
            return travelStepEntity;
        }
    }
}
