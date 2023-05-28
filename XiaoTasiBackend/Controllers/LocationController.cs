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
    public class LocationController : Controller
    {
        StorageHelper _storageHelper;
        LocationService _locationService;

        public LocationController()
        {
            _locationService = new LocationServiceImpl();
            _storageHelper = new StorageHelper();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetLocationList(int draw = 1, int start = 0, int length = 0)
        {
            List<LocationEntity> locationEntities = await _locationService.GetLocationList();

            PageControl<LocationEntity> pageControl = new PageControl<LocationEntity>();

            List<LocationEntity> llocationListNew = pageControl.pageControl((start + 1), length, locationEntities);

            // 整理回傳內容
            return Json(new ApiReturn<List<LocationEntity>>(1, draw, pageControl.size, pageControl.size, locationEntities));
        }

        [HttpPost]
        public async Task<ActionResult> GetLocationInfo(int locationId)
        {
            IEnumerable<LocationEntity> locationEntity = await _locationService.GetLocationAsync(locationId);

            return Json(new ApiResult<object>(locationEntity));
        }

        [HttpPost]
        public async Task<ActionResult> AddLocation(FormCollection formCollection)
        {
            try
            {
                var pic = System.Web.HttpContext.Current.Request.Files["locationPicFile"];
                string locationName = formCollection["locationName"];
                string locationAddress = formCollection["locationAddress"];

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
                    url = _storageHelper.UploadFile("xiaotasi", "location", picName, filebase);
                }

                var location = new LocationEntity
                {
                    locationName = locationName,
                    locationAddress = locationAddress,
                    locationPicPath = url ?? ""
                };

                await _locationService.CreateLocation(location);

                return Json(new ApiResult<string>("Add Success", "新增成功"));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateLocationInfo(FormCollection formCollection)
        {
            try
            {
                var pic = System.Web.HttpContext.Current.Request.Files["locationPicFile"];
                int locationId = Convert.ToInt16(formCollection["locationId"]);
                string locationName = formCollection["locationName"];
                string locationAddress = formCollection["locationAddress"];

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
                    url = _storageHelper.UploadFile("xiaotasi", "location", picName, filebase);
                }

                var location = new LocationEntity
                {
                    locationId = locationId,
                    locationName = locationName,
                    locationAddress = locationAddress,
                    locationPicPath = url ?? ""
                };

                await _locationService.UpdateLocation(location);

                return Json(new ApiResult<string>("Update Success", "更新成功"));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteLocation(int locationId)
        {
            try
            {
                await _locationService.DeleteLocation(locationId);


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
            return "location" + n;
        }
    }
}
