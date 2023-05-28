using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Service
{
    public interface LatestMediaNewsService
    {
        Task<bool> CreateLatestMediaNews(LatestMediaNewsEntity latestMediaNews);

        Task<bool> UpdateLatestMediaNews(LatestMediaNewsEntity latestMediaNews);

        Task<bool> DeleteLatestMediaNews(int id);

        Task<IEnumerable<LatestMediaNewsEntity>> GetLatestMediaNewsAsync(int id);

        Task<List<LatestMediaNewsEntity>> GetLatestMediaNewsList();
    }
}
