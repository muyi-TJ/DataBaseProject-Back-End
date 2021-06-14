using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Area
    {
        public Area()
        {
            Peripherals = new HashSet<Peripheral>();
            Stays = new HashSet<Stay>();
        }

        public int AreaId { get; set; }
        public string AreaName { get; set; }

        public virtual ICollection<Peripheral> Peripherals { get; set; }
        public virtual ICollection<Stay> Stays { get; set; }
    }
}
