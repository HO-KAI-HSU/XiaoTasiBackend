using System;
namespace XiaoTasiBackend.Models.Entity
{
    public class TravelStepEntity
    {
        public int travelStepId { get; set; }

        public string travelStime { get; set; }

        public string travelEtime { get; set; }

        public string travelStepCode { get; set; }

        public int travelCost { get; set; }

        public TravelStepEntity()
        {
        }
    }
}
