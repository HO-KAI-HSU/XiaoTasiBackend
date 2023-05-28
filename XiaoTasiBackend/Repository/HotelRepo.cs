using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface HotelRepo
    {
        Task<bool> CreateAsync(HotelEntity hotelEntity);

        Task<bool> UpdateAsync(HotelEntity hotelEntity);

        Task<bool> DeleteAsync(int id);

        Task<HotelEntity> GetById(int id);

        Task<List<HotelEntity>> GetAll();

        Task<bool> MultiAddAsync(string sql);
    }
}
