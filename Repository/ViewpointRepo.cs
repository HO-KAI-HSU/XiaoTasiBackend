using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface ViewpointRepo
    {
        Task<bool> CreateAsync(ViewpointEntity viewpointEntity);

        Task<bool> UpdateAsync(ViewpointEntity viewpointEntity);

        Task<bool> DeleteAsync(int id);

        Task<ViewpointEntity> GetById(int id);

        Task<List<ViewpointEntity>> GetAll();

        Task<bool> MultiAddAsync(string sql);
    }
}
