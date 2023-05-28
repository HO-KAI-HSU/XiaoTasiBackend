using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using travelManagement.Models;
using travelManagement.Models.Entity;
using travelManagement.Service;
using travelManagement.Service.Impl;

namespace travelManagement.Controllers
{
    public class BoardingController : Controller
    {
        BoardingService _boardingService;

        public BoardingController()
        {
            _boardingService = new BoardingServiceImpl();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetBoardingList(string customBoardingFlag = "false", int draw = 1, int start = 0, int length = 0)
        {
            List<BoardingEntity> boardingEntities = await _boardingService.GetBoardingList(customBoardingFlag);

            PageControl<BoardingEntity> pageControl = new PageControl<BoardingEntity>();

            List<BoardingEntity> boardingListNew = pageControl.pageControl((start + 1), length, boardingEntities);

            // 整理回傳內容
            return Json(new ApiReturn<List<BoardingEntity>>(1, draw, pageControl.size, pageControl.size, boardingEntities));
        }

        [HttpPost]
        public async Task<ActionResult> GetBoardingInfo(int boardingId)
        {
            IEnumerable<BoardingEntity> boardingEntity = await _boardingService.GetBoardingAsync(boardingId);

            return Json(new ApiResult<object>(boardingEntity));
        }

        [HttpPost]
        public async Task<ActionResult> AddBoarding(int locationId, string boardingDatetime, string customBoardingFlag = "0", string earlyBoardingFlag = "-1")
        {
            try
            {
                var boarding = new BoardingEntity
                {
                    locationId = locationId,
                    boardingDatetime = boardingDatetime,
                    customBoardingFlag = customBoardingFlag,
                    earlyBoardingFlag = earlyBoardingFlag
                };

                await _boardingService.CreateBoarding(boarding);
                return Json(new ApiResult<string>("Add Success", "新增成功"));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateBoardingInfo(int boardingId, string boardingDatetime)
        {
            try
            {
                var boarding = new BoardingEntity
                {
                    boardingId = boardingId,
                    boardingDatetime = boardingDatetime
                };

                await _boardingService.UpdateBoarding(boarding);
                return Json(new ApiResult<string>("Update Success", "更新成功"));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteBoarding(int boardingId)
        {
            try
            {
                await _boardingService.DeleteBoarding(boardingId);
                return Json(new ApiResult<string>("Delete Success", "刪除成功"));
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
