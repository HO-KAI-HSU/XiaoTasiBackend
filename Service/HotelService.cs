using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Service
{
    public interface HotelService
    {
        Task<bool> CreateHotel(HotelEntity hotelEntity);

        Task<bool> UpdateHotel(HotelEntity hotelEntity);

        Task<bool> DeleteHotel(int id);

        Task<IEnumerable<HotelEntity>> GetHotelAsync(int id);

        Task<List<HotelEntity>> GetHotelList();

        Task<bool> AddHotelFromExcel(List<HotelEntity> hotelEntity);
    }
}
