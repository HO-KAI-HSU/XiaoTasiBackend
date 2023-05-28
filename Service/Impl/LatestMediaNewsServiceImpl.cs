using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Repository;
using XiaoTasiBackend.Repository.Impl;

namespace XiaoTasiBackend.Service.Impl
{
    public class LatestMediaNewsServiceImpl : LatestMediaNewsService
    {
        LatestMediaNewsRepo _latestMediaNewsRepo;

        public LatestMediaNewsServiceImpl()
        {
            _latestMediaNewsRepo = new LatestMediaNewsRepoImpl();
        }

        public async Task<bool> CreateLatestMediaNews(LatestMediaNewsEntity latestMediaNews)
        {
            await _latestMediaNewsRepo.CreateAsync(latestMediaNews);
            return true;
        }

        public async Task<bool> DeleteLatestMediaNews(int id)
        {
            await _latestMediaNewsRepo.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<LatestMediaNewsEntity>> GetLatestMediaNewsAsync(int id)
        {
            List<LatestMediaNewsEntity> list = await _latestMediaNewsRepo.GetAll();

            IEnumerable<LatestMediaNewsEntity> latestMediaNewsEntity =
                from latestMediaNews in list
                where latestMediaNews.latestMediaNewsId == id
                select latestMediaNews;

            return latestMediaNewsEntity;
        }

        public async Task<List<LatestMediaNewsEntity>> GetLatestMediaNewsList()
        {
            List<LatestMediaNewsEntity> latestMediaNewsEntities = await _latestMediaNewsRepo.GetAll();
            return latestMediaNewsEntities;
        }

        public async Task<bool> UpdateLatestMediaNews(LatestMediaNewsEntity latestMediaNews)
        {

            await _latestMediaNewsRepo.UpdateAsync(latestMediaNews);
            return true;
        }
    }
}
