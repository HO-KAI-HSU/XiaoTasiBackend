using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Dto;
using XiaoTasiBackend.Models.Entity;

namespace XiaoTasiBackend.Service
{
    public interface ReservationService
    {
        Task<bool> cancelReservationBySystem(int seatId, int travelStepId);

        Task<List<ReservationDto>> GetReservationList();

        Task<IEnumerable<ReservationCheckEntity>> GetReservationCheckAsync(int id);

        Task<List<ReservationCheckEntity>> GetReservationChecks();

        Task<List<ReservationEntity>> GetReservations();

        Task<IEnumerable<ReservationEntity>> GetReservationAsync(int id);
    }
}
