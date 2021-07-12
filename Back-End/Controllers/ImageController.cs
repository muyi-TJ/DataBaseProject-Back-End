using System;
using System.IO;
using Aliyun.OSS;
using System.Text.RegularExpressions;

namespace Back_End
{
    public class PhotoUpload
    {
        public static string UploadPhoto(string ImgBase64, string objectPath)
        {
            // yourEndpoint填写Bucket所在地域对应的Endpoint。以华东1（杭州）为例，Endpoint填写为https://oss-cn-hangzhou.aliyuncs.com。
            var endpoint = "https://oss-cn-shanghai.aliyuncs.com";
            var accessKeyId = "LTAI5tScrNCoU2hhA8A7D67e";
            var accessKeySecret = "45fK4YI50AK6efhWLBcOP69ytN2XWp";
            var bucketName = "guisu";

            try
            {
                //string ImgBase64 = Request.Form["base"];
                string pattern = @"^data:image/(?<type>\w+);";
                string type = Regex.Matches(ImgBase64, pattern)[0].Groups["type"].Value;
                string objectName = objectPath + "." + type;

                byte[] arr = Convert.FromBase64String(ImgBase64.Split(',')[1]);//.Split(',')[1]
                MemoryStream ms = new MemoryStream(arr);

                // 创建OssClient实例。
                var client = new OssClient(endpoint, accessKeyId, accessKeySecret);


                var res = client.PutObject(bucketName, objectName, ms, new ObjectMetadata() { ContentType = "image/" + type });
                string imgurl = "https://guisu.oss-cn-shanghai.aliyuncs.com/" + objectName;

                Console.WriteLine("Put object succeeded");
                return imgurl;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Put object failed, {0}", ex.Message);
                return null;
            }
        }
    }
}
