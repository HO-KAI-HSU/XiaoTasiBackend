using System;
namespace XiaoTasiBackend.Models.Dto
{
    public class ManagerLoginDto
    {
        public int id { get; set; }
        public string username { get; set; }
        public string memberCode { get; set; }
        public string email { get; set; }
        public string name { get; set; }
        public int role { get; set; }
        public bool modify { get; set; }
        public bool status { get; set; }
        public string token { get; set; }
        public string phone { get; set; }
        public ManagerLoginDto()
        {
        }
    }
}
