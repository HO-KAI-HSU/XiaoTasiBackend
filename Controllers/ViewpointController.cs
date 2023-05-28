using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using travelManagement.Helpers;
using travelManagement.Models;
using travelManagement.Models.Entity;
using travelManagement.Service;
using travelManagement.Service.Impl;

namespace travelManagement.Controllers
{
    public class ViewpointController : Controller
    {

        StorageHelper _storageHelper;
        ViewpointService _viewpointService;

        public ViewpointController()
        {
            _storageHelper = new StorageHelper();
            _viewpointService = new ViewpointServiceImpl();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetViewpointList(int draw = 1, int start = 0, int length = 0)
        {
            var viewpoints = await _viewpointService.GetViewpointList();

            PageControl<ViewpointEntity> pageControl = new PageControl<ViewpointEntity>();

            List<ViewpointEntity> viewpointListNew = pageControl.pageControl((start + 1), length, viewpoints);

            // 整理回傳內容
            return Json(new ApiReturn<List<ViewpointEntity>>(1, draw, pageControl.size, pageControl.size, viewpoints));
        }

        [HttpPost]
        public async Task<ActionResult> GetViewpointInfo(int viewpointId)
        {
            var viewpoint = await _viewpointService.GetViewpointAsync(viewpointId);

            return Json(new ApiResult<object>(viewpoint));
        }

        [HttpPost]
        public async Task<ActionResult> AddViewpoint(FormCollection formCollection)
        {
            try
            {
                var pic = System.Web.HttpContext.Current.Request.Files["viewpointPicFile"];
                string viewpointTitle = formCollection["viewpointTitle"];
                string viewpointCity = formCollection["viewpointCity"];
                string viewpointArea = formCollection["viewpointArea"];
                string viewpointAddress = formCollection["viewpointAddress"];
                string viewpointContent = formCollection["viewpointContent"];

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

                var viewpoint = new ViewpointEntity
                {
                    viewpointTitle = viewpointTitle ?? "",
                    viewpointContent = viewpointContent ?? "",
                    viewpointCity = viewpointCity ?? "",
                    viewpointArea = viewpointArea ?? "",
                    viewpointAddress = viewpointAddress ?? "",
                    viewpointPicPath = url ?? ""
                };

                await _viewpointService.CreateViewpoint(viewpoint);

                return Json(new ApiResult<string>("Add Success", "新增成功"));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateViewpointInfo(FormCollection formCollection)
        {
            try
            {
                var pic = System.Web.HttpContext.Current.Request.Files["viewpointPicFile"];
                int viewpointId = Convert.ToInt16(formCollection["viewpointId"]);
                string viewpointTitle = formCollection["viewpointTitle"];
                string viewpointCity = formCollection["viewpointCity"];
                string viewpointArea = formCollection["viewpointArea"];
                string viewpointAddress = formCollection["viewpointAddress"];
                string viewpointContent = formCollection["viewpointContent"];

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
                    url = _storageHelper.UploadFile("xiaotasi", "viewpoint", picName, filebase);
                }

                var viewpoint = new ViewpointEntity
                {
                    viewpointId = viewpointId,
                    viewpointTitle = viewpointTitle ?? "",
                    viewpointContent = viewpointContent ?? "",
                    viewpointCity = viewpointCity ?? "",
                    viewpointArea = viewpointArea ?? "",
                    viewpointAddress = viewpointAddress ?? "",
                    viewpointPicPath = url ?? ""
                };

                await _viewpointService.UpdateViewpoint(viewpoint);

                return Json(new ApiResult<string>("Update Success", "更新成功"));
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        [HttpPost]
        public async Task<ActionResult> DeleteViewpoint(int viewpointId)
        {
            try
            {
                await _viewpointService.DeleteViewpoint(viewpointId);

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
            return "viewpoint" + n;
        }
    }
}
