using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Back_End.Contexts;
using Back_End.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        [HttpPost("customer")]
        public string CuntomerLoginByPhone()
        {
            LoginMessage loginMessage = new LoginMessage();
            string phone = Request.Form["phonenumber"]; //接受 Form 提交的数据
            string password = Request.Form["password"];
            string preNumber = Request.Form["prenumber"];
            Customer customer = CustomerController.SearchByPhone(phone, preNumber);
            if (CustomerController.CustomerLogin(customer, password))
            {
                loginMessage.data["loginState"] = true;
                loginMessage.data["userName"] = customer.CustomerName;
                loginMessage.data["userAvatar"] = customer.CustomerPhoto;
                loginMessage.errorCode = 200;
                loginMessage.msg = loginMessage.msgType[1];

                var token = Token.GetToken(new TokenInfo()
                {
                    id = customer.CustomerId.ToString(),
                    phone = phone,
                    password = password,
                    preNumber = preNumber,
                });
                loginMessage.data.Add("token", token);
                //Response.Cookies.Append("Token", token, new CookieOptions() { Path = "/", HttpOnly=true});
            }

            return loginMessage.ReturnJson();
        }

    }
}
