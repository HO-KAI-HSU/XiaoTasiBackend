using System;
namespace travelManagement.Models.Entity
{
    public class MemberEntity
    {
        public int memberId { get; set; }

        public string memberCode { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        public string email { get; set; }

        public string address { get; set; }

        public string name { get; set; }

        public string phone { get; set; }

        public string telephone { get; set; }

        public int status { get; set; }

        public MemberEntity()
        {
        }
    }
}
