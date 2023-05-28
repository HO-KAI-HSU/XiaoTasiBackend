using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Repository;
using XiaoTasiBackend.Repository.Impl;

namespace XiaoTasiBackend.Service.Impl
{
    public class TransportationServiceImpl : TransportationService
    {
        private TransportationRepo _transportationRepo;

        public TransportationServiceImpl()
        {
            _transportationRepo = new TransportationRepoImpl();
        }

        public async Task<IEnumerable<TransportationEntity>> GetTransportationAsync(int id)
        {
            List<TransportationEntity> list = await _transportationRepo.GetAll();

            IEnumerable<TransportationEntity> transportationEntity =
                from transportation in list
                where transportation.transportationId == id
                select transportation;

            return transportationEntity;
        }

        public async Task<IEnumerable<TransportationEntity>> GetTransportationByCode(string code)
        {
            List<TransportationEntity> list = await _transportationRepo.GetAll();

            IEnumerable<TransportationEntity> transportationEntity =
                from transportation in list
                where transportation.transportationLicensesNumber == code
                select transportation;

            return transportationEntity;
        }

        public async Task<List<TransportationEntity>> GetTransportationList()
        {
            return await _transportationRepo.GetAll();
        }

        public async Task<bool> CreateTransportation(TransportationEntity transportation)
        {
            return await _transportationRepo.CreateAsync(transportation);
        }

        public async Task<bool> UpdateTransportation(TransportationEntity transportation)
        {
            return await _transportationRepo.UpdateAsync(transportation);
        }

        public async Task<bool> DeleteTransportation(int id)
        {
            return await _transportationRepo.DeleteAsync(id);
        }
    }
}
