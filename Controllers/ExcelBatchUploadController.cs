using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using ClosedXML.Excel;
using travelManagement.Models;
using travelManagement.Models.Entity;
using travelManagement.Service;
using travelManagement.Service.Impl;

namespace travelManagement.Controllers
{
    public class ExcelBatchUploadController : Controller
    {
        HotelService _hotelService;
        ViewpointService _viewpointService;

        public ExcelBatchUploadController()
        {
            _hotelService = new HotelServiceImpl();
            _viewpointService = new ViewpointServiceImpl();
        }


        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ImportFromExcel(IEnumerable<HttpPostedFileBase> excelFile, String excelModule)
        {
            List<string> keyArr = new List<string>();
            List<List<string>> valueArrList = new List<List<string>>();
            List<KeyValuePair<string, object>> keyValuePairs = new List<KeyValuePair<string, object>>();

            bool pass = false;
            try
            {
                if (excelFile == null || excelFile.First() == null) throw new ApplicationException("未選取檔案或檔案上傳失敗");
                if (excelFile.Count() != 1) throw new ApplicationException("請上傳單一檔案");
                var file = excelFile.First();
                //if (Path.GetExtension(file.FileName) != ".xlsx") throw new ApplicationException("請使用Excel 2007(.xlsx)格式");
                var stream = file.InputStream;
                XLWorkbook wb = new XLWorkbook(stream);
                var ws1 = wb.Worksheet(excelModule).RowsUsed();
                var index = 0;
                int rowCount = 5;
                string errorRow = "";
                foreach (var dataRow in ws1)
                {
                    if (index == 2)
                    {
                        // Arrange key 
                        if (dataRow.Cell(1).Value.ToString().Equals("id"))
                        {
                            int colTotalCount = dataRow.CellCount();
                            for (int colIndex = 1; colIndex <= colTotalCount; colIndex++)
                            {
                                if (dataRow.Cell(colIndex).Value.Equals(""))
                                {
                                    break;
                                }
                                string key = dataRow.Cell(colIndex).Value.ToString();
                                keyArr.Add(key);
                            }
                        }
                    }
                    else if (index >= 4)
                    {
                        // Arrange value 
                        List<string> valueArr = new List<string>();


                        if (dataRow.Cell(1).Value.ToString().Equals(""))
                        {
                            break;
                        }

                        int colCount = keyArr.Count();


                        for (int colIndex = 1; colIndex <= colCount; colIndex++)
                        {
                            string value = Encoding.GetEncoding("gb2312").GetString(Encoding.GetEncoding("gb2312").GetBytes(dataRow.Cell(colIndex).Value.ToString()));
                            string valEncoding = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(value));
                            valueArr.Add(valEncoding);
                        }
                        valueArrList.Add(valueArr);
                    }
                    index++;
                }

                if (excelModule.Equals("hotel"))
                {
                    List<HotelEntity> hotelEntities = new List<HotelEntity>();
                    if (!valueArrList.Any())
                    {
                        errorRow = errorRow + rowCount + ",";
                    }
                    else
                    {
                        foreach (List<string> valueArr in valueArrList)
                        {
                            HotelEntity hotelEntity = new HotelEntity();
                            string[] valStrArr = valueArr.ToArray();
                            if (valStrArr != null
                                && (string.IsNullOrEmpty(valStrArr[1])
                                || string.IsNullOrEmpty(valStrArr[2])
                                || string.IsNullOrEmpty(valStrArr[3])
                                || string.IsNullOrEmpty(valStrArr[4])
                                || string.IsNullOrEmpty(valStrArr[5])))
                            {
                                errorRow = errorRow + rowCount + ",";
                            }
                            hotelEntity.hotelName = valStrArr[1];
                            hotelEntity.hotelCity = valStrArr[2];
                            hotelEntity.hotelArea = valStrArr[3];
                            hotelEntity.hotelAddress = valStrArr[4];
                            hotelEntity.hotelContent = valStrArr[5];
                            hotelEntities.Add(hotelEntity);
                            rowCount++;
                        }
                    }

                    if (string.IsNullOrEmpty(errorRow))
                    {
                        pass = await _hotelService.AddHotelFromExcel(hotelEntities);
                    }
                    else
                    {
                        pass = false;
                    }
                }
                else if (excelModule.Equals("viewpoint"))
                {
                    List<ViewpointEntity> viewpointDatas = new List<ViewpointEntity>();
                    if (!valueArrList.Any())
                    {
                        errorRow = errorRow + rowCount + ",";
                    }
                    else
                    {
                        foreach (List<string> valueArr in valueArrList)
                        {
                            ViewpointEntity viewpointData = new ViewpointEntity();
                            string[] valStrArr = valueArr.ToArray();
                            if (valStrArr != null
                                && (string.IsNullOrEmpty(valStrArr[1])
                                || string.IsNullOrEmpty(valStrArr[2])
                                || string.IsNullOrEmpty(valStrArr[3])
                                || string.IsNullOrEmpty(valStrArr[4])
                                || string.IsNullOrEmpty(valStrArr[5])))
                            {
                                errorRow = errorRow + rowCount + ",";
                            }
                            viewpointData.viewpointTitle = valStrArr[1];
                            viewpointData.viewpointCity = valStrArr[2];
                            viewpointData.viewpointArea = valStrArr[3];
                            viewpointData.viewpointAddress = valStrArr[4];
                            viewpointData.viewpointContent = valStrArr[5];
                            viewpointDatas.Add(viewpointData);
                            rowCount++;
                        }
                    }

                    if (string.IsNullOrEmpty(errorRow))
                    {
                        pass = await _viewpointService.AddViewpointFromExcel(viewpointDatas);
                    }
                    else
                    {
                        pass = false;
                    }
                }
                else if (excelModule.Equals("memberReservation"))
                {
                    ReservationController reservationController = new ReservationController();
                    List<ReservationController.MemberReservationData> reservationDatas = new List<ReservationController.MemberReservationData>();
                    if (!valueArrList.Any())
                    {
                        errorRow = errorRow + rowCount + ",";
                    }
                    else
                    {
                        foreach (List<string> valueArr in valueArrList)
                        {
                            ReservationController.MemberReservationData reservationData = new ReservationController.MemberReservationData();
                            string[] valStrArr = valueArr.ToArray();
                            if (valStrArr != null
                                && (string.IsNullOrEmpty(valStrArr[1])
                                || string.IsNullOrEmpty(valStrArr[2])
                                || string.IsNullOrEmpty(valStrArr[3])
                                || string.IsNullOrEmpty(valStrArr[4])
                                || string.IsNullOrEmpty(valStrArr[5])
                                || string.IsNullOrEmpty(valStrArr[6])
                                || string.IsNullOrEmpty(valStrArr[7])
                                || string.IsNullOrEmpty(valStrArr[8])
                                || string.IsNullOrEmpty(valStrArr[9])
                                || (Convert.ToInt16(valStrArr[3]) != 1 && string.IsNullOrEmpty(valStrArr[10]))
                                || string.IsNullOrEmpty(valStrArr[11])
                                || string.IsNullOrEmpty(valStrArr[13])))
                            {
                                errorRow = errorRow + rowCount + ",";
                            }
                            reservationData.transportationCode = valStrArr[1];
                            reservationData.seatPos = Convert.ToInt16(valStrArr[2]);
                            reservationData.travelType = Convert.ToInt16(valStrArr[3]);
                            reservationData.travelStepId = Convert.ToInt16(valStrArr[4]);
                            reservationData.reservationMemberCode = valStrArr[5];
                            reservationData.memberName = valStrArr[6];
                            reservationData.memberBirthday = Convert.ToDateTime(valStrArr[7]).ToString("yyyy-MM-dd");
                            reservationData.memberIdCode = valStrArr[8];
                            reservationData.memberPhone = valStrArr[9];
                            reservationData.roomsType = Convert.ToInt16(valStrArr[10]);
                            reservationData.foodsType = Convert.ToInt16(valStrArr[11]);
                            reservationData.memo = valStrArr[12];
                            reservationData.boardingId = Convert.ToInt16(valStrArr[13]);
                            reservationDatas.Add(reservationData);
                            rowCount++;
                        }
                    }

                    if (string.IsNullOrEmpty(errorRow))
                    {
                        pass = await reservationController._AddMemberReservation(reservationDatas);
                    }
                    else
                    {
                        pass = false;
                    }
                }
                else if (excelModule.Equals("travel"))
                {
                    TravelController travelController = new TravelController();
                    List<TravelController.TravelData> travelDatas = new List<TravelController.TravelData>();
                    if (!valueArrList.Any())
                    {
                        errorRow = errorRow + rowCount + ",";
                    }
                    else
                    {
                        foreach (List<string> valueArr in valueArrList)
                        {
                            TravelController.TravelData travelData = new TravelController.TravelData();
                            TravelController.CostInfo costInfo = new TravelController.CostInfo();
                            string[] valStrArr = valueArr.ToArray();
                            if (valStrArr != null
                                && (string.IsNullOrEmpty(valStrArr[1])
                                || string.IsNullOrEmpty(valStrArr[2])
                                || string.IsNullOrEmpty(valStrArr[3])
                                || string.IsNullOrEmpty(valStrArr[4])
                                || string.IsNullOrEmpty(valStrArr[5])
                                || string.IsNullOrEmpty(valStrArr[6])
                                || string.IsNullOrEmpty(valStrArr[7])
                                || string.IsNullOrEmpty(valStrArr[8])
                                || string.IsNullOrEmpty(valStrArr[9])
                                || string.IsNullOrEmpty(valStrArr[10])
                                || (Convert.ToInt16(valStrArr[10]) == 2 && string.IsNullOrEmpty(valStrArr[11]))
                                || string.IsNullOrEmpty(valStrArr[12])
                                || (Convert.ToInt16(valStrArr[1]) != 1 && string.IsNullOrEmpty(valStrArr[13]))
                                || (Convert.ToInt16(valStrArr[1]) != 1 && string.IsNullOrEmpty(valStrArr[14]))
                                || string.IsNullOrEmpty(valStrArr[15])
                                || (Convert.ToInt16(valStrArr[1]) == 1 && string.IsNullOrEmpty(valStrArr[21]))))
                            {
                                errorRow = errorRow + rowCount + ",";
                            }
                            travelData.travelType = Convert.ToInt16(valStrArr[1].ToString());
                            travelData.travelName = valStrArr[2].ToString();
                            travelData.travelContent = valStrArr[3].ToString();
                            travelData.travelStime = valStrArr[4].ToString();
                            travelData.travelEtime = valStrArr[5].ToString();
                            travelData.travelCost = Convert.ToInt16(valStrArr[6].ToString());
                            travelData.travelNum = Convert.ToInt16(valStrArr[7].ToString());
                            travelData.travelDateNum = Convert.ToInt16(valStrArr[8].ToString());
                            travelData.travelDetailSdate = valStrArr[9].ToString();
                            travelData.travelCustomBoardingFlag = valStrArr[10].ToString();
                            travelData.travelBoardingIds = valStrArr[11].ToString();
                            travelData.travelTransportationIds = valStrArr[12].ToString();
                            travelData.travelDetailViewpointIds = valStrArr[13].ToString();
                            travelData.travelDetailHotelIds = valStrArr[14].ToString();
                            travelData.travelDetailMealNames = valStrArr[15].ToString();
                            travelData.travelViewpointInfo = valStrArr[21].ToString();

                            costInfo.transportationInfo = valStrArr[16].ToString();
                            costInfo.eatInfo = valStrArr[17].ToString();
                            costInfo.liveInfo = valStrArr[18].ToString();
                            costInfo.actionInfo = valStrArr[19].ToString();
                            costInfo.insuranceInfo = valStrArr[20].ToString();
                            travelData.travelCostInfo = costInfo;
                            travelDatas.Add(travelData);
                            rowCount++;
                        }
                    }

                    if (string.IsNullOrEmpty(errorRow))
                    {
                        pass = await travelController._batchUploadTravel(travelDatas);
                    }
                    else
                    {
                        pass = false;
                    }
                }
                if (!pass && !string.IsNullOrEmpty(errorRow))
                {
                    return Json(new ApiError(2001, "Required field(s) is missing!", "批次上傳第" + errorRow + "行開始資料填寫不齊全，請填寫完整"));
                }
                else if (!pass)
                {
                    return Json(new ApiError(2002, "Upload Fail!", "上傳失敗"));
                }
                KeyValuePair<string, object> keyValuePairKey = new KeyValuePair<string, object>("key", keyArr);
                KeyValuePair<string, object> keyValuePairValue = new KeyValuePair<string, object>("value", valueArrList);
                keyValuePairs.Add(keyValuePairKey);
                keyValuePairs.Add(keyValuePairValue);
                return Json(new ApiResult<string>("Upload Success", "上傳成功"));
            }
            catch (Exception ex)
            {
                return Json(new ApiError(2002, "Upload Fail!", ex.Message));
            }
        }
    }
}
