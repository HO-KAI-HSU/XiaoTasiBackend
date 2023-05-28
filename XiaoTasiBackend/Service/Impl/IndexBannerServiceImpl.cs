using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Repository;
using XiaoTasiBackend.Repository.Impl;

namespace XiaoTasiBackend.Service.Impl
{
    public class IndexBannerServiceImpl : IndexBannerService
    {
        IndexBannerRepo _indexBannerRepo;

        public IndexBannerServiceImpl()
        {
            _indexBannerRepo = new IndexBannerRepoImpl();
        }

        public async Task<bool> CreateIndexBanner(IndexBannerEntity indexBanner)
        {
            await _indexBannerRepo.CreateAsync(indexBanner);
            return true;
        }

        public async Task<bool> DeleteIndexBanner(int id)
        {
            await _indexBannerRepo.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<IndexBannerEntity>> GetIndexBannerAsync(int id)
        {
            List<IndexBannerEntity> list = await _indexBannerRepo.GetAll();

            IEnumerable<IndexBannerEntity> indexBannerEntity =
                from indexBanner in list
                where indexBanner.indexBannerId == id
                select indexBanner;

            return indexBannerEntity;
        }

        public async Task<List<IndexBannerEntity>> GetIndexBannerList()
        {
            List<IndexBannerEntity> indexBannerEntities = await _indexBannerRepo.GetAll();
            return indexBannerEntities;
        }

        public async Task<bool> UpdateIndexBanner(IndexBannerEntity indexBanner)
        {
            await _indexBannerRepo.UpdateAsync(indexBanner);
            return true;
        }
    }
}
