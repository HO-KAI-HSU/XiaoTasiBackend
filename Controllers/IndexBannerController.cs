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
    public class IndexBannerController : Controller
    {
        StorageHelper _storageHelper;
        IndexBannerService _indexBannerService;

        public IndexBannerController()
        {
            _indexBannerService = new IndexBannerServiceImpl();
            _storageHelper = new StorageHelper();
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetIndexBannerList(int draw = 1, int start = 0, int length = 0)
        {
            List<IndexBannerEntity> indexBannerEntities = await _indexBannerService.GetIndexBannerList();

            PageControl<IndexBannerEntity> pageControl = new PageControl<IndexBannerEntity>();

            List<IndexBannerEntity> indexBannerListNew = pageControl.pageControl((start + 1), length, indexBannerEntities);

            // 整理回傳內容
            return Json(new ApiReturn<List<IndexBannerEntity>>(1, draw, pageControl.size, pageControl.size, indexBannerEntities));
        }

        [HttpPost]
        public async Task<ActionResult> GetIndexBannerInfo(int indexBannerId)
        {
            IEnumerable<IndexBannerEntity> indexBannerEntity = await _indexBannerService.GetIndexBannerAsync(indexBannerId);

            return Json(new ApiResult<IEnumerable<IndexBannerEntity>>(indexBannerEntity));
        }

        [HttpPost]
        public async Task<ActionResult> AddIndexBanner(FormCollection formCollection)
        {
            var pic = System.Web.HttpContext.Current.Request.Files["indexBannerPicFile"];
            string indexBannerStime = formCollection["indexBannerStime"];
            string indexBannerEtime = formCollection["indexBannerEtime"];
            string indexBannerTitle = formCollection["indexBannerTitle"];
            string indexBannerContent = formCollection["indexBannerContent"];

            // 圖片處理區塊
            HttpPostedFileBase filebase = new HttpPostedFileWrapper(pic);
            bool res = _storageHelper.IsImage(filebase);
            if (!res)
            {
                return Json(new ApiError(1014, "Token Expired!", "登入時效已過，請重新登入！"));
            }
            string picName = this.createPicName();
            string url = _storageHelper.UploadFile("xiaotasi", "indexBanner", picName, filebase);

            var indexBanner = new IndexBannerEntity
            {
                indexBannerTitle = indexBannerTitle ?? "",
                indexBannerContent = indexBannerContent ?? "",
                indexBannerPicPath = url,
                indexBannerStime = indexBannerStime,
                indexBannerEtime = indexBannerEtime
            };

            await _indexBannerService.CreateIndexBanner(indexBanner);

            return Json(new ApiResult<string>("Add Success", "新增成功"));
        }

        [HttpPost]
        public async Task<ActionResult> UpdateIndexBannerInfo(FormCollection formCollection)
        {
            var pic = System.Web.HttpContext.Current.Request.Files["indexBannerPicFile"];
            int indexBannerId = Convert.ToInt16(formCollection["indexBannerId"]);
            string indexBannerStime = formCollection["indexBannerStime"];
            string indexBannerEtime = formCollection["indexBannerEtime"];
            string indexBannerTitle = formCollection["indexBannerTitle"];
            string indexBannerContent = formCollection["indexBannerContent"];

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
                url = _storageHelper.UploadFile("xiaotasi", "indexBanner", picName, filebase);
            }

            var indexBanner = new IndexBannerEntity
            {

                indexBannerId = indexBannerId,
                indexBannerPicPath = url ?? "",
                indexBannerTitle = indexBannerTitle ?? "",
                indexBannerContent = indexBannerContent ?? "",
                indexBannerStime = indexBannerStime,
                indexBannerEtime = indexBannerEtime
            };

            await _indexBannerService.UpdateIndexBanner(indexBanner);

            return Json(new ApiResult<string>("Update Success", "更新成功"));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteIndexBanner(int indexBannerId)
        {

            await _indexBannerService.DeleteIndexBanner(indexBannerId);

            return Json(new ApiResult<string>("Delete Success", "刪除成功"));
        }

        // 產生圖片名稱
        private string createPicName()
        {
            string n = Convert.ToInt32(DateTime.UtcNow.AddHours(8).Subtract(new DateTime(1970, 1, 1)).TotalSeconds).ToString();
            return "indexBanner" + n;
        }
    }
}
