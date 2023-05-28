using System;
namespace XiaoTasiBackend.Models.Dto
{
    public class TransportationDto
    {
        public int transportationId { get; set; }

        public string transportationName { get; set; }

        public string transportationLicensesNumber { get; set; }

        public string transportationInteriorPicPath { get; set; }

        public TransportationDto()
        {
        }
    }
}
