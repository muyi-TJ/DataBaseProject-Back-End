using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Near
    {
        public int PeripheralId { get; set; }
        public int StayId { get; set; }
        public decimal Distance { get; set; }

        public virtual Peripheral Peripheral { get; set; }
        public virtual Stay Stay { get; set; }
    }
}
