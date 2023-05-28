using System;
namespace XiaoTasiBackend.Models.Entity
{
    /// <summary>
    /// 用來代表一筆需要顯示的ReservationEntity資訊ReservationEntity
    /// </summary>
    public class ReservationEntity
    {
        public int reservationId { get; set; }

        public string reservationCode { get; set; }

        public string memberCode { get; set; }

        public int travelId { get; set; }

        public int reservationNum { get; set; }

        public int reservationCost { get; set; }

        public int travelStepId { get; set; }

        public int status { get; set; }

        public string seatIds { get; set; }

        public string note { get; set; }

        public ReservationEntity()
        {
        }
    }
}
