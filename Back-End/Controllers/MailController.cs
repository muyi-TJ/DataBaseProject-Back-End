using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Back_End.Contexts;
using Back_End.Models;
using Microsoft.AspNetCore.Http;

namespace Back_End.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MailController : ControllerBase
    {

        //GET: api/<VerficateMail>
        [HttpPost]
        public string VerficateMail()
        {
            string email = Request.Form["email"];
            if (!CheckMail(email))
                return null;
            string VCcode = GetMailCode(6);
            SendMailMessage("342472121@qq.com", email, "收一下验证码", VCcode, "bvwmqmhltgwsbhbb");
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Path = "/";
            cookieOptions.HttpOnly = true;
            cookieOptions.MaxAge = new TimeSpan(0, 10, 0);
            Response.Cookies.Append("VCcode", MD5Helper.EncryptString(VCcode), cookieOptions);
            return MD5Helper.EncryptString(VCcode);
        }

        public static string GetMailCode(int codeLength)
        {
            int randNum;
            char code;
            string randomCode = String.Empty;

            for (int i = 0; i < codeLength; i++)
            {
                byte[] buffer = Guid.NewGuid().ToByteArray();
                int seed = BitConverter.ToInt32(buffer, 0);
                Random random = new Random(seed);
                randNum = random.Next();

                if (randNum % 3 == 1)
                    code = (char)('A' + (char)(randNum % 26));
                else if (randNum % 3 == 2)
                    code = (char)('a' + (char)(randNum % 26));
                else
                    code = (char)('0' + (char)(randNum % 10));
                randomCode += code.ToString();
            }
            return randomCode;
        }

        public static bool SendMailMessage(string myEmailAddress, string recEmailAddress, string subject, string body, string authorizationCode)
        {

            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(myEmailAddress);//发件人邮箱地址
            mail.To.Add(new MailAddress(recEmailAddress));//收件人邮箱地址
            mail.Subject = subject;//邮件标题
            mail.Body = body;  //邮件内容  
            mail.Priority = MailPriority.High;//优先级

            SmtpClient client = new SmtpClient();//qq邮箱:smtp.qq.com；126邮箱:smtp.126.com              
            client.Host = "smtp.qq.com";
            client.Port = 587;//SMTP端口465或587
            client.EnableSsl = true;//使用安全加密SSL连接  
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Credentials = new NetworkCredential(myEmailAddress, authorizationCode);//验证发件人身份(发件人邮箱，邮箱授权码);                   

            try
            {
                client.Send(mail);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            return true;
        }

        public static bool CheckMail(string mail)
        {
            string str = @"^[1-9][0-9]{4,}@qq.com$";
            Regex mReg = new Regex(str);

            if (mReg.IsMatch(mail))
                return true;
            return false;
        }
    }

    public static class MD5Helper
    {
        public static string EncryptString(string str)
        {
            MD5 md5 = MD5.Create();
            // 将字符串转换成字节数组
            byte[] byteOld = Encoding.UTF8.GetBytes(str);
            // 调用加密方法
            byte[] byteNew = md5.ComputeHash(byteOld);
            // 将加密结果转换为字符串
            StringBuilder sb = new StringBuilder();
            foreach (byte b in byteNew)
            {
                // 将字节转换成16进制表示的字符串，
                sb.Append(b.ToString("x2"));
            }
            // 返回加密的字符串
            return sb.ToString();
        }
    }
}