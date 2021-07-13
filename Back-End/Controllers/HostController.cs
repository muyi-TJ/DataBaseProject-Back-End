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
using System.Collections.Generic;
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

        [HttpGet("hostInfo")]
        public string GetHostInfo() {
            GetHostInfoMessage message = new GetHostInfoMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    try {
                        //ModelContext context = new ModelContext();
                        int hostId = int.Parse(data["id"]);
                        var host = myContext.Hosts.Single(b => b.HostId == hostId);

                        message.data["hostAvatar"] = host.HostAvatar;
                        message.data["hostNickName"] = host.HostUsername;
                        message.data["hostRealName"] = host.HostRealname;
                        message.data["hostSex"] = host.HostGender == "M" ? "男" : "女";
                        message.data["hostLevel"] = host.HostLevel == null ? null : host.HostLevel;
                        message.data["hostLevelName"] = host.HostLevel == null ? null : host.HostLevelNavigation.HostLevelName;
                        int commentNum = 0, commentScore = 0;
                        int publishedNum = 0, unpublishedNum = 0, pendingReviewNum = 0;
                        foreach (var stay in host.Stays) {
                            commentNum += (int)stay.CommentNum;
                            commentScore += (int)stay.CommentScore;
                            if (stay.StayStatus == 0)
                                unpublishedNum += 1;
                            else if (stay.StayStatus == 1)
                                pendingReviewNum += 1;
                            else if (stay.StayStatus == 2)
                                pendingReviewNum += 1;
                        }
                        message.data["hostScore"] = host.HostScore;
                        message.data["publishedNum"] = publishedNum;
                        message.data["unpublishedNum"] = unpublishedNum;
                        message.data["pendingReviewNum"] = pendingReviewNum;
                        message.data["reviewNum"] = commentNum;
                        message.data["emailTag"] = host.HostEmail == null ? false : true;
                        message.data["phoneTag"] = host.HostPhone == null ? false : true;
                        message.data["authenticationTag"] = true;
                        message.data["hostCreateTime"] = host.HostCreateTime;
                        message.data["averageRate"] = commentNum == 0 ? 0 : ((float)commentScore / (float)commentNum);
                        message.data["unpublishedStayInfo"] = new List<Dictionary<string, dynamic>>();
                        message.data["pendingStayInfo"] = new List<Dictionary<string, dynamic>>();
                        message.data["publishedHouseInfo"] = new List<Dictionary<string, dynamic>>();
                        foreach (var stay in host.Stays) {
                            var stayInfo = new Dictionary<string, dynamic>();

                            int imgListNum = 0, stayPrice = 999999999;
                            var stayImgList = new List<string>();
                            foreach (var room in stay.Rooms) {
                                imgListNum += room.RoomPhotos.Count();
                                stayPrice = Math.Min(stayPrice, room.Price);
                                foreach (var roomphoto in room.RoomPhotos)
                                    stayImgList.Add(roomphoto.RPhoto);

                            }
                            stayInfo.Add("stayId", stay.StayId);
                            stayInfo.Add("imgListNum", imgListNum);
                            stayInfo.Add("stayType", stay.StayType);
                            stayInfo.Add("stayNickName", stay.StayName);
                            stayInfo.Add("stayPlace", stay.DetailedAddress);
                            stayInfo.Add("stayPrice", stayPrice);
                            stayInfo.Add("stayImgList", stayImgList);
                            if (stay.StayStatus == 0) {
                                message.data["unpublishedStayInfo"].Add(stayInfo);
                            }
                            else if (stay.StayStatus == 1) {
                                message.data["pendingStayInfo"].Add(stayInfo);
                            }
                            else if (stay.StayStatus == 2) {
                                //stayInfo.Add("valReplyTime", stay.AdministratorStays.First().ValReplyTime);
                                message.data["publishedHouseInfo"].Add(stayInfo);
                            }
                        }
                        message.errorCode = 200;
                        return message.ReturnJson();

                    }
                    catch(Exception e) {
                        Console.WriteLine(e.ToString());
                        message.errorCode = 300;
                        return message.ReturnJson();
                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpPut("avatar")]
        public string ChangeCustomerPhoto() {
            Message message = new Message();
            message.errorCode = 400;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                
                var data = Token.VerifyToken(token);
                if (data != null) {
                    context.DetachAll();
                    int id = int.Parse(data["id"]);
                    var host = context.Hosts.Single(b => b.HostId == id);
                    string photo = Request.Form["hostAvatar"];
                    Console.WriteLine(photo + "200");
                    if (photo != null) {
                        try {
                            string newPhoto = PhotoUpload.UploadPhoto(photo, "hostAvatar/" + id.ToString());
                            if (newPhoto != null) {
                                host.HostAvatar = newPhoto;
                                context.SaveChanges();
                                message.errorCode = 200;
                            }
                        }
                        catch {
                            message.errorCode = 300;
                        }
                    }

                }
            }
            return message.ReturnJson();
        }
    }
}
