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

namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministratorController : ControllerBase
    {
        private readonly ModelContext context;
        AdministratorController(ModelContext modelContext)
        {
            context = modelContext;
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

        public static bool AdminLoginByName(Administrator admin,string password)
        {
            try
            {
                if(admin==null)
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
                        var pageInfo = context.Stays.Where(s => s.StayStatus == 0).OrderBy(b => b.StayId).Skip((page - 1) * pageSize)
                            .Take(pageSize).Select(c => new PagedStays { stayId = c.StayId, hostId = (int)c.HostId, stayCity = c.Area.AreaName });
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
                        var roomsInfo = new List<string>();
                        var photos = new List<string>();
                        foreach (var room in rooms)
                        {
                            string temp = "";
                            temp += "roomId:";
                            temp += room.RoomId.ToString();
                            temp += ",\nbathroomNum:";
                            temp += room.BathroomNum.ToString();
                            int bedCount = 0;
                            string bedType = "";
                            foreach (var bed in room.RoomBeds)
                            {
                                bedCount += bed.BedNum;
                                bedType += BedController.SearchById(bed.BedId).BedType + ' ';
                            }
                            temp += ",\nbedNum:";
                            temp += stay.BedNum.ToString();
                            temp += ",\nbedType:";
                            temp += bedType;
                            roomsInfo.Add(temp);
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
                        var pageInfo = context.Reports.Where(s => s.IsDealed == 0).OrderBy(b => b.ReportTime).Skip((page - 1) * pageSize)
                            .Take(pageSize).Select(c => new PagedReports
                            { stayId = c.Order.Generates.First().StayId, reportId = c.OrderId, reporterId = (int)c.Order.CustomerId });
                        //TODO:generate
                        var examines = pageInfo.ToList();
                        message.data["reportList"] = examines;
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
                        //message.data["hostId"]=report.Order.Generates.First().
                        //TODO:generate
                        //message.data["stayId"]
                        //message.data["hostCredit"]=
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
                        var pageInfo = context.Peripherals.OrderBy(b => b.PeripheralId).Skip((page - 1) * pageSize)
                            .Take(pageSize).Select(c => new PagedNears
                            {
                                nearbyId = c.PeripheralId,
                                nearbyType = c.PeripheralClass,
                                nearbyName = c.PeripheralName,
                                nearbyPopularity = (int)c.PeripheralPopularity,
                                nearbyDetailedAdd = c.PeripheralRoad
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
            UploadStayExamineMessage message = new UploadStayExamineMessage();
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
                        context.DetachAll();
                        int stayId = int.Parse(Request.Form["stayId"]);
                        bool isPass = bool.Parse(Request.Form["isPass"]);
                        Stay stay = StayController.SearchById(stayId);
                        AdministratorStay form = new AdministratorStay();
                        form.AdminId = id;
                        form.StayId = stayId;
                        form.ValReplyTime = DateTime.Now;
                        if(isPass)
                        {
                            stay.StayStatus = 2;
                            form.ValidateResult = 1;
                        }
                        else
                        {
                            stay.StayStatus = 1;
                            form.ValidateResult = 0;
                            form.ValidateReply = Request.Form["msg"];
                        }//房源状态0未审核，1审核不通过，2审核通过
                        context.AdministratorStays.Add(form);
                        context.SaveChanges();
                        message.data["isSuccess"] = true;
                    }
                }
            }
            return message.ReturnJson();
        }
    }
}
