using System;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface ManagerRepo
    {
        Task<ManagerEntity> GetByUsername(string username);
    }
}
