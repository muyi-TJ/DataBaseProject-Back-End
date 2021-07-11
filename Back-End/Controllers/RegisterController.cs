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
        [HttpPost("customer")]
        public string CustomerRegister()
        {
            RegisterMessage registerMessage = new RegisterMessage();
            try
            {
                ModelContext.Instance.DetachAll();
                Customer customer = new Customer();
                customer.CustomerName = Request.Form["username"];
                customer.CustomerPassword = Request.Form["password"];
                customer.CustomerPrephone = Request.Form["prenumber"];
                customer.CustomerPhone = Request.Form["phonenumber"];
                customer.CustomerCreatetime = DateTime.Now;
                ModelContext.Instance.Add(customer);
                ModelContext.Instance.SaveChanges();
                registerMessage.errorCode = 200;
                registerMessage.data["registerState"] = true;
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
                ModelContext.Instance.DetachAll();
                Host host = new Host();
                host.HostUsername = Request.Form["username"];
                host.HostPassword = Request.Form["password"];
                host.HostPrephone = Request.Form["prenumber"];
                host.HostPhone = Request.Form["phonenumber"];
                host.HostRealname = Request.Form["truename"];
                host.HostIdnumber = Request.Form["ID"];
                host.HostGender = Request.Form["gender"].ToString().ToUpper();
                host.HostCreateTime = DateTime.Now;
                ModelContext.Instance.Add(host);
                ModelContext.Instance.SaveChanges();
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
