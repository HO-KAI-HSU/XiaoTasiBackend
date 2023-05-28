using System;
namespace XiaoTasiBackend.Models.Entity
{
    public class ReservationCheckEntity
    {
        public int reservationCheckId { get; set; }

        public string reservationCode { get; set; }

        public string memberCode { get; set; }

        public string bankbookAccountName { get; set; }

        public string bankbookAccountCode { get; set; }

        public int status { get; set; }

        public string reservationCheckPicPath { get; set; }

        public ReservationCheckEntity()
        {
        }
    }
}
