using System;
namespace XiaoTasiBackend.Models.Entity
{
    public class LatestNewsEntity
    {
        public int latestNewsId { get; set; }

        public string latestNewsTitle { get; set; }

        public string latestNewsContent { get; set; }

        public string latestNewsPicPath { get; set; }

        public string publishStime { get; set; }

        public string publishEtime { get; set; }

        public string fDate { get; set; }

        public string eDate { get; set; }
    }
}
