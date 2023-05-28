using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface BoardingRepo
    {
        Task<bool> CreateAsync(BoardingEntity boardingEntity);

        Task<bool> UpdateAsync(BoardingEntity boardingEntity);

        Task<bool> DeleteAsync(int id);

        Task<BoardingEntity> GetById(int id);

        Task<List<BoardingEntity>> GetAll();
    }
}
