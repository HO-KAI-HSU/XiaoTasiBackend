using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Service
{
    public interface IndexBannerService
    {
        Task<bool> CreateIndexBanner(IndexBannerEntity indexBanner);

        Task<bool> UpdateIndexBanner(IndexBannerEntity indexBanner);

        Task<bool> DeleteIndexBanner(int id);

        Task<IEnumerable<IndexBannerEntity>> GetIndexBannerAsync(int id);

        Task<List<IndexBannerEntity>> GetIndexBannerList();
    }
}
