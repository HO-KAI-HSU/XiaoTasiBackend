using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Repository;
using XiaoTasiBackend.Repository.Impl;

namespace XiaoTasiBackend.Service.Impl
{
    public class LatestNewsServiceImpl : LatestNewsService
    {
        LatestNewsRepo _latestNewsRepo;

        public LatestNewsServiceImpl()
        {
            _latestNewsRepo = new LatestNewsRepoImpl();
        }

        public async Task<bool> CreateLatestNews(LatestNewsEntity latestNews)
        {
            await _latestNewsRepo.CreateAsync(latestNews);
            return true;
        }

        public async Task<bool> DeleteLatestNews(int id)
        {
            await _latestNewsRepo.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<LatestNewsEntity>> GetLatestNewsAsync(int id)
        {
            List<LatestNewsEntity> list = await _latestNewsRepo.GetAll();

            IEnumerable<LatestNewsEntity> latestNewsEntity =
                from latestNews in list
                where latestNews.latestNewsId == id
                select latestNews;

            return latestNewsEntity;
        }

        public async Task<List<LatestNewsEntity>> GetLatestNewsList()
        {
            List<LatestNewsEntity> list = await _latestNewsRepo.GetAll();

            return list;
        }

        public async Task<bool> UpdateLatestNews(LatestNewsEntity latestNews)
        {
            await _latestNewsRepo.UpdateAsync(latestNews);
            return true;
        }
    }
}
