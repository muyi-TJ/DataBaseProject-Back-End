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
using Microsoft.EntityFrameworkCore;


//简单测试
namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HostController : ControllerBase
    {
        //GET: api/<Host>
        private readonly ModelContext myContext;
        public HostController(ModelContext modelContext)
        {
            myContext = modelContext;
        }

        public static Host SearchById(int id)
        {
            try
            {
                ModelContext modelContext = new ModelContext();
                var host = modelContext.Hosts
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
                var host = myContext.Hosts
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
                ModelContext modelContext = new ModelContext();
                var host = modelContext.Hosts
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
                myContext.Entry(host).State = EntityState.Unchanged;
                if (password != null)
                {
                    message.errorCode = 200;
                    host.HostPassword = password;
                    try
                    {
                        myContext.SaveChanges();
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
                        myContext.DetachAll();
                        int id = int.Parse(data["id"]);
                        var host = SearchById(id);
                        myContext.Entry(host).State = EntityState.Unchanged;
                        if (password != null)
                        {
                            message.errorCode = 200;
                            host.HostPassword = password;
                            try
                            {
                                myContext.SaveChanges();
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
