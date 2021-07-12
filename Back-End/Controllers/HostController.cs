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
using Microsoft.Extensions.Primitives;


//简单测试
namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HostController : ControllerBase
    {
        //GET: api/<Host>

        [HttpDelete("DelById")]
        public bool DelById(int id)
        {
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
            catch (Exception e)
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
            catch (Exception e)
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
                if (host.HostPassword == Request.Form["password"])
                {
                    host.HostPassword = Request.Form["newpassword"];
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
        }
        public static Host SearchById(int id)
        {
            try
            {
                var host = ModelContext.Instance.Hosts
                    .Single(b => b.HostId == id);
                return host;
            }
            catch
            {
                return null;
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
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }

        }

        public static Host SearchByPhone(string phone, string prePhone)
        {
            try
            {
                var host = ModelContext.Instance.Hosts
                    .Single(b => b.HostPhone == phone && b.HostPrephone == prePhone);
                return host;
            }
            catch
            {
                return null;
            }
        }

        public static bool HostLogin(Host host, string password)
        {
            try
            {
                if (host == null)
                {
                    return false;
                }
                return host.HostPassword == password;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost("phone")]
        public string CheckHostPhoneRegisitered()
        {
            CheckPhoneMessage checkPhoneMessage = new CheckPhoneMessage();
            string phone = Request.Form["phonenumber"];
            string prePhone = Request.Form["prenumber"];
            if (phone != null && prePhone != null)
            {
                checkPhoneMessage.errorCode = 200;
                if (SearchByPhone(phone, prePhone) == null)
                {
                    checkPhoneMessage.data["phoneunique"] = true;
                }
            }
            return checkPhoneMessage.ReturnJson();
        }

        [HttpPost("changepassword")]
        public string ChangeCustomerPassword()
        {
            ChangePasswordMessage message = new ChangePasswordMessage();
            string phone = Request.Form["phone"];
            string preNumber = Request.Form["prenumber"];
            string password = Request.Form["password"];
            if (phone != null && preNumber != null)
            {
                var host = SearchByPhone(phone, preNumber);
                if (password != null)
                {
                    message.errorCode = 200;
                    host.HostPassword = password;
                    try
                    {
                        ModelContext.Instance.SaveChanges();
                        message.data["changestate"] = true;
                    }
                    catch
                    {

                    }

                }
            }
            else
            {
                StringValues token = default(StringValues);
                if (Request.Headers.TryGetValue("token", out token))
                {
                    message.errorCode = 300;
                    var data = Token.VerifyToken(token);
                    if (data != null)
                    {
                        ModelContext.Instance.DetachAll();
                        int id = int.Parse(data["id"]);
                        var host = SearchById(id);
                        if (password != null)
                        {
                            message.errorCode = 200;
                            host.HostPassword = password;
                            try
                            {
                                ModelContext.Instance.SaveChanges();
                                message.data["changestate"] = true;
                            }
                            catch
                            {

                            }
                        }
                    }
                }
            }
            return message.ReturnJson();
        }


    }
}
