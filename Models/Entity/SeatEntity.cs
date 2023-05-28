using System;
namespace travelManagement.Models.Entity
{

    public class SeatEntity
    {
        public int transportationId { get; set; }

        public string transportationName { get; set; }

        public string transportationLicensesNumber { get; set; }

        public string transportationInteriorPicPath { get; set; }

        public SeatEntity()
        {
        }
    }
}
