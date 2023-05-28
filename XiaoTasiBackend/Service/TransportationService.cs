using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Service
{
    public interface TransportationService
    {
        Task<IEnumerable<TransportationEntity>> GetTransportationAsync(int id);

        Task<IEnumerable<TransportationEntity>> GetTransportationByCode(string code);

        Task<List<TransportationEntity>> GetTransportationList();

        Task<bool> CreateTransportation(TransportationEntity transportation);

        Task<bool> UpdateTransportation(TransportationEntity transportation);

        Task<bool> DeleteTransportation(int id);
    }
}
