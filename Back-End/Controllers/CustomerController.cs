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
        
        
        [HttpDelete]
        public bool Delete(int id) {
            if (id < 0)
                return false;
            try
            {
                var context = ModelContext.Instance;
                context.DetachAll();
                Customer customer = new Customer() { CustomerId = id };
                context.Customers.Attach(customer);
                context.Customers.Remove(customer);
                context.SaveChanges();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        //GET: api/<Customer>
        [HttpPost]
        public bool GetAction()
        {
            string action = Request.Form["action"];
            switch(action)
            {
                case "login":return Login();
                case "register":return Register();
                case "changepsw":return ChangePassword();
                default:return false;
            }
        }
        public bool Login()
        {
            string phone = Request.Form["phone"]; //接受 Form 提交的数据
            string email = Request.Form["mail"];
            string password = Request.Form["password"];
            string prePhone = Request.Form["prePhone"];
            Customer customer;
            if(email==null)
            {
                customer = SearchByPhone(phone, prePhone);
            }
            else
            {
                customer = SearchByEmail(email);
            }
            return CustomerLogin(customer, password);
        }

        public bool Register()
        {
            try
            {
                Customer customer = new Customer();
                customer.CustomerName = Request.Form["name"];
                customer.CustomerPassword = Request.Form["password"];
                customer.CustomerPrephone = Request.Form["prePhone"];
                customer.CustomerPhone = Request.Form["phone"];
                customer.CustomerCreatetime = DateTime.Now;
                ModelContext.Instance.Add(customer);
                ModelContext.Instance.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ChangePassword()
        {
            try
            {
                Customer customer = SearchById(int.Parse(Request.Form["id"]));
                if(customer==null)
                {
                    return false;
                }
                if(customer.CustomerPassword==Request.Form["password"])
                {
                    customer.CustomerPassword = Request.Form["newpassword"];
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool CustomerLogin(Customer customer, string password)
        {
            try
            {
                if (customer == null)
                {
                    return false;
                }
                return customer.CustomerPassword == password;
            }
            catch
            {
                return false;
            }
        }

        public static Customer SearchByPhone(string phone, string prePhone)
        {
            try
            {
                var customer = ModelContext.Instance.Customers
                    .Single(b => b.CustomerPhone == phone && b.CustomerPrephone == prePhone);
                return customer;
            }
            catch
            {
                return null;
            }

        }

        public static Customer SearchByEmail(string email)
        {
            try
            {
                var customer = ModelContext.Instance.Customers
                    .Single(b => b.CustomerEmail == email);
                return customer;
            }
            catch
            {
                return null;
            }

        }
        
        [HttpGet]
        public static Customer SearchById(int id)
        {
            try
            {
                var customer = ModelContext.Instance.Customers
                    .Single(b => b.CustomerId == id);
                return customer;
            }
            catch
            {
                return null;
            }

        }
    }

}
