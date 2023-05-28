using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XiaoTasiBackend.Helpers;
using XiaoTasiBackend.Models;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Service;
using XiaoTasiBackend.Service.Impl;

namespace XiaoTasiBackend.Controllers
{
    public class HotelController : Controller
    {
        StorageHelper _storageHelper;
        HotelService _hotelService;

        public HotelController()
        {
            _storageHelper = new StorageHelper();
            _hotelService = new HotelServiceImpl();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetHotelList(int draw = 1, int start = 0, int length = 0)
        {
            List<HotelEntity> hotels = await _hotelService.GetHotelList();

            PageControl<HotelEntity> pageControl = new PageControl<HotelEntity>();

            List<HotelEntity> hotelListNew = pageControl.pageControl((start + 1), length, hotels);

            // 整理回傳內容
            return Json(new ApiReturn<List<HotelEntity>>(1, draw, pageControl.size, pageControl.size, hotels));
        }

        [HttpPost]
        public async Task<ActionResult> GetHotelInfo(int hotelId)
        {
            IEnumerable<HotelEntity> hotelEntity = await _hotelService.GetHotelAsync(hotelId);

            return Json(new ApiResult<object>(hotelEntity));
        }

        [HttpPost]
        public async Task<ActionResult> AddHotel(FormCollection formCollection)
        {
            try
            {
                var pic = System.Web.HttpContext.Current.Request.Files["hotelPicFile"];
                string hotelName = formCollection["hotelName"];
                string hotelCity = formCollection["hotelCity"];
                string hotelArea = formCollection["hotelArea"];
                string hotelAddress = formCollection["hotelAddress"];
                string hotelContent = formCollection["hotelContent"];

                // 圖片處理區塊
                string url = "";
                if (pic != null)
                {
                    HttpPostedFileBase filebase = new HttpPostedFileWrapper(pic);
                    bool res = _storageHelper.IsImage(filebase);
                    if (!res)
                    {
                        return Json(new ApiError(1014, "Token Expired!", "登入時效已過，請重新登入！"));
                    }
                    string picName = this.createPicName();
                    url = _storageHelper.UploadFile("xiaotasi", "hotel", picName, filebase);
                }

                var hotel = new HotelEntity
                {
                    hotelName = hotelName ?? "",
                    hotelContent = hotelContent ?? "",
                    hotelCity = hotelCity ?? "",
                    hotelArea = hotelArea ?? "",
                    hotelAddress = hotelAddress ?? "",
                    hotelPicPath = url ?? ""
                };

                await _hotelService.CreateHotel(hotel);

                return Json(new ApiResult<string>("Add Success", "新增成功"));

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateHotelInfo(FormCollection formCollection)
        {
            try
            {
                var pic = System.Web.HttpContext.Current.Request.Files["hotelPicFile"];
                int hotelId = Convert.ToInt16(formCollection["hotelId"]);
                string hotelName = formCollection["hotelName"];
                string hotelCity = formCollection["hotelCity"];
                string hotelArea = formCollection["hotelArea"];
                string hotelAddress = formCollection["hotelAddress"];
                string hotelContent = formCollection["hotelContent"];

                // 圖片處理區塊
                string url = "";
                if (pic != null)
                {
                    HttpPostedFileBase filebase = new HttpPostedFileWrapper(pic);
                    bool res = _storageHelper.IsImage(filebase);
                    if (!res)
                    {
                        return Json(new ApiError(1015, "Error file format!", "檔案類型不符！"));
                    }
                    string picName = this.createPicName();
                    url = _storageHelper.UploadFile("xiaotasi", "hotel", picName, filebase);
                }

                var hotel = new HotelEntity
                {
                    hotelId = hotelId,
                    hotelName = hotelName ?? "",
                    hotelContent = hotelContent ?? "",
                    hotelCity = hotelCity ?? "",
                    hotelArea = hotelArea ?? "",
                    hotelAddress = hotelAddress ?? "",
                    hotelPicPath = url ?? ""
                };

                await _hotelService.UpdateHotel(hotel);

                return Json(new ApiResult<string>("Update Success", "更新成功"));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteHotel(int hotelId)
        {
            try
            {
                await _hotelService.DeleteHotel(hotelId);

                return Json(new ApiResult<string>("Delete Success", "刪除成功"));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        // 產生圖片名稱
        private string createPicName()
        {
            string n = Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            return "hotel" + n;
        }
    }
}
