using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Repository
{
    public interface TransportationRepo
    {
        TransportationEntity GetById(int id);

        TransportationEntity GetByCode(string code);

        Task<List<TransportationEntity>> GetAll();

        Task<bool> CreateAsync(TransportationEntity transportationEntity);

        Task<bool> UpdateAsync(TransportationEntity transportationEntity);

        Task<bool> DeleteAsync(int id);
    }
}
