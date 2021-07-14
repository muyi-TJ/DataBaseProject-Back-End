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

namespace Back_End.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase {
        [HttpPost("customer")]
        public string CuntomerLoginByPhone() {
            LoginMessage loginMessage = new LoginMessage();
            string phone = Request.Form["phonenumber"]; //接受 Form 提交的数据
            string password = Request.Form["password"];
            string preNumber = Request.Form["prenumber"];
            if (phone != null && password != null && preNumber != null) {
                loginMessage.errorCode = 200;
            }
            Customer customer = CustomerController.SearchByPhone(phone, preNumber);
            if (CustomerController.CustomerLogin(customer, password)) {
                loginMessage.data["loginState"] = true;
                loginMessage.data["userName"] = customer.CustomerName;
                loginMessage.data["userAvatar"] = customer.CustomerPhoto;

                var token = Token.GetToken(new TokenInfo() {
                    id = customer.CustomerId.ToString(),
                    phone = phone,
                    password = password,
                    preNumber = preNumber,
                });
                loginMessage.data.Add("token", token);
                //Response.Headers.Add("Access-Control-Expose-Headers", "Token");
                //Response.Headers.Add("Token", token);
                //Response.Cookies.Append("Token", token, new CookieOptions() { Path = "/", HttpOnly=true});
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Path = "/";
                cookieOptions.HttpOnly = false;
                cookieOptions.SameSite = SameSiteMode.None;
                cookieOptions.Secure = true;
                cookieOptions.MaxAge = new TimeSpan(0, 10, 0);
                Response.Cookies.Append("Token", token, cookieOptions);
            }
            var request = Request;
            return loginMessage.ReturnJson();
        }

        [HttpPost("host")]
        public string HostLoginByPhone() {
            LoginMessage loginMessage = new LoginMessage();
            string phone = Request.Form["phonenumber"]; //接受 Form 提交的数据
            string password = Request.Form["password"];
            string preNumber = Request.Form["prenumber"];
            if (phone != null && password != null && preNumber != null) {
                loginMessage.errorCode = 200;
            }
            Host host = HostController.SearchByPhone(phone, preNumber);
            if (HostController.HostLogin(host, password)) {
                loginMessage.data["loginState"] = true;
                loginMessage.data["userName"] = host.HostUsername;
                loginMessage.data["userAvatar"] = host.HostAvatar;

                var token = Token.GetToken(new TokenInfo() {
                    id = host.HostId.ToString(),
                    phone = phone,
                    password = password,
                    preNumber = preNumber,
                });
                loginMessage.data.Add("token", token);
                //Response.Headers.Add("Access-Control-Expose-Headers", "Token");
                //Response.Headers.Add("Token", token);
                //Response.Cookies.Append("Token", token, new CookieOptions() { Path = "/", HttpOnly=true});
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Path = "/";
                cookieOptions.HttpOnly = false;
                cookieOptions.SameSite = SameSiteMode.Lax;
                cookieOptions.MaxAge = new TimeSpan(0, 10, 0);
                Response.Cookies.Append("Token", token, cookieOptions);
            }

            return loginMessage.ReturnJson();
        }

        [HttpPost("administrator")]
        public string AdminLoginByName() {
            LoginMessage loginMessage = new LoginMessage();
            string adminName = Request.Form["adminName"];
            string password = Request.Form["password"];
            if (adminName != null && password != null) {
                loginMessage.errorCode = 200;
            }
            Administrator admin = AdministratorController.SearchByName(adminName);
            if (AdministratorController.AdminLoginByName(admin, password)) {
                loginMessage.data["loginState"] = true;
                loginMessage.data["userName"] = admin.AdminUsername;
                loginMessage.data["userAvatar"] = admin.AdminAvatar;
                var token = Token.GetToken(new TokenInfo() {
                    id = admin.AdminId.ToString(),
                    password = password,
                });
                loginMessage.data.Add("token", token);
                //Response.Headers.Add("Access-Control-Expose-Headers", "Token");
                //Response.Headers.Add("Token", token);
                //Response.Cookies.Append("Token", token, new CookieOptions() { Path = "/", HttpOnly=true});
                CookieOptions cookieOptions = new CookieOptions();
                cookieOptions.Path = "/";
                cookieOptions.HttpOnly = false;
                cookieOptions.SameSite = SameSiteMode.Lax;
                cookieOptions.MaxAge = new TimeSpan(0, 10, 0);
                Response.Cookies.Append("Token", token, cookieOptions);

            }
            return loginMessage.ReturnJson();
        }

    }
}
