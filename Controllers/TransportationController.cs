using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using XiaoTasiBackend.Models;
using XiaoTasiBackend.Models.Entity;
using XiaoTasiBackend.Service;
using XiaoTasiBackend.Service.Impl;

namespace XiaoTasiBackend.Controllers
{
    public class TransportationController : Controller
    {
        TransportationService _transportationService;

        public TransportationController()
        {
            _transportationService = new TransportationServiceImpl();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetTransportationList(int draw = 1, int start = 0, int length = 0)
        {

            List<TransportationEntity> transportationDatas = await _transportationService.GetTransportationList();

            PageControl<TransportationEntity> pageControl = new PageControl<TransportationEntity>();

            List<TransportationEntity> transportationListNew = pageControl.pageControl((start + 1), length, transportationDatas);

            // 整理回傳內容
            return Json(new ApiReturn<List<TransportationEntity>>(1, draw, pageControl.size, pageControl.size, transportationDatas));
        }

        [HttpPost]
        public async Task<ActionResult> GetTransportationInfo(int transportationId)
        {
            IEnumerable<TransportationEntity> transportation = await _transportationService.GetTransportationAsync(transportationId);
            return Json(new ApiResult<IEnumerable<TransportationEntity>>(transportation));
        }

        [HttpPost]
        public async Task<ActionResult> AddTransportation(string transportationName, string transportationLicensesNumber, string transportationInteriorPicPath)
        {
            var fileName = "";
            if (!string.IsNullOrEmpty(transportationInteriorPicPath))
            {
                var pic = System.Web.HttpContext.Current.Request.Files["transportationPicFile"];
                HttpPostedFileBase filebase = new HttpPostedFileWrapper(pic);
                UploadFile fileObj = new UploadFile();
                fileName = Path.GetFileName(filebase.FileName);
                fileObj._uploadPic("~/Scripts/img/transportation/", filebase);
            }



            TransportationEntity transportation = new TransportationEntity()
            {
                transportationId = 0,
                transportationName = transportationName,
                transportationLicensesNumber = transportationLicensesNumber,
                transportationInteriorPicPath = fileName,
            };

            await _transportationService.CreateTransportation(transportation);
            return Json(new ApiResult<string>("Add Success", "新增成功"));
        }

        [HttpPost]
        public async Task<ActionResult> UpdateTransportationInfo(int transportationId, string transportationName, string transportationLicensesNumber)
        {
            TransportationEntity transportation = new TransportationEntity()
            {
                transportationId = transportationId,
                transportationName = transportationName,
                transportationLicensesNumber = transportationLicensesNumber,
            };

            await _transportationService.UpdateTransportation(transportation);
            return Json(new ApiResult<string>("Update Success", "更新成功"));
        }

        [HttpPost]
        public async Task<ActionResult> DeleteTransportation(int transportationId)
        {
            await _transportationService.DeleteTransportation(transportationId);
            return Json(new ApiResult<string>("Delete Success", "刪除成功"));
        }


        //群組資訊物件
        public class TransportationData
        {
            public int transportationId { get; set; }
            public string transportationName { get; set; }
            public string transportationLicensesNumber { get; set; }
            public string transportationInteriorPicPath { get; set; }
            public static explicit operator TransportationData(SqlDataReader v)
            {
                throw new NotImplementedException();
            }
        }
    }
}
