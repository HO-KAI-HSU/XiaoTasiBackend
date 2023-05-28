using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Dto;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Repository;
using XiaoTasiBackend.Repository.Impl;

namespace XiaoTasiBackend.Service.Impl
{
    public class ReservationServiceImpl : ReservationService
    {
        private SeatTravelMatchRepo _seatTravelMatchRepo;
        private MemberReservationRepo _memberReservationRepo;
        private ReservationRepo _reservationRepo;
        private TravelStepRepo _travelStepRepo;
        private ReservationCheckRepo _reservationCheckRepo;
        private MemberRepo _memberRepo;

        public ReservationServiceImpl()
        {
            this._seatTravelMatchRepo = new SeatTravelMatchRepoImpl();
            this._memberReservationRepo = new MemberReservationRepoImpl();
            this._reservationRepo = new ReservationRepoImpl();
            this._travelStepRepo = new TravelStepRepoImpl();
            this._reservationCheckRepo = new ReservationCheckRepoImpl();
            this._memberRepo = new MemberRepoImpl();
        }

        public async Task<bool> cancelReservationBySystem(int seatId, int travelStepId)
        {
            // step1 : Get MemberReservationInfo（get ReservationCode）
            MemberReservationEntity memberReservationEntity = await getMemberReservationByIds(seatId, travelStepId);
            string reservationCode = memberReservationEntity.reservationCode;

            // step2 : Get ResrvationInfo(1位)
            ReservationEntity reservationEntity = await _reservationRepo.GetByCode(reservationCode);
            int reservationId = reservationEntity.reservationId;
            string oldCode = reservationEntity.reservationCode;

            // step3 : Get TravelStepInfo
            TravelStepEntity travelStepEntity = await _travelStepRepo.GetById(travelStepId);
            int travelCost = travelStepEntity.travelCost;

            // step4 : Get All MemberReservationInfo By ReservationCode
            List<MemberReservationEntity> memberReservationEntities = await _memberReservationRepo.GetByCode(reservationCode);
            List<MemberReservationEntity> addMemberReservationEntities = new List<MemberReservationEntity>();
            ReservationEntity newReservationEntity = new ReservationEntity();
            string newCode = createOrderNumber();
            string newSeatIds = "";

            // step5 arrange memberReservation (readd memberReservationList) 
            foreach (MemberReservationEntity memberReservation in memberReservationEntities)
            {
                if (!memberReservation.seatId.Equals(memberReservationEntity.seatId))
                {
                    newSeatIds += memberReservation.seatId.ToString() + ",";
                    memberReservation.reservationCode = newCode;
                    addMemberReservationEntities.Add(memberReservation);
                }
            }

            // step6 MemberReservation seat count > 1, create New Resrvation and del old Resrvation
            if (memberReservationEntities.Count > 1)
            {
                int remainCount = memberReservationEntities.Count - 1;
                int cost = travelCost * remainCount;
                newReservationEntity.memberCode = reservationEntity.memberCode;
                newReservationEntity.note = reservationEntity.note;
                newReservationEntity.travelId = reservationEntity.travelId;
                newReservationEntity.travelStepId = reservationEntity.travelStepId;
                newReservationEntity.reservationCode = newCode;
                newReservationEntity.reservationNum = remainCount;
                newReservationEntity.reservationCost = cost;
                newReservationEntity.seatIds = newSeatIds;
                await _reservationRepo.CreateAsync(newReservationEntity);
                foreach (MemberReservationEntity addMemberReservation in addMemberReservationEntities)
                {
                    await _memberReservationRepo.CreateAsync(addMemberReservation);
                }
            }
            else if (memberReservationEntities.Count == 0)
            {
                return true;
            }

            // Del Reservation, MemberReservation and SeatTravelMatch
            await _memberReservationRepo.DeleteReservationBindAsync(oldCode);
            await _reservationRepo.DeleteBindAsync(reservationId);
            await _seatTravelMatchRepo.DelSeatTravelBindAsync(seatId, travelStepId);

            return true;
        }

        public async Task<List<ReservationDto>> GetReservationList()
        {
            try
            {
                var reservations = (await _reservationRepo.GetAll()).AsEnumerable();
                var members = (await _memberRepo.GetAll()).AsEnumerable();
                var reservationChecks = (await _reservationCheckRepo.GetAll()).AsEnumerable();

                var reservationDtos =
                    from r in reservations
                    join rc in reservationChecks
                       on new { r.reservationCode, r.memberCode } equals new { rc.reservationCode, rc.memberCode }
                    join m in members
                       on r.memberCode equals m.memberCode
                    select new ReservationDto
                    {
                        reservationId = r.reservationId,
                        reservationCode = r.reservationCode,
                        memberCode = r.memberCode,
                        travelId = r.travelId,
                        reservationNum = r.reservationNum,
                        reservationCost = r.reservationCost,
                        travelStepId = r.travelStepId,
                        seatIds = r.seatIds,
                        status = rc.status,
                        reservationCheckPicPath = rc.reservationCheckPicPath,
                        memberName = m.name,
                    };

                return reservationDtos.ToList();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public async Task<List<ReservationEntity>> GetReservations()
        {
            List<ReservationEntity> list = await _reservationRepo.GetAll();

            return list;
        }

        public async Task<IEnumerable<ReservationEntity>> GetReservationAsync(int id)
        {
            List<ReservationEntity> list = await _reservationRepo.GetAll();

            IEnumerable<ReservationEntity> reservationEntity =
                from reservation in list
                where reservation.reservationId == id
                select reservation;

            return reservationEntity;
        }

        public async Task<List<ReservationCheckEntity>> GetReservationChecks()
        {
            List<ReservationCheckEntity> list = await _reservationCheckRepo.GetAll();

            return list;
        }

        public async Task<IEnumerable<ReservationCheckEntity>> GetReservationCheckAsync(int id)
        {
            List<ReservationCheckEntity> list = await _reservationCheckRepo.GetAll();

            IEnumerable<ReservationCheckEntity> reservationCheckEntity =
                from reservationCheck in list
                where reservationCheck.reservationCheckId == id
                select reservationCheck;

            return reservationCheckEntity;
        }

        //產生訂單編號
        private string createOrderNumber()
        {
            string n = DateTime.Now.ToString("yyyyMMddHHmmss");
            return "OR" + n;
        }

        private async Task<MemberReservationEntity> getMemberReservationByIds(int seatId, int travelStepId)
        {
            string ids = 0 + ":" + seatId + ":" + travelStepId;

            // step1 : Get MemberReservationInfo（ReservationCode）
            MemberReservationEntity memberReservationEntity = await _memberReservationRepo.GetByIds(ids, 2);

            return memberReservationEntity;
        }
    }
}
