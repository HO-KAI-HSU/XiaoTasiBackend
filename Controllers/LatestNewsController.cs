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
    public class LatestNewsController : Controller
    {
        StorageHelper _storageHelper;
        LatestNewsService _latestNewsService;

        public LatestNewsController()
        {
            _storageHelper = new StorageHelper();
            _latestNewsService = new LatestNewsServiceImpl();
        }


        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetLatestNewsList(int draw = 1, int start = 0, int length = 0)
        {
            List<LatestNewsEntity> latestNewsEntities = await _latestNewsService.GetLatestNewsList();

            PageControl<LatestNewsEntity> pageControl = new PageControl<LatestNewsEntity>();

            List<LatestNewsEntity> latestNewsListNew = pageControl.pageControl((start + 1), length, latestNewsEntities);

            // 整理回傳內容
            return Json(new ApiReturn<List<LatestNewsEntity>>(1, draw, pageControl.size, pageControl.size, latestNewsEntities));
        }

        [HttpPost]
        public async Task<ActionResult> GetLatestNewsInfo(int latestNewsId)
        {
            IEnumerable<LatestNewsEntity> latestNewsEntity = await _latestNewsService.GetLatestNewsAsync(latestNewsId);

            return Json(new ApiResult<IEnumerable<LatestNewsEntity>>(latestNewsEntity));
        }

        [HttpPost]
        public async Task<ActionResult> AddLatestNews(FormCollection formCollection)
        {
            var pic = System.Web.HttpContext.Current.Request.Files["latestNewsPicFile"];
            string latestNewsTitle = formCollection["latestNewsTitle"];
            string latestNewsContent = formCollection["latestNewsContent"];
            string latestNewsStime = formCollection["latestNewsStime"];
            string latestNewsEtime = formCollection["latestNewsEtime"];

            // 圖片處理區塊
            HttpPostedFileBase filebase = new HttpPostedFileWrapper(pic);
            bool res = _storageHelper.IsImage(filebase);
            if (!res)
            {
                return Json(new ApiError(1015, "Error file format!", "檔案類型不符！"));
            }
            string picName = this.createPicName();
            string url = _storageHelper.UploadFile("xiaotasi", "lateNews", picName, filebase);


            var latestNewsEntity = new LatestNewsEntity
            {
                latestNewsTitle = latestNewsTitle ?? "",
                latestNewsContent = latestNewsContent ?? "",
                latestNewsPicPath = url ?? "",
                publishStime = latestNewsStime,
                publishEtime = latestNewsEtime,
            };

            await _latestNewsService.CreateLatestNews(latestNewsEntity);

            return Json(new ApiResult<string>("Add Success", "新增成功"));
        }

        [HttpPost]
        public async Task<ActionResult> UpdateLatestNewsInfo(FormCollection formCollection)
        {
            var pic = System.Web.HttpContext.Current.Request.Files["latestNewsPicFile"];
            int latestNewsId = Convert.ToInt16(formCollection["latestNewsId"]);
            string latestNewsTitle = formCollection["latestNewsTitle"];
            string latestNewsContent = formCollection["latestNewsContent"];
            string latestNewsStime = formCollection["latestNewsStime"];
            string latestNewsEtime = formCollection["latestNewsEtime"];

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
                url = _storageHelper.UploadFile("xiaotasi", "lateNews", picName, filebase);
            }

            var latestNewsEntity = new LatestNewsEntity
            {
                latestNewsId = latestNewsId,
                latestNewsTitle = latestNewsTitle ?? "",
                latestNewsContent = latestNewsContent ?? "",
                latestNewsPicPath = url ?? "",
                publishStime = latestNewsStime,
                publishEtime = latestNewsEtime,
            };

            await _latestNewsService.UpdateLatestNews(latestNewsEntity);

            return Json(new ApiResult<string>("Update Success", "更新成功"));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteLatestNews(int latestNewsId)
        {
            await _latestNewsService.DeleteLatestNews(latestNewsId);

            return Json(new ApiResult<string>("Delete Success", "刪除成功"));
        }

        // 產生圖片名稱
        private string createPicName()
        {
            string n = Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            return "lateNews" + n;
        }
    }
}
