using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Order
    {
        public Order()
        {
            Generates = new HashSet<Generate>();
        }

        public int OrderId { get; set; }
        public int CustomerId { get; set; }
        public DateTime? OrderTime { get; set; }
        public decimal MemberNum { get; set; }
        public decimal TotalCost { get; set; }

        public virtual Customer Customer { get; set; }
        public virtual CustomerComment CustomerComment { get; set; }
        public virtual HostComment HostComment { get; set; }
        public virtual Report Report { get; set; }
        public virtual ICollection<Generate> Generates { get; set; }
    }
}
