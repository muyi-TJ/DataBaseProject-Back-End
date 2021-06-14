using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Bed
    {
        public Bed()
        {
            RoomBeds = new HashSet<RoomBed>();
        }

        public byte BedId { get; set; }
        public int BedType { get; set; }
        public decimal BedLength { get; set; }
        public bool PersonNum { get; set; }

        public virtual ICollection<RoomBed> RoomBeds { get; set; }
    }
}
