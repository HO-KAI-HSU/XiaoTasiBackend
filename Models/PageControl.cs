using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XiaoTasiBackend.Models
{
    public class PageControl<T>
    {
        /// <summary>
        /// 結果代碼(0=成功，其餘為錯誤代號)
        /// </summary>
        public int size { get; set; }

        // 傳送簡訊
        public List<T> pageControl(int page, int limit, List<T> list)
        {
            List<T> listNew = new List<T>();
            int size = list.Count;
            this.size = size;
            int start = (page - 1) * limit;
            int end = limit;
            if (size < page * limit)
            {
                end = (size - limit * (page - 1));
            }
            if ((size > 0 && page == 1) || (size > 0 && page > 1 && (size / ((page - 1) * limit)) > 0))
            {
                listNew = list.GetRange(start, end).ToList();
            }
            return listNew;
        }
        
    }
}
