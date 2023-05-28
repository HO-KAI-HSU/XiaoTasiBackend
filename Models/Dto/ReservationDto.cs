using System;
namespace XiaoTasiBackend.Models.Dto
{
    public class ReservationDto
    {
        public int reservationId { get; set; }

        public string reservationCode { get; set; }

        public string memberCode { get; set; }

        public int travelId { get; set; }

        public int reservationNum { get; set; }

        public int reservationCost { get; set; }

        public int travelStepId { get; set; }

        public int status { get; set; }

        public string reservationCheckPicPath { get; set; }

        public string memberName { get; set; }

        public string seatIds { get; set; }

        public ReservationDto()
        {
        }
    }
}
