using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface LatestNewsRepo
    {
        Task<bool> CreateAsync(LatestNewsEntity latestNewsEntity);

        Task<bool> UpdateAsync(LatestNewsEntity latestNewsEntity);

        Task<bool> DeleteAsync(int id);

        Task<LatestNewsEntity> GetById(int id);

        Task<List<LatestNewsEntity>> GetAll();
    }
}
