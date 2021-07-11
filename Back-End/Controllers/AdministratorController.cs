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
        public readonly int pageSize = 10;

        public static Administrator SearchById(int id)
        {
            try
            {
                var admin = ModelContext.Instance.Administrators
                    .Single(b => b.AdminId == id);
                return admin;
            }
            catch
            {
                return null;
            }

        }

        [HttpGet]
        public string GetAdmin()
        {
            //UNDONE:等待修改api
            return null;
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
                if(data!=null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if(admin!=null)
                    {
                        message.errorCode = 200;
                        int page = int.Parse(Request.Query["pagenum"]);
                        var pageInfo = ModelContext.Instance.Stays.Where(s => s.StayStatus == 0).OrderBy(b => b.StayId).Skip((page - 1) * pageSize)
                            .Take(pageSize).Select(c => new PagedStays{ stayId = c.StayId, hostId = (int)c.HostId, stayCity = c.Area.AreaName });
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
                        int stayid= int.Parse(Request.Query["stayId"]);
                        Stay stay = StayController.SearchById(id);
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
                                bedType += BedController.BedType[BedController.SearchById(bed.BedId).BedType] + ' ';
                            }
                            temp += ",\nbedNum:";
                            temp += stay.BedNum.ToString();
                            temp += ",\nbedType:";
                            temp += bedType;
                            roomsInfo.Add(temp);
                            foreach(var pic in room.RoomPhotos)
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
                        var pageInfo = ModelContext.Instance.Reports.Where(s => s.IsDealed == 0).OrderBy(b => b.ReportTime).Skip((page - 1) * pageSize)
                            .Take(pageSize).Select(c => new PagedReports
                            { stayId = c.Order.Generates.First().StayId, reportId = c.OrderId, reporterId = (int)c.Order.CustomerId });
                        var examines = pageInfo.ToList();
                        message.data["reportList"] = examines;
                    }
                }
            }
            return message.ReturnJson();
        }
    }
}
