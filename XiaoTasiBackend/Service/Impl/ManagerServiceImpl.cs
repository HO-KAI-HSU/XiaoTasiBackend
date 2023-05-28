using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Repository;
using XiaoTasiBackend.Repository.Impl;

namespace XiaoTasiBackend.Service.Impl
{
    public class ManagerServiceImpl : ManagerService
    {
        ManagerRepo _managerRepo;

        public ManagerServiceImpl()
        {
            _managerRepo = new ManagerRepoImpl();
        }

        public async Task<ManagerEntity> GetManagerByUsername(string username)
        {
            return await _managerRepo.GetByUsername(username);
        }
    }
}
