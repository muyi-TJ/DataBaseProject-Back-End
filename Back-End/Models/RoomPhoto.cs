using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class RoomPhoto
    {
        public int StayId { get; set; }
        public int RoomId { get; set; }
        public string RPhoto { get; set; }

        public virtual Room Room { get; set; }
    }
}
