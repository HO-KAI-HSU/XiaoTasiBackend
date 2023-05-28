using System;
namespace travelManagement.Models.Entity
{
    /// <summary>
    /// 用來代表一筆需要顯示的Transportation資訊TransportationEntity
    /// </summary>
    public class TransportationEntity
    {
        public int transportationId { get; set; }

        public string transportationName { get; set; }

        public string transportationLicensesNumber { get; set; }

        public string transportationInteriorPicPath { get; set; }

        public TransportationEntity()
        {
        }
    }
}
