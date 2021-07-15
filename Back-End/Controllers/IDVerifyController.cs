using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back_End.Contexts;
using System.Text.Json;
using Back_End.Models;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;

namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IDVerifyController : ControllerBase
    {


        [HttpPost]
        public string IDVerify()
        {
            IDVerifyMessage message = new IDVerifyMessage();
            try
            {
                string appcode = "443d03aefd5c4e4f87be752c72ebafae";
                string img_file = Request.Form["positivePhoto"];
                //face:表示正面；back：表示反面
                string config = "{\"side\": \"face\",\"ignore_exif\": \"false\"}";

                string method = "POST";
                string url = "https://dm-51.data.aliyun.com/rest/160601/ocr/ocr_idcard.json";

                string bodys;
                if (img_file.StartsWith("http"))
                {
                    bodys = "{\"image\":\"" + img_file + "\",\"configure\":" + config + "}";
                }
                else
                {
                    string base64 = img_file;
                    bodys = "{\"image\":\"" + base64 + "\",\"configure\":" + config + "}";
                };

                HttpWebRequest httpRequest = null;
                HttpWebResponse httpResponse = null;
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                httpRequest = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                httpRequest.Method = method;
                httpRequest.Headers.Add("Authorization", "APPCODE " + appcode);
                //根据API的要求，定义相对应的Content-Type
                httpRequest.ContentType = "application/json; charset=UTF-8";

                if (0 < bodys.Length)
                {
                    byte[] data = Encoding.UTF8.GetBytes(bodys);
                    using (Stream stream = httpRequest.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                }
                try
                {
                    httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                }
                catch (WebException ex)
                {
                    httpResponse = (HttpWebResponse)ex.Response;
                }
                Stream st = httpResponse.GetResponseStream();
                StreamReader reader = new StreamReader(st);
                string str = reader.ReadToEnd();

                var dict = JsonConvert.DeserializeObject<Dictionary<dynamic, dynamic>>(str);

                var context = new ModelContext();
                context.DetachAll();
                string idNumber = dict["num"];
                if (!context.Hosts.Any(b => b.HostIdnumber == idNumber))
                {
                    message.errorCode = 200;
                    message.data["verifyResult"] = 2;
                    message.data["trueName"] = dict["name"];
                    message.data["trueID"] = dict["num"];
                    return message.ReturnJson();
                }
                else
                {
                    message.errorCode = 200;
                    message.data["verifyResult"] = 1;
                    message.data["trueName"] = dict["name"];
                    message.data["trueID"] = dict["num"];
                    return message.ReturnJson();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                message.errorCode = 200;
                message.data["verifyResult"] = 0;
                return message.ReturnJson();
            }
        }




        public static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }
}
