using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Service
{
    public interface ManagerService
    {
        Task<ManagerEntity> GetManagerByUsername(string username);
    }
}
