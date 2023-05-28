using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface SeatRepo
    {

        int getSeatIdByTransportationIdAndPos(int transportationId, int seatPos);

        SeatEntity GetById(int id);

        SeatEntity GetByCode(string code);

        Task<List<SeatEntity>> GetAll();

        Task<bool> CreateAsync(SeatEntity seatEntity);

        Task<bool> UpdateAsync(SeatEntity seatEntity);

        Task<bool> DeleteAsync(int id);
    }
}
