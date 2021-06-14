using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class StayType
    {
        public StayType()
        {
            Stays = new HashSet<Stay>();
        }

        public string StayType1 { get; set; }
        public string Characteristic { get; set; }

        public virtual ICollection<Stay> Stays { get; set; }
    }
}
