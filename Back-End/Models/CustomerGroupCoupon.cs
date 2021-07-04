using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class CustomerGroupCoupon
    {
        public byte CustomerLevel { get; set; }
        public int CouponTypeId { get; set; }
        public bool CouponNum { get; set; }

        public virtual CouponType CouponType { get; set; }
        public virtual CustomerGroup CustomerLevelNavigation { get; set; }
    }
}
