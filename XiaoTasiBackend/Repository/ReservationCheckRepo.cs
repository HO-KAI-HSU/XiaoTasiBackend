using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface ReservationCheckRepo
    {
        Task<ReservationCheckEntity> GetById(int id);

        Task<List<ReservationCheckEntity>> GetAll();
    }
}
