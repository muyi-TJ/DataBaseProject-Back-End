using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back_End.Contexts;
using System.Text.Json;
using Back_End.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CouponController : ControllerBase
    {
        private readonly ModelContext myContext;
        public CouponController(ModelContext modelContext)
        {
            myContext = modelContext;
        }

        class CouponInfo
        {
            public string couponName { get; set; }
            public decimal couponAmount { get; set; }
            public decimal couponLimit { get; set; }
            public DateTime couponStart { get; set; }
            public DateTime couponEnd { get; set; }
        }


        [HttpGet("CouponInfo")]
        public string GetCustomerCoupon()
        {
            GetCustomerCouponMessage message = new GetCustomerCouponMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    myContext.DetachAll();
                    int id = int.Parse(data["id"]);
                    var customer =CustomerController. SearchById(id);
                    if(customer!=null)
                    {
                        List<Coupon> coupons = customer.Coupons.ToList();
                        List<CouponInfo> couponList = new List<CouponInfo>();
                        foreach (var coupon in coupons)
                        {
                            CouponInfo info = new CouponInfo();
                            info.couponName = coupon.CouponType.CouponName;
                            info.couponAmount = coupon.CouponType.CouponAmount;
                            info.couponLimit = coupon.CouponType.CouponLimit;
                            info.couponStart = coupon.CouponStart;
                            info.couponEnd = coupon.CouponEnd;
                            couponList.Add(info);
                        }
                        message.errorCode = 200;
                        message.data["couponList"] = couponList;

                    }
                }
            }
            return message.ReturnJson();
        }



    }
}
