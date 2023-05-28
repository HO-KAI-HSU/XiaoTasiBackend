using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Repository;
using XiaoTasiBackend.Repository.Impl;

namespace XiaoTasiBackend.Service.Impl
{
    public class BoardingServiceImpl : BoardingService
    {
        BoardingRepo _boardingRepo;

        public BoardingServiceImpl()
        {
            _boardingRepo = new BoardingRepoImpl();
        }

        public async Task<bool> CreateBoarding(BoardingEntity boarding)
        {
            await _boardingRepo.CreateAsync(boarding);
            return true;
        }

        public async Task<bool> DeleteBoarding(int id)
        {
            await _boardingRepo.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<BoardingEntity>> GetBoardingAsync(int id)
        {
            List<BoardingEntity> list = await _boardingRepo.GetAll();

            IEnumerable<BoardingEntity> boardingEntity =
                from boarding in list
                where boarding.boardingId == id
                select boarding;

            return boardingEntity;
        }

        public async Task<List<BoardingEntity>> GetBoardingList(string customBoardingFlag = "false")
        {
            List<BoardingEntity> list = await _boardingRepo.GetAll();
            IEnumerable<BoardingEntity> boardingEntities;
            if (customBoardingFlag.Equals("false"))
            {
                boardingEntities =
                    from boarding in list
                    select new BoardingEntity
                    {
                        boardingId = boarding.boardingId,
                        locationId = boarding.locationId,
                        boardingDatetime = boarding.boardingDatetime,
                        customBoardingFlag = boarding.customBoardingFlag,
                        earlyBoardingFlag = boarding.earlyBoardingFlag
                    };
            }
            else
            {
                boardingEntities =
                    from boarding in list
                    where boarding.customBoardingFlag == customBoardingFlag
                    select new BoardingEntity
                    {
                        boardingId = boarding.boardingId,
                        locationId = boarding.locationId,
                        boardingDatetime = boarding.boardingDatetime,
                        customBoardingFlag = boarding.customBoardingFlag,
                        earlyBoardingFlag = boarding.earlyBoardingFlag
                    };
            }

            return boardingEntities.ToList();
        }

        public async Task<bool> UpdateBoarding(BoardingEntity boarding)
        {
            await _boardingRepo.UpdateAsync(boarding);
            return true;
        }
    }
}
