using System;
using System.Collections.Generic;
using Back_End.Contexts;
using System.Linq;

#nullable disable

namespace Back_End.Models
{
    public partial class Customer
    {
        public Customer()
        {
            Collects = new HashSet<Collect>();
            Coupons = new HashSet<Coupon>();
            Orders = new HashSet<Order>();
        }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPassword { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerPrephone { get; set; }
        public DateTime? CustomerCreatetime { get; set; }
        public string CustomerPhoto { get; set; }
        public string CustomerGender { get; set; }
        public DateTime? CustomerBirthday { get; set; }
        public decimal? CustomerState { get; set; }
        public byte? CustomerLevel { get; set; }
        public int? CustomerDegree { get; set; }

        public virtual CustomerGroup CustomerLevelNavigation { get; set; }
        public virtual ICollection<Collect> Collects { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; }
        public virtual ICollection<Order> Orders { get; set; }

        public static bool Login(string message, string password)
        {
            try
            {
                using (var context = new ModelContext())
                {
                    var customer = context.Customers
                        .Single(b => b.CustomerPhone == message || b.CustomerEmail == message);
                    return customer.CustomerPassword == password;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
