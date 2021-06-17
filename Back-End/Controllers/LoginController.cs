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
            public bool loginState { set; get; } = false;
            public string userName { set; get; } = null;
            public string userAvatar { set; get; } = null;
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
                loginMessage.loginState = true;
                loginMessage.userName = customer.CustomerName;
                loginMessage.userAvatar = customer.CustomerPhoto;
            }
            
            return JsonSerializer.Serialize(loginMessage);
        }

    }
}
