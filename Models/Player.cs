using System;

namespace XiaoTasiBackend.Controllers
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime RegDate { get; set; }
        public int Score { get; set; }

        public string ToText()
        {
            return string.Format("I:{0} N:{1} R:{2:yyyy-MM-dd} S:{2:n0}",
                Id, Name, RegDate, Score);
        }
    }
}