using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface LatestMediaNewsRepo
    {
        Task<bool> CreateAsync(LatestMediaNewsEntity latestMediaNewsEntity);

        Task<bool> UpdateAsync(LatestMediaNewsEntity latestMediaNewsEntity);

        Task<bool> DeleteAsync(int id);

        Task<LatestMediaNewsEntity> GetById(int id);

        Task<List<LatestMediaNewsEntity>> GetAll();
    }
}
