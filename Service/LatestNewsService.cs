using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Service
{
    public interface LatestNewsService
    {
        Task<bool> CreateLatestNews(LatestNewsEntity latestNews);

        Task<bool> UpdateLatestNews(LatestNewsEntity latestNews);

        Task<bool> DeleteLatestNews(int id);

        Task<IEnumerable<LatestNewsEntity>> GetLatestNewsAsync(int id);

        Task<List<LatestNewsEntity>> GetLatestNewsList();
    }
}
