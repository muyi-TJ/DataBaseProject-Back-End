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
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.IO;
using System.Web;




namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/phone/sendmessage")]
    public class VCcodeController : ControllerBase
    {
        private static readonly string url = "http://www.etuocloud.com/gatetest.action";
        private static readonly string appKey = "vxdunoQ8hwJPBQoUcqz2rfEzpN9D7agz";
        private static readonly string appSecret = "ClPdmYAiPeky7acZdlwvbUljG6uOIaJ13JiQOqQveCAtC89Ybr3o73UPV71tNdj1";
        private static readonly string format = "json";

        [HttpPost]
        public string SendCode()
        {
            string code = InitialCode(4);
            string phone = Request.Form["phonenumber"];
            VCcodeMessage message = new VCcodeMessage();
            if (phone != null)
            {
                NameValueCollection parameters = new NameValueCollection();
                parameters.Add("app_key", appKey);
                parameters.Add("view", format);
                parameters.Add("method", "cn.etuo.cloud.api.sms.simple");
                parameters.Add("to", phone);
                parameters.Add("template", "1");
                parameters.Add("smscode", code);
                parameters.Add("sign", getsign(parameters));
                Console.WriteLine(code);
                HttpClient.HttpPost(url, parameters);
                message.errorCode = 200;
                message.data["sendstate"] = true;
            }
            return message.ReturnJson();
        }


        private string InitialCode(int length = 4)
        {
            string VCcode = "";
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                VCcode += random.Next(0, 9).ToString();
            }
            MD5 md5 = MD5.Create();
            byte[] str = md5.ComputeHash(Encoding.UTF8.GetBytes(VCcode));
            CookieOptions cookieOptions = new CookieOptions();
            cookieOptions.Path = "/";
            cookieOptions.HttpOnly = true;
            cookieOptions.MaxAge = new TimeSpan(0, 10, 0);
            Response.Cookies.Append("code", Convert.ToBase64String(str), cookieOptions);
            return VCcode;
        }

        //实现自网站api示例
        private static string getsign(NameValueCollection parameters)
        {
            SortedDictionary<string, string> sParams = new SortedDictionary<string, string>();
            foreach (string key in parameters.Keys)
            {
                sParams.Add(key, parameters[key]);
            }

            string sign = string.Empty;
            StringBuilder codedString = new StringBuilder();
            foreach (KeyValuePair<string, string> temp in sParams)
            {
                if (temp.Value == "" || temp.Value == null || temp.Key.ToLower() == "sign")
                {
                    continue;
                }

                if (codedString.Length > 0)
                {
                    codedString.Append("&");
                }
                codedString.Append(temp.Key.Trim());
                codedString.Append("=");
                codedString.Append(temp.Value.Trim());
            }

            // 应用key
            codedString.Append(appSecret);
            string signkey = codedString.ToString();
            sign = GetMD5(signkey, "utf-8");

            return sign;

        }

        //实现自网站api示例
        private static string GetMD5(string encypStr, string charset)
        {
            string retStr;
            MD5CryptoServiceProvider m5 = new MD5CryptoServiceProvider();

            //创建md5对象
            byte[] inputBye;
            byte[] outputBye;

            //使用XXX编码方式把字符串转化为字节数组．
            try
            {
                inputBye = Encoding.GetEncoding(charset).GetBytes(encypStr);
            }
            catch (Exception)
            {
                inputBye = System.Text.Encoding.UTF8.GetBytes(encypStr);
            }
            outputBye = m5.ComputeHash(inputBye);

            retStr = System.BitConverter.ToString(outputBye);
            retStr = retStr.Replace("-", "").ToUpper();

            //  return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(ConvertString, "MD5").ToLower(); ;

            return retStr;
        }

    }

    //实现自网站api示例
    public class HttpClient
    {
        /// <summary>
        /// POST请求与获取结果  
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static string HttpPost(string Url, NameValueCollection parameters)
        {
            return HttpPost(Url, toParaData(parameters));
        }



        //调用http接口,接口编码为utf-8
        private static string toParaData(NameValueCollection parameters)
        {

            //设置参数，并进行URL编码
            StringBuilder codedString = new StringBuilder();
            foreach (string key in parameters.Keys)
            {
                // codedString.Append(HttpUtility.UrlEncode(key));
                codedString.Append(key);
                codedString.Append("=");
                codedString.Append(HttpUtility.UrlEncode(parameters[key], System.Text.Encoding.UTF8));
                codedString.Append("&");
            }
            string paraUrlCoded = codedString.Length == 0 ? string.Empty : codedString.ToString().Substring(0, codedString.Length - 1);


            return paraUrlCoded;
        }


        /// <summary>  
        /// POST请求与获取结果  
        /// </summary>  
        public static string HttpPost(string Url, string postDataStr)
        {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";

            //request.ContentLength = postDataStr.Length;
            //StreamWriter writer = new StreamWriter(request.GetRequestStream(), System.Text.Encoding.UTF8);
            // writer.Write(postDataStr);
            // writer.Flush();


            //将URL编码后的字符串转化为字节
            byte[] payload = System.Text.Encoding.UTF8.GetBytes(postDataStr);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();

            //获得响应流
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string encoding = response.ContentEncoding;
            if (encoding == null || encoding.Length < 1)
            {
                encoding = "UTF-8"; //默认编码  
            }
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding(encoding));

            string retString = reader.ReadToEnd();
            return retString;
        }



    }
}


