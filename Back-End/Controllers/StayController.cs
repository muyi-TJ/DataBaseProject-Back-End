using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back_End.Contexts;
using System.Text.Json;
using Back_End.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StayController : ControllerBase
    {
        private readonly ModelContext myContext;
        public StayController(ModelContext modelContext)
        {
            myContext = modelContext;
        }
        

        public static Stay SearchById(int id)
        {
            try
            {
                ModelContext context = new ModelContext();
                var stay = context.Stays
                    .Single(b => b.StayId == id);
                return stay;
            }
            catch
            {
                return null;
            }
        }

        public class StayInfo {
            public int stayId { get; set; }
            public string stayName { get; set; }
            public string stayCharcateristic { get; set; }
            public int stayPrice { get; set; }
            public string stayPhoto { get; set; }
        }

        // 根据最低价格选取6个价格最低的
        [HttpGet("getStayByPrice")]
        public string GetStayByPrice() {
            //var context = context; 
            //context.DetachAll();
            var message = new GetStayMessage();
            message.data.Add("stayList", new List<StayInfo>());
            try {
                var staySelectList = myContext.Rooms
                    .GroupBy(r => r.StayId)
                    .OrderBy(r => r.Min(x => x.Price))
                    .Select(g => new StayInfo {
                        stayId = g.Key,
                        stayPrice = g.Min(x => x.Price)
                    })
                    .ToList();
                if (staySelectList.Count > 6)
                    staySelectList.RemoveRange(6, staySelectList.Count - 6);
                foreach (var staySelect in staySelectList) {
                    var stay = myContext.Stays.Single(b => b.StayId == staySelect.stayId);
                    staySelect.stayCharcateristic = stay.Characteristic;
                    staySelect.stayName = stay.StayName;
                    staySelect.stayPhoto = myContext.RoomPhotos.Where(b => b.StayId == staySelect.stayId).FirstOrDefault().RPhoto;
                }
                message.data["stayList"] = staySelectList;
                message.errorCode = 200;
                return message.ReturnJson();
            }
            catch(Exception e) {
                Console.WriteLine(e.ToString());
                return message.ReturnJson();
            }
        }

        // 根据用户评分选取4个最高的
        [HttpGet("getStayByScore")]
        public string GetStayByScore() {
            //var context = context;
            //context.DetachAll();
            var message = new GetStayMessage();
            message.data.Add("stayList", new List<StayInfo>());
            try {
                var staySelectList = myContext.Stays.
                    OrderByDescending(r => r.CommentScore > 0 ? ((float)r.CommentScore / (float)r.CommentNum) : 0)
                    .Select(g => new StayInfo {
                        stayId = g.StayId
                    })
                    .ToList();

                if (staySelectList.Count > 4)
                    staySelectList.RemoveRange(4, staySelectList.Count - 4);

                foreach (var staySelect in staySelectList) {
                    var stay = myContext.Stays.Single(b => b.StayId == staySelect.stayId);
                    staySelect.stayPhoto = myContext.RoomPhotos.Where(b => b.StayId == staySelect.stayId).FirstOrDefault().RPhoto;
                    staySelect.stayPrice = myContext.Rooms.Where(b => b.StayId == staySelect.stayId).Min(x => x.Price);
                    staySelect.stayCharcateristic = stay.Characteristic;
                    staySelect.stayName = stay.StayName;
                }
                message.errorCode = 200;
                message.data["stayList"] = staySelectList;
                return message.ReturnJson();
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                return message.ReturnJson();
            }
        }

        // 根据用户评论数选取前4个评论最多的
        [HttpGet("getStayByHot")]
        public string GetStayByHot() {
            //var context = context;
            //context.DetachAll();
            var message = new GetStayMessage();
            message.data.Add("stayList", new List<StayInfo>());
            try {
                var staySelectList = myContext.Stays.
                    OrderByDescending(r => r.CommentNum)
                    .Select(g => new StayInfo {
                        stayId = g.StayId
                    })
                    .ToList();

                if (staySelectList.Count > 4)
                    staySelectList.RemoveRange(4, staySelectList.Count - 4);

                foreach (var staySelect in staySelectList) {
                    var stay = myContext.Stays.Single(b => b.StayId == staySelect.stayId);
                    staySelect.stayPhoto = myContext.RoomPhotos.Where(b => b.StayId == staySelect.stayId).FirstOrDefault().RPhoto;
                    staySelect.stayPrice = myContext.Rooms.Where(b => b.StayId == staySelect.stayId).Min(x => x.Price);
                    staySelect.stayCharcateristic = stay.Characteristic;
                    staySelect.stayName = stay.StayName;
                }
                message.errorCode = 200;
                message.data["stayList"] = staySelectList;
                return message.ReturnJson();
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                return message.ReturnJson();
            }
        }


        class StayInMapInfo
        {
            public int stayID { get; set; }
            public int stayPrice { get; set; }
            public decimal[] stayPosition { get; set; }
        }

        class StayRoughInfo
        {
            public string stayName { get; set; }
            public string stayDescribe { get; set; }
            public List<string> stayPhotos { get; set; }
            public string hostAvatar { get; set; }
            public double stayScore { get; set; }
            public bool isLike { get; set; }

        }

        class StayDetailedInfo:StayRoughInfo
        {
            public List<string> stayLabel { get; set; }
            public int stayPrice { get; set; }
            public int stayCommentNum { get; set; }
            public decimal[] stayPosition { get; set; }
        }

        [HttpGet("getPositions")]
        public string GetStayByLngAndLat()
        {
            GetStayInfoMessage message = new GetStayInfoMessage();
            try
            {
                decimal left = decimal.Parse(Request.Query["westLng"]);
                decimal right = decimal.Parse(Request.Query["eastLng"]);
                decimal up = decimal.Parse(Request.Query["northLat"]);
                decimal down = decimal.Parse(Request.Query["southLat"]);
                bool cross = left * right < 0 && right - left < 0;
                var stays = myContext.Stays.Where(s => s.StayStatus == 2 && IsInMap(left, right, up, down, s.Latitude, s.Longitude, cross))
                    .Select(c => new StayInMapInfo
                    {
                        stayID = c.StayId,
                        stayPrice = GetMinPrice(c.Rooms.ToList()),
                        stayPosition = new decimal[] { c.Longitude, c.Latitude }
                    }).ToList();
                message.errorCode = 200;
                message.data["stayPositionNum"] = stays.Count;
                message.data["stayPositionInfo"] = stays;
            }
            catch
            {

            }
            return message.ReturnJson();
        }

        public int GetMinPrice(List<Room> rooms)
        {
            int min = int.MaxValue;
            foreach(var room in rooms)
            {
                if(room.Price<min)
                {
                    min = room.Price;
                }
            }
            return min;
        }

        private bool IsInMap(decimal left,decimal right,decimal up,decimal down,decimal lat,decimal lng,bool cross)
        {
            if(lat<up&&lat>down)
            {
                if(cross)
                {
                    if(lng>left||lng<right)
                    {
                        return true;
                    }
                }
                else
                {
                    if (lng > left && lng <right)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        [HttpGet("type")]
        public string GetAllStayType()
        {
            GetStayTypeMessage message = new GetStayTypeMessage();
            try
            {
                message.errorCode = 200;
                var stayType = myContext.StayTypes.Select(c => c.StayType1).ToList();
                message.data["typeList"] = stayType;
            }
            catch
            {

            }
            return message.ReturnJson();
        }

        [HttpGet("getRoughStay")]
        public string GetStayRoughInfo()
        {
            GetStayInfoMessage message = new GetStayInfoMessage();
            try
            {
                int stayId = int.Parse(Request.Query["stayID"]);
                var stay = SearchById(stayId);
                if(stay!=null)
                {
                    message.errorCode = 200;
                    StayRoughInfo info = new StayRoughInfo();
                    info.stayName = stay.StayName;
                    var rooms = stay.Rooms.ToList();
                    int bathroom = (int)stay.PublicToilet;
                    var photos = new List<string>();
                    foreach (var room in rooms)
                    {
                        bathroom += (int)room.BathroomNum;
                        foreach (var pic in room.RoomPhotos)
                        {
                            photos.Add(pic.RPhoto);
                        }
                    }
                    info.stayDescribe = rooms.Count.ToString() + "室" + bathroom.ToString() + "卫";
                    info.stayPhotos = photos;
                    info.hostAvatar = stay.Host.HostAvatar;
                    info.stayScore = (double)stay.CommentScore / (double)stay.CommentNum;
                    bool islike = false;
                    StringValues token = default(StringValues);
                    if (Request.Headers.TryGetValue("token", out token))
                    {
                        var data = Token.VerifyToken(token);
                        if (data != null)
                        {
                            int id = int.Parse(data["id"]);
                            var customer = CustomerController.SearchById(id);
                            var favorites = customer.Favorites.ToList();
                            foreach(var favorite in favorites)
                            {
                                if (!islike)
                                { foreach (var elem in favorite.Favoritestays.ToList())
                                    {
                                        if (elem.StayId==stayId)
                                        {
                                            islike = true;
                                            break;
                                        }
                                }
                                }
                            }
                        }
                    }
                    info.isLike = islike;
                    message.data["stayPositionNum"] = 1;
                    message.data["stayPositionInfo"] = info;
                }

            }
            catch
            {

            }
            return message.ReturnJson();
        }

        [HttpGet("getDetailedStay")]
        public string GetStayDetailedInfo()
        {
            GetStayInfoMessage message = new GetStayInfoMessage();
            try
            {
                int stayId = int.Parse(Request.Query["stayID"]);
                var stay = SearchById(stayId);
                if (stay != null)
                {
                    message.errorCode = 200;
                    StayDetailedInfo info = new StayDetailedInfo();
                    info.stayName = stay.StayName;
                    var rooms = stay.Rooms.ToList();
                    int bathroom = (int)stay.PublicToilet;
                    var photos = new List<string>();
                    foreach (var room in rooms)
                    {
                        bathroom += (int)room.BathroomNum;
                        foreach (var pic in room.RoomPhotos)
                        {
                            photos.Add(pic.RPhoto);
                        }
                    }
                    info.stayDescribe = rooms.Count.ToString() + "室" + bathroom.ToString() + "卫";
                    info.stayPhotos = photos;
                    info.hostAvatar = stay.Host.HostAvatar;
                    if(stay.CommentNum!=0)
                    {
                        info.stayScore = (double)stay.CommentScore / (double)stay.CommentNum;
                    }
                    else
                    {
                        info.stayScore = 0;
                    }
                    bool islike = false;
                    StringValues token = default(StringValues);
                    if (Request.Headers.TryGetValue("token", out token))
                    {
                        var data = Token.VerifyToken(token);
                        if (data != null)
                        {
                            int id = int.Parse(data["id"]);
                            var customer = CustomerController.SearchById(id);
                            var favorites = customer.Favorites.ToList();
                            foreach (var favorite in favorites)
                            {
                                if (!islike)
                                {
                                    foreach (var elem in favorite.Favoritestays.ToList())
                                    {
                                        if (elem.StayId == stayId)
                                        {
                                            islike = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    info.isLike = islike;
                    info.stayCommentNum =(int) stay.CommentNum;
                    info.stayLabel = myContext.StayLabels.Where(s => s.StayId == stay.StayId).Select(c => c.LabelName).ToList();
                    info.stayPrice = GetMinPrice(stay.Rooms.ToList());
                    info.stayPosition = new decimal[] { stay.Longitude, stay.Latitude };
                    message.data["stayPositionNum"] = 1;
                    message.data["stayPositionInfo"] = info;
                }

            }
            catch
            {

            }
            return message.ReturnJson();
        }

        class RoomInfo
        {
            public int roomId { get; set; }
            public int price { get; set; }
            public decimal? roomArea { get; set; }
            public byte? bathNum { get; set; }
            public string[] bedTypes { get; set; }
            public byte[] bedNums { get; set; }
            public string[] images { get; set; }
        }

        [HttpPost("infos")]
        public string AddNewStay()
        {
            Message message = new Message();
            message.errorCode = 300;
            try
            {
                Stay stay = new Stay();
                stay.StayType = Request.Form["stayType"];
                stay.StayCapacity = byte.Parse(Request.Form["maxTenantNum"]);
                stay.DetailedAddress = Request.Form["struPos"];
                stay.RoomNum = byte.Parse(Request.Form["roomNum"]);
                stay.BedNum = byte.Parse(Request.Form["bedNum"]);
                stay.PublicToilet = decimal.Parse(Request.Form["pubRestNum"]);
                stay.PublicBathroom = decimal.Parse(Request.Form["pubBathNum"]);
                stay.NonBarrierFacility = decimal.Parse(Request.Form["barrierFree"]);
                stay.Longitude = decimal.Parse(Request.Form["Longtitude"]);
                stay.Latitude = decimal.Parse(Request.Form["Latitude"]);
                stay.StayName = Request.Form["stayName"];
                stay.Characteristic = Request.Form["stayChars"];
                stay.StartTime = DateTime.Parse(Request.Form["startTime"]);
                stay.EndTime = DateTime.Parse(Request.Form["endTime"]);
                stay.DaysMax = byte.Parse(Request.Form["maxDay"]);
                stay.DaysMin = byte.Parse(Request.Form["minDay"]);
                stay.StayStatus = decimal.Parse(Request.Form["stayStatus"]);
                stay.CommentNum = 0;
                stay.CommentScore = 0;
                StringValues token = default(StringValues);
                int? hostId = null;
                if (Request.Headers.TryGetValue("token", out token))
                {
                    var data = Token.VerifyToken(token);
                    if (data != null)
                    {
                        myContext.DetachAll();
                        int id = int.Parse(data["id"]);
                        var host = HostController.SearchById(id);
                        if(host!=null)
                        {
                            hostId = host.HostId;
                        }
                    }
                }
                if(hostId!=null)
                {
                    stay.HostId = hostId;
                }
                else
                {
                    throw (null);
                }
                myContext.Stays.Add(stay);
                myContext.SaveChanges();
                //TODO:test
                var rooms = JsonSerializer.Deserialize<List<RoomInfo>>(Request.Form["roomInfo"]);
                foreach(var room in rooms)
                {
                    Room newRoom = new Room();
                    newRoom.StayId = stay.StayId;
                    newRoom.RoomId = room.roomId;
                    newRoom.Price = room.price;
                    newRoom.RoomArea = room.roomArea;
                    newRoom.BathroomNum = room.bathNum;
                    myContext.Rooms.Add(newRoom);
                    myContext.SaveChanges();
                    for(int i=0;i<room.bedTypes.Length;i++)
                    {
                        if(room.bedNums[i]>0)
                        {
                            RoomBed roomBed = new RoomBed();
                            //TODO:修改type
                            roomBed.BedNum = room.bedNums[i];
                            roomBed.RoomId = newRoom.RoomId;
                            roomBed.StayId = stay.StayId;
                            myContext.RoomBeds.Add(roomBed);
                        }//全部插入后再保存
                    }
                    myContext.SaveChanges();
                    for(int i=0;i<room.images.Length;i++)
                    {
                        string url = PhotoUpload.UploadPhoto(room.images[i], "roomPhoto/" + stay.StayId + '-' + newRoom.RoomId + '-' + i.ToString());
                        if(url!=null)
                        {
                            var photo = new RoomPhoto();
                            photo.StayId = stay.StayId;
                            photo.RoomId = newRoom.RoomId;
                            photo.RPhoto = url;
                            myContext.RoomPhotos.Add(photo);
                        }//目前上传失败不抛出异常
                    }
                    myContext.SaveChanges();
                }
                message.errorCode = 200;
            }
            catch
            {

            }

            return message.ReturnJson();
        }

        [HttpPut("info")]
        public string ChangeStayInfo()
        {
            Message message = new Message();
            message.errorCode = 300;
            int preId = -1;
            int.TryParse(Request.Form["stayId"], out preId);
            Stay preStay = SearchById(-1);
            if (preStay != null)
            {
                myContext.Entry(preStay).State = EntityState.Unchanged;
                myContext.Stays.Remove(preStay);
                myContext.SaveChanges();
                try
                {
                    Stay stay = new Stay();
                    stay.StayType = Request.Form["stayType"];
                    stay.StayCapacity = byte.Parse(Request.Form["maxTenantNum"]);
                    stay.DetailedAddress = Request.Form["struPos"];
                    stay.RoomNum = byte.Parse(Request.Form["roomNum"]);
                    stay.BedNum = byte.Parse(Request.Form["bedNum"]);
                    stay.PublicToilet = decimal.Parse(Request.Form["pubRestNum"]);
                    stay.PublicBathroom = decimal.Parse(Request.Form["pubBathNum"]);
                    stay.NonBarrierFacility = decimal.Parse(Request.Form["barrierFree"]);
                    stay.Longitude = decimal.Parse(Request.Form["Longtitude"]);
                    stay.Latitude = decimal.Parse(Request.Form["Latitude"]);
                    stay.StayName = Request.Form["stayName"];
                    stay.Characteristic = Request.Form["stayChars"];
                    stay.StartTime = DateTime.Parse(Request.Form["startTime"]);
                    stay.EndTime = DateTime.Parse(Request.Form["endTime"]);
                    stay.DaysMax = byte.Parse(Request.Form["maxDay"]);
                    stay.DaysMin = byte.Parse(Request.Form["minDay"]);
                    stay.StayStatus = decimal.Parse(Request.Form["stayStatus"]);
                    stay.CommentNum = 0;
                    stay.CommentScore = 0;
                    StringValues token = default(StringValues);
                    int? hostId = null;
                    if (Request.Headers.TryGetValue("token", out token))
                    {
                        var data = Token.VerifyToken(token);
                        if (data != null)
                        {
                            myContext.DetachAll();
                            int id = int.Parse(data["id"]);
                            var host = HostController.SearchById(id);
                            if (host != null)
                            {
                                hostId = host.HostId;
                            }
                        }
                    }
                    if (hostId != null)
                    {
                        stay.HostId = hostId;
                    }
                    else
                    {
                        throw (null);
                    }
                    myContext.Stays.Add(stay);
                    myContext.SaveChanges();
                    //TODO:test
                    var rooms = JsonSerializer.Deserialize<List<RoomInfo>>(Request.Form["roomInfo"]);
                    foreach (var room in rooms)
                    {
                        Room newRoom = new Room();
                        newRoom.StayId = stay.StayId;
                        newRoom.RoomId = room.roomId;
                        newRoom.Price = room.price;
                        newRoom.RoomArea = room.roomArea;
                        newRoom.BathroomNum = room.bathNum;
                        myContext.Rooms.Add(newRoom);
                        myContext.SaveChanges();
                        for (int i = 0; i < room.bedTypes.Length; i++)
                        {
                            if (room.bedNums[i] > 0)
                            {
                                RoomBed roomBed = new RoomBed();
                                //TODO:修改type
                                roomBed.BedNum = room.bedNums[i];
                                roomBed.RoomId = newRoom.RoomId;
                                roomBed.StayId = stay.StayId;
                                myContext.RoomBeds.Add(roomBed);
                            }//全部插入后再保存
                        }
                        myContext.SaveChanges();
                        for (int i = 0; i < room.images.Length; i++)
                        {
                            string url = PhotoUpload.UploadPhoto(room.images[i], "roomPhoto/" + stay.StayId + '-' + newRoom.RoomId + '-' + i.ToString());
                            if (url != null)
                            {
                                var photo = new RoomPhoto();
                                photo.StayId = stay.StayId;
                                photo.RoomId = newRoom.RoomId;
                                photo.RPhoto = url;
                                myContext.RoomPhotos.Add(photo);
                            }//目前上传失败不抛出异常
                        }
                        myContext.SaveChanges();
                    }
                    message.errorCode = 200;
                }
                catch
                {

                }
            }
            return message.ReturnJson();
        }

        // 通过房源Id删除房源
        [HttpDelete("delStayById")]
        public string DelStayById(int stayId = -1) {
            Message message = new Message();
            message.errorCode = 400;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    try {
                        var stay = new Stay() { StayId = stayId };
                        myContext.Stays.Remove(stay);
                        myContext.SaveChanges();

                        message.errorCode = 200;
                        return message.ReturnJson();
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.ToString());
                        message.errorCode = 300;
                        return message.ReturnJson();
                    }
                }
            }
            return message.ReturnJson();
        }

        // 获取房东某个房源的订单数据
        /*[HttpGet("StayOrderInfo")]
        public string GetStayOrderInfo(int stayId = -1) {
            StayOrderInfoMessage message = new StayOrderInfoMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    try {
                        var stay = myContext.Stays.Single(b => b.StayId == stayId);
                        message.data["averageScore"] = (float)stay.CommentScore / (float)stay.CommentNum;

                        DateTime time = DateTime.Now.AddDays(-365);
                        var orderInfoOfDateList = new List<Dictionary<string, dynamic>>();
                        var orderDateList = myContext.Orders.Where(b => DateTime.Compare((DateTime)b.OrderTime, time) > 0);
                        foreach(var order in orderDateList) {
                            var dict = new Dictionary<string, dynamic>();
                            var orderTime = (DateTime)order.OrderTime;
                            dict.Add("data", orderTime.Year.ToString() + "-" + orderTime.Month.ToString() + "月");

                        }
                    }
                    catch (Exception e) {
                    }
                }
            }
        }*/
    }
}
