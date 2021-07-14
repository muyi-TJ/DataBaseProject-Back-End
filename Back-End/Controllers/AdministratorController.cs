using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back_End.Contexts;
using System.Text.Json;
using Back_End.Models;
using Microsoft.Extensions.Primitives;
using Microsoft.EntityFrameworkCore;

namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministratorController : ControllerBase
    {
        private readonly ModelContext myContext;
        public AdministratorController(ModelContext modelContext)
        {
            myContext = modelContext;
        }
        class PagedStays
        {
            public int stayId { get; set; }
            public int hostId { get; set; }
            public string stayCity { get; set; }
        }

        class PagedReports
        {
            public int reportId { get; set; }
            public int reporterId { get; set; }
            public int stayId { get; set; }
        }

        class PagedNears
        {
            public int nearbyId { get; set; }
            public string nearbyName { get; set; }
            public string nearbyType { get; set; }
            public int nearbyPopularity { get; set; }
            public string nearbyDetailedAdd { get; set; }
        }
        public readonly int pageSize = 10;

        public static Administrator SearchById(int id)
        {
            try
            {
                ModelContext modelContext = new ModelContext();
                var admin = modelContext.Administrators
                    .Single(b => b.AdminId == id);
                return admin;
            }
            catch
            {
                return null;
            }

        }

        public static Administrator SearchByName(string name)
        {
            try
            {
                ModelContext modelContext = new ModelContext();
                var admin = modelContext.Administrators
                    .Single(b => b.AdminUsername == name);
                return admin;
            }
            catch
            {
                return null;
            }
        }

        public static bool AdminLoginByName(Administrator admin, string password)
        {
            try
            {
                if (admin == null)
                {
                    return false;
                }
                else
                {
                    return admin.AdminPassword == password;
                }
            }
            catch
            {
                return false;
            }
        }

        class RoomInfo
        {
            public int roomId { get; set; }
            public byte bathroomNum { get; set; }
            public int bedNum { get; set; }
            public string bedType { get; set; }
        }


        [HttpGet("examineStay/all")]
        public string GetStayTotalNumber()
        {
            GetTotalNumberMessage message = new GetTotalNumberMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if (admin != null)
                    {
                        message.errorCode = 200;
                        int totalNum = myContext.Stays.Where(s => s.StayStatus == 1).Count();
                        message.data["totalNum"] = totalNum;
                    }

                }
            }
            return message.ReturnJson();
        }

        [HttpGet("examineStay")]
        public string GetStayByPage()
        {
            GetStayByPageMessage message = new GetStayByPageMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if (admin != null)
                    {
                        message.errorCode = 200;
                        int page = int.Parse(Request.Query["pagenum"]);
                        var pageInfo = myContext.Stays.Where(s => s.StayStatus == 1).OrderBy(b => b.StayId).Skip((page - 1) * pageSize)
                            .Take(pageSize).Select(c => new PagedStays { stayId = c.StayId, hostId = (int)c.HostId, stayCity = c.DetailedAddress });
                        var examines = pageInfo.ToList();
                        message.data["examineStayList"] = examines;
                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpGet("examineStay/one")]
        public string GetStayById()
        {
            GetStayByIdMessage message = new GetStayByIdMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if (admin != null)
                    {
                        message.errorCode = 200;
                        int stayid = int.Parse(Request.Query["stayId"]);
                        Stay stay = StayController.SearchById(stayid);
                        message.data["detailedAddress"] = stay.DetailedAddress;
                        message.data["stayType"] = stay.StayType;
                        message.data["stayCapability"] = stay.StayCapacity;
                        var rooms = stay.Rooms.ToList();
                        var roomsInfo = new List<RoomInfo>();
                        var photos = new List<string>();
                        foreach (var room in rooms)
                        {
                            var roomInfo = new RoomInfo();
                            roomInfo.roomId = room.RoomId;
                            roomInfo.bathroomNum = (byte)room.BathroomNum;
                            int bedCount = 0;
                            string bedType = "";
                            foreach (var bed in room.RoomBeds)
                            {
                                bedCount += bed.BedNum;
                                bedType += bed.BedType + ' ';
                            }
                            roomInfo.bedNum = bedCount;
                            roomInfo.bedType = bedType;
                            roomsInfo.Add(roomInfo);
                            foreach (var pic in room.RoomPhotos)
                            {
                                photos.Add(pic.RPhoto);
                            }

                        }
                        message.data["roomList"] = roomsInfo;
                        message.data["publicToliet"] = stay.PublicToilet;
                        message.data["publicBath"] = stay.PublicBathroom;
                        message.data["hasBarrierFree"] = stay.NonBarrierFacility;
                        message.data["stayPicList"] = photos;
                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpGet("examineReport/all")]
        public string GetReportyTotalNumber()
        {
            GetTotalNumberMessage message = new GetTotalNumberMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if (admin != null)
                    {
                        message.errorCode = 200;
                        int totalNum = myContext.Reports.Where(s => s.IsDealed == 0).Count();
                        message.data["totalNum"] = totalNum;
                    }

                }
            }
            return message.ReturnJson();
        }


        [HttpGet("examineReport")]
        public string GetReportByPage()
        {
            GetReportByPageMessage message = new GetReportByPageMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if (admin != null)
                    {
                        message.errorCode = 200;
                        int page = int.Parse(Request.Query["pagenum"]);
                        var pageInfo = myContext.Reports.Where(s => s.IsDealed == 0).OrderBy(b => b.ReportTime).Skip((page - 1) * pageSize)
                            .Take(pageSize).Select(c => new PagedReports
                            { stayId = c.Order.Generates.First().StayId, reportId = c.OrderId, reporterId = c.Order.CustomerId });
                        var examines = pageInfo.ToList();
                        message.data["reportList"] = examines;
                        //TODO:test
                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpGet("examineReport/one")]
        public string GetReportById()
        {
            GetReportByIdMessage message = new GetReportByIdMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if (admin != null)
                    {
                        message.errorCode = 200;
                        int reportId = int.Parse(Request.Query["reportId"]);
                        Report report = ReportController.SearchById(reportId);
                        message.data["orderId"] = report.OrderId;
                        message.data["reportTime"] = report.ReportTime;
                        message.data["reportReason"] = report.Reason;
                        message.data["hostId"] = report.Order.Generates.First().Room.Stay.HostId;
                        message.data["stayId"] = report.Order.Generates.First().StayId;
                        message.data["hostCredit"] = report.Order.Generates.First().Room.Stay.Host.HostScore;
                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpGet("nearby/all")]
        public string GetNearTotalNumber()
        {
            GetTotalNumberMessage message = new GetTotalNumberMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if (admin != null)
                    {
                        message.errorCode = 200;
                        int totalNum = myContext.Peripherals.Count();
                        message.data["totalNum"] = totalNum;
                    }

                }
            }
            return message.ReturnJson();
        }




        [HttpGet("nearby")]
        public string GetNearByPage()
        {
            GetNearByPageMessage message = new GetNearByPageMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if (admin != null)
                    {
                        message.errorCode = 200;
                        int page = int.Parse(Request.Query["pagenum"]);
                        var pageInfo = myContext.Peripherals.OrderBy(b => b.PeripheralId).Skip((page - 1) * pageSize)
                            .Take(pageSize).Select(c => new PagedNears
                            {
                                nearbyId = c.PeripheralId,
                                nearbyType = c.PeripheralClass,
                                nearbyName = c.PeripheralName,
                                nearbyPopularity = (int)c.PeripheralPopularity,
                                nearbyDetailedAdd = c.DetailedAddress
                            });
                        var nears = pageInfo.ToList();
                        message.data["nearbyList"] = nears;
                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpPost("examineStay/result")]
        public string UploadStayExamine()
        {
            UploadExamineMessage message = new UploadExamineMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if (admin != null)
                    {
                        message.errorCode = 200;
                        myContext.DetachAll();
                        int stayId = int.Parse(Request.Form["stayId"]);
                        int isPass = int.Parse(Request.Form["isPass"]);
                        Stay stay = StayController.SearchById(stayId);
                        if (stay != null)
                        {
                            myContext.Entry(stay).State = EntityState.Unchanged;
                            AdministratorStay form = new AdministratorStay();
                            form.AdminId = id;
                            form.StayId = stayId;
                            form.ValReplyTime = DateTime.Now;
                            if (isPass == 1)
                            {
                                stay.StayStatus = 2;
                                form.ValidateResult = 1;
                                form.ValidateReply = " ";
                            }
                            else
                            {
                                stay.StayStatus = 3;
                                form.ValidateResult = 0;
                                form.ValidateReply = Request.Form["msg"];
                            }//房源状态0保存未提交，1提交未审核，2审核通过，3审核不通过
                            myContext.AdministratorStays.Add(form);
                            myContext.SaveChanges();

                            message.data["isSuccess"] = true;
                        }
                        else
                        {
                            message.data["isSuccess"] = true;
                        }
                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpPost("examineReport/result")]
        public string UploadReportExamine()
        {
            UploadExamineMessage message = new UploadExamineMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if (admin != null)
                    {
                        message.errorCode = 200;
                        myContext.DetachAll();
                        int reportId = int.Parse(Request.Form["reportId"]);
                        int isBan = int.Parse(Request.Form["isBan"]);
                        Report report = ReportController.SearchById(reportId);
                        if (report != null)
                        {
                            myContext.Entry(report).State = EntityState.Unchanged;
                            report.IsDealed = 1;
                            report.AdminId = id;
                            report.DealTime = DateTime.Now;
                            report.Reply = "已处理完成 ";
                            if (isBan == 1)
                            {
                                report.Reply += "已封禁";
                                report.Order.Generates.First().Room.Stay.Host.HostState = 1;
                            }
                            else
                            {
                                report.Reply = "未封禁";
                            }
                        }
                        myContext.SaveChanges();
                        message.data["isSuccess"] = true;
                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpPost("near/update")]
        public string UploadNewNear()
        {
            UploadExamineMessage message = new UploadExamineMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if (admin != null)
                    {
                        message.errorCode = 200;
                        myContext.DetachAll();
                        Peripheral near = new Peripheral();
                        near.PeripheralName = Request.Form["nearbyName"];
                        near.PeripheralClass = Request.Form["nearbyType"];
                        near.PeripheralPopularity = int.Parse(Request.Form["nearbyPopularity"]);
                        near.DetailedAddress = Request.Form["nearbyDetailedAdd"];
                        myContext.Peripherals.Add(near);
                        myContext.SaveChanges();
                        message.data["isSuccess"] = true;
                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpPut("near/modify")]
        public string ChangeNearInfo()
        {
            UploadExamineMessage message = new UploadExamineMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if (admin != null)
                    {
                        message.errorCode = 200;
                        myContext.DetachAll();
                        int nearId = int.Parse(Request.Form["nearbyId"]);
                        Peripheral near = myContext.Peripherals.Single(s => s.PeripheralId == nearId);
                        near.PeripheralName = Request.Form["nearbyName"];
                        near.PeripheralClass = Request.Form["nearbyType"];
                        near.PeripheralPopularity = int.Parse(Request.Form["nearbyPopularity"]);
                        near.DetailedAddress = Request.Form["nearbyDetailedAdd"];
                        myContext.SaveChanges();
                        message.data["isSuccess"] = true;
                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpGet("nearby/search")]
        public string SearchNearInfo()
        {
            SearchNearMessage message = new SearchNearMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if (admin != null)
                    {
                        message.errorCode = 200;
                        string search = Request.Query["search"];
                        var searchedInfo = myContext.Peripherals.Where(s => s.PeripheralId.ToString().Contains(search) || s.PeripheralName.Contains(search) || s.DetailedAddress.Contains(search)).OrderBy(b => b.PeripheralId)
                            .Take(pageSize).Select(c => new PagedNears
                            {
                                nearbyId = c.PeripheralId,
                                nearbyType = c.PeripheralClass,
                                nearbyName = c.PeripheralName,
                                nearbyPopularity = (int)c.PeripheralPopularity,
                                nearbyDetailedAdd = c.DetailedAddress
                            });
                        var nears = searchedInfo.ToList();
                        message.data["total"] = nears.Count();
                        message.data["nearbyList"] = nears;
                    }
                }
            }
            return message.ReturnJson();

        }
    }
}
