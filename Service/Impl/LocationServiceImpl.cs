using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Repository;
using XiaoTasiBackend.Repository.Impl;

namespace XiaoTasiBackend.Service.Impl
{
    public class LocationServiceImpl : LocationService
    {
        LocationRepo _locationRepo;

        public LocationServiceImpl()
        {
            _locationRepo = new LocationRepoImpl();
        }

        public async Task<bool> CreateLocation(LocationEntity location)
        {
            await _locationRepo.CreateAsync(location);
            return true;
        }

        public async Task<bool> DeleteLocation(int id)
        {
            await _locationRepo.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<LocationEntity>> GetLocationAsync(int id)
        {
            List<LocationEntity> list = await _locationRepo.GetAll();

            IEnumerable<LocationEntity> locationEntity =
                from location in list
                where location.locationId == id
                select location;

            return locationEntity;
        }

        public async Task<List<LocationEntity>> GetLocationList()
        {
            List<LocationEntity> list = await _locationRepo.GetAll();
            return list;
        }

        public async Task<bool> UpdateLocation(LocationEntity location)
        {
            await _locationRepo.UpdateAsync(location);
            return true;
        }
    }
}
