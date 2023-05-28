using System;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface TravelStepRepo
    {
        Task<TravelStepEntity> GetById(int id);
    }
}
