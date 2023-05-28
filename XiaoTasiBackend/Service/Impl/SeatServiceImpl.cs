using System;
using XiaoTasiBackend.Repository;
using XiaoTasiBackend.Repository.Impl;

namespace XiaoTasiBackend.Service.Impl
{
    public class SeatServiceImpl : SeatService
    {
        private SeatRepo _seatRepo;

        public SeatServiceImpl()
        {
            this._seatRepo = new SeatRepoImpl();
        }

        public int GetSeatIdByTransportationIdAndPos(int transportationId, int seatPos)
        {
            int seatId = _seatRepo.getSeatIdByTransportationIdAndPos(transportationId, seatPos);

            return seatId;
        }
    }
}
