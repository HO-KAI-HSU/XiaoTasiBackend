using System;
namespace XiaoTasiBackend.Models.Entity
{
    /// <summary>
    /// 用來代表一筆需要顯示的MemberReservationEntity資訊MemberReservationEntity
    /// </summary>
    public class MemberReservationEntity
    {
        public int mrId { get; set; }

        public string id { get; set; }

        public string name { get; set; }

        public string phone { get; set; }

        public string birthday { get; set; }

        public int mealsType { get; set; }

        public int boardingId { get; set; }

        public int transportationId { get; set; }

        public int seatId { get; set; }

        public int travelStepId { get; set; }

        public string memberCode { get; set; }

        public string reservationCode { get; set; }

        public string note { get; set; }

        public int roomsType { get; set; }

        public int status { get; set; }

        public MemberReservationEntity()
        {
        }
    }
}
