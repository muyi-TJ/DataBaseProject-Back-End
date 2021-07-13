using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class CustomerGroup
    {
        public CustomerGroup()
        {
            Customers = new HashSet<Customer>();
        }

        public short CustomerLevel { get; set; }
        public string CustomerLevelName { get; set; }
        public int CustomerLevelDegree { get; set; }

        public virtual CustomerGroupCoupon CustomerGroupCoupon { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
    }
}
