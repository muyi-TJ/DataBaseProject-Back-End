using System;
using System.Collections.Generic;

#nullable disable

namespace Back_End.Models
{
    public partial class Coupon
    {
        public int CouponId { get; set; }
        public byte? CouponTypeId { get; set; }
        public int? CustomerId { get; set; }
        public DateTime CouponStart { get; set; }
        public DateTime CouponEnd { get; set; }

        public virtual CouponType CouponType { get; set; }
        public virtual Customer Customer { get; set; }
    }
}
