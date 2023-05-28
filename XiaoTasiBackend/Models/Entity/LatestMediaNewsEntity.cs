using System;
namespace XiaoTasiBackend.Models.Entity
{
    public class LatestMediaNewsEntity
    {
        public int latestMediaNewsId { get; set; }

        public int latestMediaNewsType { get; set; }

        public string latestMediaNewsTitle { get; set; }

        public string latestMediaNewsContent { get; set; }

        public string latestMediaNewsMovieUrl { get; set; }

        public string latestMediaNewsPicPath { get; set; }

        public string fDate { get; set; }

        public string eDate { get; set; }

        public string latestMediaNewsStime { get; set; }

        public string latestMediaNewsEtime { get; set; }
    }
}
