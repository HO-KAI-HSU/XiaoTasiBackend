using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface ReservationRepo
    {
        Task<bool> CreateAsync(ReservationEntity transportationEntity);

        Task<bool> UpdateAsync(ReservationEntity transportationEntity);

        Task<bool> DeleteAsync(int id);

        Task<bool> DeleteBindAsync(int id);

        Task<ReservationEntity> GetByCode(string code);

        ReservationEntity GetById(int id);

        Task<List<ReservationEntity>> GetAll();
    }
}
