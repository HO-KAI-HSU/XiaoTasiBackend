using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface LocationRepo
    {
        Task<bool> CreateAsync(LocationEntity locationEntity);

        Task<bool> UpdateAsync(LocationEntity locationEntity);

        Task<bool> DeleteAsync(int id);

        Task<LocationEntity> GetById(int id);

        Task<List<LocationEntity>> GetAll();
    }
}
