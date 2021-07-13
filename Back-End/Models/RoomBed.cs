using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class RoomBed
    {
        public int StayId { get; set; }
        public int RoomId { get; set; }
        public string BedType { get; set; }
        public byte BedNum { get; set; }

        public virtual Bed BedTypeNavigation { get; set; }
        public virtual Room Room { get; set; }
    }
}
