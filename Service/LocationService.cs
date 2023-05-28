using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Service
{
    public interface LocationService
    {
        Task<bool> CreateLocation(LocationEntity location);

        Task<bool> UpdateLocation(LocationEntity location);

        Task<bool> DeleteLocation(int id);

        Task<IEnumerable<LocationEntity>> GetLocationAsync(int id);

        Task<List<LocationEntity>> GetLocationList();
    }
}
