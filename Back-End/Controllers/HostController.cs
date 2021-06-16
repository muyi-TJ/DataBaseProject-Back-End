using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Text.RegularExpressions;
using Back_End.Contexts;
using Back_End.Models;
using Microsoft.AspNetCore.Http;


//简单测试
namespace Back_End.Controllers {
    [ApiController]
    [Route("[controller]")]
    public class HostController : ControllerBase {
        //GET: api/<Host>

        [HttpDelete("DelById")]
        public bool DelById(int id) {
            if (id < 0)
                return false;
            try
            {
                var context = ModelContext.Instance;
                context.DetachAll();
                Host host = new Host() { HostId = id };
                context.Hosts.Attach(host);
                context.Hosts.Remove(host);
                context.SaveChanges();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
        
        [HttpPost("Login")]
        public bool Login()
        {
            string phone = Request.Form["phone"]; //接受 Form 提交的数据
            string email = Request.Form["email"];
            string password = Request.Form["password"];
            string prePhone = Request.Form["prePhone"];
            var context = ModelContext.Instance;
            Host host;
            try
            {
                if (email == null)
                {
                    host = context.Hosts
                        .Single(b => b.HostPhone == phone && b.HostPrephone == prePhone);
                }
                else
                {
                    host = context.Hosts
                        .Single(b => b.HostEmail == email);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return host.HostPassword == password;
        }

        [HttpPost("Register")]
        public bool Register()
        {
            Host host = new Host();
            host.HostUsername = Request.Form["name"];
            host.HostPassword = Request.Form["password"];
            host.HostPrephone = Request.Form["prePhone"];
            host.HostPhone = Request.Form["phone"];
            host.HostEmail = Request.Form["email"];
            host.HostIdnumber = Request.Form["idNumber"];
            host.HostRealname = Request.Form["realName"];
            host.HostGender = Request.Form["gender"];
            host.HostCreateTime = DateTime.Now;
            try
            {
                var context = ModelContext.Instance;
                context.Add(host);
                context.SaveChanges();
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
        
        [HttpPut("ChangePassword")]
        public bool ChangePassword()
        {
            try
            {
                var context = ModelContext.Instance;
                context.DetachAll();
                int id = int.Parse(Request.Form["id"]);
                Host host = context.Hosts
                    .Single(b => b.HostId == id);
                if(host.HostPassword == Request.Form["password"])
                {
                    host.HostPassword = Request.Form["newpassword"];
                    return true;
                }
                return false;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }

        
        [HttpGet("GetById")]
        public string GetById(int id)
        {
            try
            {
                var host = ModelContext.Instance.Hosts
                    .Single(b => b.HostId == id);
                return JsonSerializer.Serialize(host);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

        }
    }
}
