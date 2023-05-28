using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Repository;
using XiaoTasiBackend.Repository.Impl;

namespace XiaoTasiBackend.Service.Impl
{
    public class HotelServiceImpl : HotelService
    {
        HotelRepo _hotelRepo;

        public HotelServiceImpl()
        {
            _hotelRepo = new HotelRepoImpl();
        }

        public async Task<bool> CreateHotel(HotelEntity hotelEntity)
        {
            await _hotelRepo.CreateAsync(hotelEntity);
            return true;
        }

        public async Task<bool> DeleteHotel(int id)
        {
            await _hotelRepo.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<HotelEntity>> GetHotelAsync(int id)
        {
            List<HotelEntity> list = await _hotelRepo.GetAll();

            IEnumerable<HotelEntity> hotelEntity =
                from hotel in list
                where hotel.hotelId == id
                select hotel;

            return hotelEntity;
        }

        public async Task<List<HotelEntity>> GetHotelList()
        {
            List<HotelEntity> list = await _hotelRepo.GetAll();
            return list;
        }

        public async Task<bool> UpdateHotel(HotelEntity hotelEntity)
        {
            await _hotelRepo.UpdateAsync(hotelEntity);
            return true;
        }

        public async Task<bool> AddHotelFromExcel(List<HotelEntity> hotelDatas)
        {
            int lastCount = hotelDatas.Count - 1;
            int index = 0;
            string sql = "INSERT INTO hotel_list (hotel_name, hotel_city, hotel_area, hotel_address, hotel_content, hotel_pic_path) VALUES ";
            foreach (HotelEntity hotelData in hotelDatas)
            {
                string hotelName = hotelData.hotelName;
                string hotelCity = hotelData.hotelCity;
                string hotelArea = hotelData.hotelArea;
                string hotelAddress = hotelData.hotelAddress;
                string hotelContent = hotelData.hotelContent;
                if (index == lastCount)
                {
                    sql += "(N'" + hotelName + "', N'" + hotelCity + "', N'" + hotelArea + "', N'" + hotelAddress + "', N'" + hotelContent + "', '" + "" + "')";
                }
                else
                {
                    sql += "(N'" + hotelName + "', N'" + hotelCity + "', N'" + hotelArea + "', N'" + hotelAddress + "', N'" + hotelContent + "', '" + "" + "'),";
                }
                index++;
            }

            await _hotelRepo.MultiAddAsync(sql);

            return true;
        }
    }
}
