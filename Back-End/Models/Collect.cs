using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Collect
    {
        public int CustomerId { get; set; }
        public int StayId { get; set; }
        public DateTime? CollectDate { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual Stay Stay { get; set; }
    }
}
