using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back_End.Contexts;
using System.Text.Json;
using Back_End.Models;
using Microsoft.Extensions.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomerGroupController
    {
        private readonly ModelContext myContext;
        public CustomerGroupController(ModelContext modelContext)
        {
            myContext = modelContext;
        }

        [HttpGet]
        public string GetCustomerGroup()
        {
            Message message = new Message();
            try
            {
                var customerGropList = myContext.CustomerGroups.ToList();
                message.data.Add("customerGroup", new List<Dictionary<string, dynamic>>());
                for (int i = 0; i < customerGropList.Count; i++)
                {
                    int customerNextLevelDegree = (i == customerGropList.Count - 1) ? 999 : customerGropList[i + 1].CustomerLevelDegree;
                    int count = 0;
                    try
                    {
                        count = (int)customerGropList[i].CustomerGroupCoupon.CouponType.CouponAmount;
                    }
                    catch
                    {

                    }
                    message.data["customerGroup"].Add(
                        new Dictionary<string, dynamic> {
                            { "customerLevel", customerGropList[i].CustomerLevelDegree },
                            { "customerLevelName", customerGropList[i].CustomerLevelName },
                            { "customerNextLevelDegree", customerNextLevelDegree},
                            {"customerLevelCouponNum", customerGropList[i].Customers.Count },
                            {"customerLevelCouponAmount", count}
                        }
                    );
                    message.errorCode = 200;
                    return message.ReturnJson();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                message.errorCode = 300;
                return message.ReturnJson();
            }
            return message.ReturnJson();
        }
    }
}
