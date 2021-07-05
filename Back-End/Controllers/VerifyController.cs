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



namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/verifycode")]
    public class VerifyController : ControllerBase
    {
        [HttpGet]
        public string GetVerifyCode()
        {
            VerifyControllerMessage message = new VerifyControllerMessage();
            string code = MakeCode(4);
            MD5 md5 = MD5.Create();
            byte[] str = md5.ComputeHash(Encoding.UTF8.GetBytes(code));
            string image = CreateCodeImg(code);
            if (image != null)
            {
                message.errorCode = 200;
                message.data["verifycode"] = str;
                message.data["codeimg"] = image;
            }
            return message.ReturnJson();

        }



        public string MakeCode(int codelen)
        {
            int number;
            StringBuilder code = new StringBuilder();
            Random random = new Random();
            for (int index = 0; index < codelen; index++)
            {
                number = random.Next();
                if (number % 2 == 0)
                {
                    code.Append((char)('0' + (char)((number + index) % 10)));//生成随机数字
                }
                else
                {
                    code.Append((char)('A' + (char)((number + index) % 26)));//生成随机字母
                }
            }
            return code.ToString();
        }


        public string CreateCodeImg(string CheckCode)
        {
            if (string.IsNullOrEmpty(CheckCode))
            {
                return null;
            }
            Bitmap image = new Bitmap((int)Math.Ceiling((CheckCode.Length * 12.5)), 22);
            Graphics graphic = Graphics.FromImage(image);//创建一个验证码图片
            try
            {
                Random random = new Random();
                graphic.Clear(Color.White);
                int x1 = 0, y1 = 0, x2 = 0, y2 = 0;
                for (int index = 0; index < 25; index++)
                {
                    x1 = random.Next(image.Width);
                    x2 = random.Next(image.Width);
                    y1 = random.Next(image.Height);
                    y2 = random.Next(image.Height);
                    graphic.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }
                Font font = new Font("Arial", 12, (FontStyle.Bold | FontStyle.Italic));//Font设置字体，字号，字形
                //设置图形渐变色的起始颜色与终止颜色，渐变角度
                LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Red, Color.DarkRed, 1.2f, true);
                graphic.DrawString(CheckCode, font, brush, 2, 2);
                int X = 0; int Y = 0;
                //绘制图片的前景噪点
                for (int i = 0; i < 100; i++)
                {
                    X = random.Next(image.Width);
                    Y = random.Next(image.Height);
                    image.SetPixel(X, Y, Color.FromArgb(random.Next()));
                }
                //画图片的边框线
                graphic.DrawRectangle(new Pen(Color.Silver), 0, 0, image.Width - 1, image.Height - 1);
                MemoryStream ms = new MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                byte[] arr = new byte[ms.Length];
                ms.Position = 0;
                ms.Read(arr, 0, (int)ms.Length);
                ms.Close();
                string strbaser64 = Convert.ToBase64String(arr);
                byte[] arr1 = { 123 };
                Console.Write(Convert.ToBase64String(arr1));
                return strbaser64;
            }
            finally
            {
                graphic.Dispose();
                image.Dispose();
            }
        }
    }
}
