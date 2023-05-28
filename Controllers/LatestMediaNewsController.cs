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
    public class LatestMediaNewsController : Controller
    {
        StorageHelper _storageHelper;
        LatestMediaNewsService _latestMediaNewsService;

        public LatestMediaNewsController()
        {
            _storageHelper = new StorageHelper();
            _latestMediaNewsService = new LatestMediaNewsServiceImpl();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetLatestMediaNewsList(int draw = 1, int start = 0, int length = 0)
        {
            List<LatestMediaNewsEntity> latestMediaNewsEntities = await _latestMediaNewsService.GetLatestMediaNewsList();

            PageControl<LatestMediaNewsEntity> pageControl = new PageControl<LatestMediaNewsEntity>();

            List<LatestMediaNewsEntity> latestMediaNewsListNew = pageControl.pageControl((start + 1), length, latestMediaNewsEntities);

            // 整理回傳內容
            return Json(new ApiReturn<List<LatestMediaNewsEntity>>(1, draw, pageControl.size, pageControl.size, latestMediaNewsEntities));
        }

        [HttpPost]
        public async Task<ActionResult> GetLatestMediaNewsInfo(int latestMediaNewsId)
        {
            IEnumerable<LatestMediaNewsEntity> latestMediaNewsEntity = await _latestMediaNewsService.GetLatestMediaNewsAsync(latestMediaNewsId);

            return Json(new ApiResult<IEnumerable<LatestMediaNewsEntity>>(latestMediaNewsEntity));
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> AddLatestMediaNews(FormCollection formCollection)
        {
            var pic = System.Web.HttpContext.Current.Request.Files["latestMediaNewsPicFile"];
            int latestMediaNewsType = Convert.ToInt16(formCollection["latestMediaNewsType"]);
            string latestMediaNewsTitle = formCollection["latestMediaNewsTitle"];
            string latestMediaNewsContent = formCollection["latestMediaNewsContent"];
            string latestMediaNewsStime = formCollection["latestMediaNewsStime"];
            string latestMediaNewsEtime = formCollection["latestMediaNewsEtime"];
            string latestMediaNewsMovieUrl = formCollection["latestMediaNewsMovieUrl"];

            // 圖片處理區塊
            string picPath = "";
            if (pic != null)
            {
                HttpPostedFileBase filebase = new HttpPostedFileWrapper(pic);
                bool res = _storageHelper.IsImage(filebase);
                if (!res)
                {
                    return Json(new ApiError(1015, "Error file format!", "檔案類型不符！"));
                }
                string picName = this.createPicName();
                picPath = _storageHelper.UploadFile("xiaotasi", "lateNews", picName, filebase);
            }

            var latestMediaNewsEntity = new LatestMediaNewsEntity
            {
                latestMediaNewsType = latestMediaNewsType,
                latestMediaNewsTitle = latestMediaNewsTitle ?? "",
                latestMediaNewsContent = latestMediaNewsContent ?? "",
                latestMediaNewsStime = latestMediaNewsStime,
                latestMediaNewsEtime = latestMediaNewsEtime,
                latestMediaNewsPicPath = picPath ?? "",
                latestMediaNewsMovieUrl = latestMediaNewsMovieUrl ?? ""
            };

            await _latestMediaNewsService.CreateLatestMediaNews(latestMediaNewsEntity);

            return Json(new ApiResult<string>("Add Success", "新增成功"));
        }

        [HttpPost, ValidateInput(false)]
        public async Task<ActionResult> UpdateLatestMediaNewsInfo(FormCollection formCollection)
        {
            var pic = System.Web.HttpContext.Current.Request.Files["latestMediaNewsPicFile"];
            int latestMediaNewsId = Convert.ToInt16(formCollection["latestMediaNewsId"]);
            int latestMediaNewsType = Convert.ToInt16(formCollection["latestMediaNewsType"]);
            string latestMediaNewsTitle = formCollection["latestMediaNewsTitle"];
            string latestMediaNewsContent = formCollection["latestMediaNewsContent"];
            string latestMediaNewsStime = formCollection["latestMediaNewsStime"];
            string latestMediaNewsEtime = formCollection["latestMediaNewsEtime"];
            string latestMediaNewsMovieUrl = formCollection["latestMediaNewsMovieUrl"];

            // 圖片處理區塊
            string picPath = "";
            if (pic != null)
            {
                HttpPostedFileBase filebase = new HttpPostedFileWrapper(pic);
                bool res = _storageHelper.IsImage(filebase);
                if (!res)
                {
                    return Json(new ApiError(1015, "Error file format!", "檔案類型不符！"));
                }
                string picName = this.createPicName();
                picPath = _storageHelper.UploadFile("xiaotasi", "lateNews", picName, filebase);
            }

            var latestMediaNewsEntity = new LatestMediaNewsEntity
            {
                latestMediaNewsId = latestMediaNewsId,
                latestMediaNewsType = latestMediaNewsType,
                latestMediaNewsTitle = latestMediaNewsTitle ?? "",
                latestMediaNewsContent = latestMediaNewsContent ?? "",
                latestMediaNewsStime = latestMediaNewsStime,
                latestMediaNewsEtime = latestMediaNewsEtime,
                latestMediaNewsPicPath = picPath ?? "",
                latestMediaNewsMovieUrl = latestMediaNewsMovieUrl ?? ""
            };

            await _latestMediaNewsService.UpdateLatestMediaNews(latestMediaNewsEntity);

            return Json(new ApiResult<string>("Update Success", "修改成功"));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteLatestMediaNews(int latestMediaNewsId)
        {

            await _latestMediaNewsService.DeleteLatestMediaNews(latestMediaNewsId);

            return Json(new ApiResult<string>("Delete Success", "刪除成功"));
        }

        // 產生圖片名稱
        private string createPicName()
        {
            string n = Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            return "latestMediaNews" + n;
        }
    }
}
