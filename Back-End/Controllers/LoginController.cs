using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Back_End.Contexts;
using Back_End.Models;
using System.Text.Json;


namespace Back_End.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class LoginController : ControllerBase
    {
        public class LoginMessage
        {
            public int errorCode { get; set; }
            public Dictionary<string, dynamic> data { get; set; }
        }


        [HttpPost("customer")]
        public string CuntomerLoginByPhone()
        {
            LoginMessage loginMessage = new LoginMessage();
            string phone = Request.Form["phonenumber"]; //接受 Form 提交的数据
            string password = Request.Form["password"];
            string preNumber = Request.Form["prenumber"];
            Customer customer = CustomerController.SearchByPhone(phone, preNumber);
            if(CustomerController.CustomerLogin(customer,password))
            {
                loginMessage.data.Add("loginState",true);
                loginMessage.data.Add("userName", customer.CustomerName);
                loginMessage.data.Add("userAvatar", customer.CustomerPhoto);
            }
            
            return JsonSerializer.Serialize(loginMessage);
        }

    }
}
