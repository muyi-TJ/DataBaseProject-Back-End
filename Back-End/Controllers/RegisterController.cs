using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back_End.Contexts;
using System.Text.Json;
using Back_End.Models;

namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegisterController : ControllerBase
    {
        private readonly ModelContext myContext;
        public RegisterController(ModelContext modelContext)
        {
            myContext = modelContext;
        }
        [HttpPost("customer")]
        public string CustomerRegister()
        {
            RegisterMessage registerMessage = new RegisterMessage();
            try
            {
                myContext.DetachAll();
                Customer customer = new Customer();
                customer.CustomerName = Request.Form["username"];
                customer.CustomerPassword = Request.Form["password"];
                customer.CustomerPrephone = Request.Form["prenumber"];
                customer.CustomerPhone = Request.Form["phonenumber"];
                customer.CustomerCreatetime = DateTime.Now;
                customer.CustomerLevel = 1;
                myContext.Add(customer);
                myContext.SaveChanges();
                registerMessage.errorCode = 200;
                registerMessage.data["registerState"] = true;


                // 添加默认收藏夹
                Favorite favorite = new Favorite() {
                    CustomerId = customer.CustomerId,
                    Name = "默认收藏夹"
                };
                myContext.Add(favorite);
                myContext.SaveChanges();
            }
            catch
            {

            }
            return registerMessage.ReturnJson();

        }

        [HttpPost("host")]
        public string HostRegister()
        {
            RegisterMessage registerMessage = new RegisterMessage();
            try
            {
                myContext.DetachAll();
                Host host = new Host();
                host.HostUsername = Request.Form["username"];
                host.HostPassword = Request.Form["password"];
                host.HostPrephone = Request.Form["prenumber"];
                host.HostPhone = Request.Form["phonenumber"];
                host.HostRealname = Request.Form["truename"];
                host.HostIdnumber = Request.Form["ID"];
                host.HostGender = Request.Form["gender"].ToString().ToUpper();
                host.HostCreateTime = DateTime.Now;
                host.HostLevel = 1;
                myContext.Add(host);
                myContext.SaveChanges();
                registerMessage.errorCode = 200;
                registerMessage.data["registerState"] = true;
            }
            catch
            {

            }
            return registerMessage.ReturnJson();

        }

    }
}
