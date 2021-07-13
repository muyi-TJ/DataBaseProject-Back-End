using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class CustomerGroupCoupon
    {
        public short CustomerLevel { get; set; }
        public int CouponTypeId { get; set; }
        public int CouponNum { get; set; }

        public virtual CouponType CouponType { get; set; }
        public virtual CustomerGroup CustomerLevelNavigation { get; set; }
    }
}
