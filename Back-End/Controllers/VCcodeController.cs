using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back_End.Contexts;
using System.Text.Json;
using Back_End.Models;
using Microsoft.AspNetCore.Http;


namespace Back_End
{
    [ApiController]
    [Route("[controller]")]
    public class VCcodeController : ControllerBase
    {
        [HttpPost]
        public bool SendCode()
        {
            Console.WriteLine(InitialCode(4));
            return true;
        }


        private string InitialCode(int length = 4)
        {
            string VCcode = "";
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                VCcode += random.Next(0, 9).ToString();
            }

            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Path = "/";
            cookieOptions.HttpOnly = true;
            cookieOptions.MaxAge = new TimeSpan(0, 10, 0);
            Response.Cookies.Append("VCcode", VCcode, cookieOptions);
            return VCcode;
        }
    }
}
/*
namespace Back_End
{
    public class VCcode
    {
        private string userId;
        private string userKey;
        private void getCode(HttpContext context)
        {
            //从http://sms.webchinese.cn/申请账号，获得密钥
            string vc = "";//生成4位随机数码做为验证码
            Random r = new Random();
            int num1 = r.Next(0, 9);
            int num2 = r.Next(0, 9);
            int num3 = r.Next(0, 9);
            int num4 = r.Next(0, 9);

            int[] numbers = new int[4] { num1, num2, num3, num4 };
            for (int i = 0; i < numbers.Length; i++)
            {
                vc += numbers[i].ToString();
            }

            string number = "15805910204";//接受短信的手机号
            string smsText = "您的验证码为" + vc + "。";//【重走霞客路】

            string postUrl = GetPostUrl(number, smsText);
            string result = PostSmsInfo(postUrl);
            string t = GetResult(result);

            HttpCookie mycookie = new HttpCookie("getCode");
            mycookie.Values.Add("VCode", vc);//把验证码放到COOKIE里
            TimeSpan ts = new TimeSpan(3, 0, 30, 0);
            DateTime dt = DateTime.Now;
            mycookie.Expires = dt.Add(ts);
            context.Response.AppendCookie(mycookie);

        }
        //示例http://localhost:2446/WebService.ashx?action=userLoginmobile=13888888888&VCode=name01

        public string GetPostUrl(string smsMob, string smsText)
        {

            //uid为用户名,key为密钥
            string postUrl = "http://utf8.api.smschinese.cn/?Uid=" + userId + "&key=" + userKey + "&smsMob=" + smsMob + "&smsText=" + smsText;
            return postUrl;
        }
        public string PostSmsInfo(string url)
        {
            string strRet = null;
            if (url == null || url.Trim().ToString() == "")
            {
                return strRet;
            }
            string targeturl = url.Trim().ToString();
            try
            {
                HttpWebRequest hr = (HttpWebRequest)WebRequest.Create(targeturl);
                hr.UserAgent = "Mozilla/4.0(compatible;MISE 6.0;Window NT 5.1)";
                hr.Method = "GET";
                hr.Timeout = 30 * 60 * 1000;
                WebResponse hs = hr.GetResponse();
                Stream sr = hs.GetResponseStream();
                StreamReader ser = new StreamReader(sr, Encoding.Default);
                strRet = ser.ReadToEnd();
            }
            catch (Exception ex)
            {
                strRet = null;
            }
            return strRet;
        }

        public string GetResult(string strRet)
        {
            int result = 0;
            try
            {
                result = int.Parse(strRet);
                switch (result)
                {
                    case -1:
                        strRet = "没有该用户账户";
                        break;
                    case -2:
                        strRet = "接口密钥不正确，不是账户登陆密码";
                        break;
                    case -21:
                        strRet = "MDS接口密钥加密不正确";
                        break;
                    case -3:
                        strRet = "短信数量不足";
                        break;
                    case -11:
                        strRet = "该用户被禁用";
                        break;
                    case -14:
                        strRet = "短信内容出现非法字符";
                        break;
                    case -4:
                        strRet = "手机格式不正确";
                        break;
                    case -41:
                        strRet = "手机号码为空";
                        break;
                    case -42:
                        strRet = "短信内容为空";
                        break;
                    case -51:
                        strRet = "短信签名格式不正确，接口签名格式为：【签名内容】";
                        break;
                    case -6:
                        strRet = "IP限制";
                        break;
                    default:
                        strRet = "发送短信数量：" + result;
                        break;
                }
            }
            catch (Exception ex)
            {
                strRet = ex.Message;
            }
            //TextBox1.Text = strRet.ToString();
            return strRet;
        }
    }
}
*/