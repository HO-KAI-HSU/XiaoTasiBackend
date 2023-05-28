using System;
namespace XiaoTasiBackend.Service
{
    public interface SeatService
    {
        int GetSeatIdByTransportationIdAndPos(int transportationId, int seatPos);
    }
}
