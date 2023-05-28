using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Service
{
    public interface BoardingService
    {
        Task<bool> CreateBoarding(BoardingEntity boarding);

        Task<bool> UpdateBoarding(BoardingEntity boarding);

        Task<bool> DeleteBoarding(int id);

        Task<IEnumerable<BoardingEntity>> GetBoardingAsync(int id);

        Task<List<BoardingEntity>> GetBoardingList(string customBoardingFlag = "false");
    }
}
