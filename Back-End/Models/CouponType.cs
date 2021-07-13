using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class CouponType
    {
        public CouponType()
        {
            Coupons = new HashSet<Coupon>();
            CustomerGroupCoupons = new HashSet<CustomerGroupCoupon>();
        }

        public int CouponTypeId { get; set; }
        public decimal CouponAmount { get; set; }
        public decimal CouponLimit { get; set; }
        public string CouponName { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; }
        public virtual ICollection<CustomerGroupCoupon> CustomerGroupCoupons { get; set; }
    }
}
