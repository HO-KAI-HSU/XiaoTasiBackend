using System;
using System.Collections.Generic;

namespace travelManagement.Models
{
    // <summary>
    /// API呼叫時，傳回的統一物件
    /// </summary>
    public class ApiResult<T>
    {
        /// <summary>
        /// 執行成功與否
        /// </summary>
        public int success { get; set; }
        /// <summary>
        /// 結果代碼(0=成功，其餘為錯誤代號)
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 英文訊息
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// 中文訊息
        /// </summary>
        public string reason { get; set; }
        /// <summary>
        /// 資料本體
        /// </summary>
        public T data { get; set; }

        /// <summary>
        /// 建立成功結果(更新，上傳)
        /// </summary>
        /// <param name="data"></param>
        public ApiResult()
        {
        }

        /// <summary>
        /// 建立成功結果(更新，上傳)
        /// </summary>
        /// <param name="data"></param>
        public ApiResult(string msg, string reas)
        {
            success = 1;
            message = msg;
            reason = reas;
        }

        /// <summary>
        /// 建立成功結果（顯示列表，訊息）
        /// </summary>
        /// <param name="data"></param>
        public ApiResult(T dataObj)
        {

            success = 1;
            data = dataObj;
        }
    }

    public class ApiError : ApiResult<object>
    {
        /// <summary>
        /// 建立失敗結果
        /// </summary>
        /// <param name="code"></param>
        /// <param name="success"></param>
        /// <param name="message"></param>
        /// <param name="reason"></param>
        public ApiError(int codes, string msg, string reas)
        {
            code = codes;
            success = 0;
            message = msg;
            reason = reas;
        }
    }

    public class ApiReturn<T> : ApiResult<T>
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public ApiReturn(int s, int drawI, int records, int recordsFilter, T datas)
        {
            success = s;
            draw = drawI;
            recordsTotal = records;
            recordsFiltered = recordsFilter;
            data = datas;
        }
    }
}
