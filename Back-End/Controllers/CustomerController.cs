using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back_End.Contexts;
using System.Text.Json;
using Back_End.Models;


//简单测试
namespace Back_End.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase {

        //GET: api/<Customer>
        [HttpGet]
        public string Get(int customerId) {
            try {
                using (var context = new ModelContext()) {
                    var customer = context.Customers
                        .Single(b => b.CustomerId == customerId);
                    String customerString = JsonSerializer.Serialize(customer);
                    Console.WriteLine(customerString);
                    return customerString;
                }
            }
            catch {
                return "meizhaodao";
            }
        }

        [HttpPost]
        public bool Post([FromForm] Customer customer) {
            try {
                using (var context = new ModelContext()) {
                    Console.WriteLine(customer);
                    context.Add(customer);
                    context.SaveChanges();
                }
                return true;
            }
            catch {
                return false;
            }
        }

        [HttpDelete]
        public bool Delete(int customerId) {
            if (customerId < 0)
                return false;
            try {
                using (var context = new ModelContext()) {
                    Customer customer = new Customer() { CustomerId = customerId };
                    context.Customers.Attach(customer);
                    context.Customers.Remove(customer);
                    context.SaveChanges();
                }
                return true;
            }
            catch {
                return false;
            }
        }
    }
}
