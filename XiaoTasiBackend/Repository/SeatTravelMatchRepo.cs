using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface SeatTravelMatchRepo
    {
        Task<bool> DelSeatTravelBindAsync(int seatId, int travelStepId);

        SeatTravelMatchEntity GetById(int id);

        SeatTravelMatchEntity GetByCode(string code);

        Task<List<SeatTravelMatchEntity>> GetAll();

        Task<bool> CreateAsync(SeatTravelMatchEntity seatEntity);

        Task<bool> UpdateAsync(SeatTravelMatchEntity seatEntity);

        Task<bool> DeleteAsync(int id);
    }
}
