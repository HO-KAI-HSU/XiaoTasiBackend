namespace travelManagement.Models.Entity
{
    public class ManagerEntity
    {
        public int id { get; set; }

        public string username { get; set; }

        public string password { get; set; }

        public string memberCode { get; set; }

        public string email { get; set; }

        public string name { get; set; }

        public int roleId { get; set; }

        public bool isModify { get; set; }

        public bool status { get; set; }

        public string token { get; set; }

        public string phone { get; set; }
        public ManagerEntity()
        {
        }
    }
}
