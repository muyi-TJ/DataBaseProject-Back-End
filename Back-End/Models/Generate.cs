using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Generate
    {
        public int OrdersId { get; set; }
        public int StayId { get; set; }
        public int RoomId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal Money { get; set; }

        public virtual Order Orders { get; set; }
        public virtual Room Room { get; set; }
    }
}
