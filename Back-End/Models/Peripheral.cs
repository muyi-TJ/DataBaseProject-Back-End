using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Peripheral
    {
        public Peripheral()
        {
            Nears = new HashSet<Near>();
        }

        public int PeripheralId { get; set; }
        public string PeripheralName { get; set; }
        public string PeripheralClass { get; set; }
        public int? PeripheralPopularity { get; set; }
        public string DetailedAddress { get; set; }

        public virtual ICollection<Near> Nears { get; set; }
    }
}
