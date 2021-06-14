using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class RoomBed
    {
        public int StayId { get; set; }
        public int RoomId { get; set; }
        public byte BedId { get; set; }
        public byte BedNum { get; set; }

        public virtual Bed Bed { get; set; }
        public virtual Room Room { get; set; }
    }
}
