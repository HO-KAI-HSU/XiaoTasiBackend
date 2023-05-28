using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface IndexBannerRepo
    {
        Task<bool> CreateAsync(IndexBannerEntity indexBannerEntity);

        Task<bool> UpdateAsync(IndexBannerEntity indexBannerEntity);

        Task<bool> DeleteAsync(int id);

        Task<IndexBannerEntity> GetById(int id);

        Task<List<IndexBannerEntity>> GetAll();
    }
}
