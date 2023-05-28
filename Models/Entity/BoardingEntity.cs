using System;
namespace travelManagement.Models.Entity
{
    public class BoardingEntity
    {
        public int boardingId { get; set; }
        public int locationId { get; set; }
        public string boardingDatetime { get; set; }
        public string customBoardingFlag { get; set; }
        public string earlyBoardingFlag { get; set; }
    }
}
