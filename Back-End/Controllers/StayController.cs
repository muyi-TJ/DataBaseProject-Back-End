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

namespace Back_End.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class StayController : ControllerBase {
        private readonly ModelContext myContext;
        public StayController(ModelContext modelContext) {
            myContext = modelContext;
        }


        public static Stay SearchById(int id) {
            try {
                ModelContext context = new ModelContext();
                var stay = context.Stays
                    .Single(b => b.StayId == id);
                return stay;
            }
            catch {
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
                    .Where( b=> b.Stay.StayStatus == 2)
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
            catch (Exception e) {
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
                    Where(b => b.StayStatus == 2).
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
                    Where(b => b.StayStatus == 2).
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


        class StayInMapInfo {
            public int stayID { get; set; }
            public int stayPrice { get; set; }
            public decimal[] stayPosition { get; set; }
        }

        class StayRoughInfo {
            public string stayName { get; set; }
            public string stayDescribe { get; set; }
            public List<string> stayPhotos { get; set; }
            public string hostAvatar { get; set; }
            public double stayScore { get; set; }
            public bool isLike { get; set; }

        }

        class StayDetailedInfo : StayRoughInfo {
            public List<string> stayLabel { get; set; }
            public int stayPrice { get; set; }
            public int stayCommentNum { get; set; }
            public decimal[] stayPosition { get; set; }
        }

        [HttpGet("getPositions")]
        public string GetStayByLngAndLat() {
            GetStayInfoMessage message = new GetStayInfoMessage();
            try {
                decimal left = decimal.Parse(Request.Query["westLng"]);
                decimal right = decimal.Parse(Request.Query["eastLng"]);
                decimal up = decimal.Parse(Request.Query["northLat"]);
                decimal down = decimal.Parse(Request.Query["southLat"]);
                bool cross = left * right < 0 && right - left < 0;
                var stays = myContext.Stays.Where(s => s.StayStatus == 2 && IsInMap(left, right, up, down, s.Latitude, s.Longitude, cross))
                    .Select(c => new StayInMapInfo {
                        stayID = c.StayId,
                        stayPrice = GetMinPrice(c.Rooms.ToList()),
                        stayPosition = new decimal[] { c.Longitude, c.Latitude }
                    }).ToList();
                message.errorCode = 200;
                message.data["stayPositionNum"] = stays.Count;
                message.data["stayPositionInfo"] = stays;
            }
            catch {

            }
            return message.ReturnJson();
        }

        public int GetMinPrice(List<Room> rooms) {
            int min = int.MaxValue;
            foreach (var room in rooms) {
                if (room.Price < min) {
                    min = room.Price;
                }
            }
            return min;
        }

        private bool IsInMap(decimal left, decimal right, decimal up, decimal down, decimal lat, decimal lng, bool cross) {
            if (lat < up && lat > down) {
                if (cross) {
                    if (lng > left || lng < right) {
                        return true;
                    }
                }
                else {
                    if (lng > left && lng < right) {
                        return true;
                    }
                }
            }
            return false;
        }

        [HttpGet("type")]
        public string GetAllStayType() {
            GetStayTypeMessage message = new GetStayTypeMessage();
            try {
                message.errorCode = 200;
                var stayType = myContext.StayTypes.Select(c => c.StayType1).ToList();
                message.data["typeList"] = stayType;
            }
            catch {

            }
            return message.ReturnJson();
        }

        [HttpGet("getRoughStay")]
        public string GetStayRoughInfo() {
            GetStayInfoMessage message = new GetStayInfoMessage();
            try {
                int stayId = int.Parse(Request.Query["stayID"]);
                var stay = SearchById(stayId);
                if (stay != null) {
                    message.errorCode = 200;
                    StayRoughInfo info = new StayRoughInfo();
                    info.stayName = stay.StayName;
                    var rooms = stay.Rooms.ToList();
                    int bathroom = (int)stay.PublicToilet;
                    var photos = new List<string>();
                    foreach (var room in rooms) {
                        bathroom += (int)room.BathroomNum;
                        foreach (var pic in room.RoomPhotos) {
                            photos.Add(pic.RPhoto);
                        }
                    }
                    info.stayDescribe = rooms.Count.ToString() + "室" + bathroom.ToString() + "卫";
                    info.stayPhotos = photos;
                    info.hostAvatar = stay.Host.HostAvatar;
                    info.stayScore = (double)stay.CommentScore / (double)stay.CommentNum;
                    bool islike = false;
                    StringValues token = default(StringValues);
                    if (Request.Headers.TryGetValue("token", out token)) {
                        var data = Token.VerifyToken(token);
                        if (data != null) {
                            int id = int.Parse(data["id"]);
                            var customer = CustomerController.SearchById(id);
                            var favorites = customer.Favorites.ToList();
                            foreach (var favorite in favorites) {
                                if (!islike) {
                                    foreach (var elem in favorite.Favoritestays.ToList()) {
                                        if (elem.StayId == stayId) {
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
            catch {

            }
            return message.ReturnJson();
        }

        [HttpGet("getDetailedStay")]
        public string GetStayDetailedInfo() {
            GetStayInfoMessage message = new GetStayInfoMessage();
            try {
                int stayId = int.Parse(Request.Query["stayID"]);
                var stay = SearchById(stayId);
                if (stay != null) {
                    message.errorCode = 200;
                    StayDetailedInfo info = new StayDetailedInfo();
                    info.stayName = stay.StayName;
                    var rooms = stay.Rooms.ToList();
                    int bathroom = (int)stay.PublicToilet;
                    var photos = new List<string>();
                    foreach (var room in rooms) {
                        bathroom += (int)room.BathroomNum;
                        foreach (var pic in room.RoomPhotos) {
                            photos.Add(pic.RPhoto);
                        }
                    }
                    info.stayDescribe = rooms.Count.ToString() + "室" + bathroom.ToString() + "卫";
                    info.stayPhotos = photos;
                    info.hostAvatar = stay.Host.HostAvatar;
                    if (stay.CommentNum != 0) {
                        info.stayScore = (double)stay.CommentScore / (double)stay.CommentNum;
                    }
                    else {
                        info.stayScore = 0;
                    }
                    bool islike = false;
                    StringValues token = default(StringValues);
                    if (Request.Headers.TryGetValue("token", out token)) {
                        var data = Token.VerifyToken(token);
                        if (data != null) {
                            int id = int.Parse(data["id"]);
                            var customer = CustomerController.SearchById(id);
                            var favorites = customer.Favorites.ToList();
                            foreach (var favorite in favorites) {
                                if (!islike) {
                                    foreach (var elem in favorite.Favoritestays.ToList()) {
                                        if (elem.StayId == stayId) {
                                            islike = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    info.isLike = islike;
                    info.stayCommentNum = (int)stay.CommentNum;
                    info.stayLabel = myContext.StayLabels.Where(s => s.StayId == stay.StayId).Select(c => c.LabelName).ToList();
                    info.stayPrice = GetMinPrice(stay.Rooms.ToList());
                    info.stayPosition = new decimal[] { stay.Longitude, stay.Latitude };
                    message.data["stayPositionNum"] = 1;
                    message.data["stayPositionInfo"] = info;
                }

            }
            catch {

            }
            return message.ReturnJson();
        }

        class RoomInfo {
            public int roomId { get; set; }
            public int price { get; set; }
            public decimal? roomArea { get; set; }
            public byte? bathNum { get; set; }
            public string[] bedTypes { get; set; }
            public byte[] bedNums { get; set; }
            public string[] images { get; set; }
        }

        [HttpPost("infos")]
        public string AddNewStay() {
            Message message = new Message();
            message.errorCode = 300;
            try {
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
                stay.StartTime = Request.Form["startTime"];
                stay.EndTime = Request.Form["endTime"];
                stay.DaysMax = byte.Parse(Request.Form["maxDay"]);
                stay.DaysMin = byte.Parse(Request.Form["minDay"]);
                stay.StayStatus = decimal.Parse(Request.Form["stayStatus"]);
                stay.CommentNum = 0;
                stay.CommentScore = 0;
                StringValues token = default(StringValues);
                int? hostId = null;
                if (Request.Headers.TryGetValue("token", out token)) {
                    var data = Token.VerifyToken(token);
                    if (data != null) {
                        myContext.DetachAll();
                        int id = int.Parse(data["id"]);
                        var host = HostController.SearchById(id);
                        if (host != null) {
                            hostId = host.HostId;
                        }
                    }
                }
                if (hostId != null) {
                    stay.HostId = hostId;
                }
                else {
                    throw (null);
                }
                myContext.Stays.Add(stay);
                myContext.SaveChanges();
                //TODO:test
                var rooms = JsonSerializer.Deserialize<List<RoomInfo>>(Request.Form["roomInfo"]);
                foreach (var room in rooms) {
                    Room newRoom = new Room();
                    newRoom.StayId = stay.StayId;
                    newRoom.RoomId = room.roomId;
                    newRoom.Price = room.price;
                    newRoom.RoomArea = room.roomArea;
                    newRoom.BathroomNum = room.bathNum;
                    myContext.Rooms.Add(newRoom);
                    myContext.SaveChanges();
                    for (int i = 0; i < room.bedTypes.Length; i++) {
                        if (room.bedNums[i] > 0) {
                            RoomBed roomBed = new RoomBed();
                            roomBed.BedType = room.bedTypes[i];
                            roomBed.BedNum = room.bedNums[i];
                            roomBed.RoomId = newRoom.RoomId;
                            roomBed.StayId = stay.StayId;
                            myContext.RoomBeds.Add(roomBed);
                        }//全部插入后再保存
                    }
                    myContext.SaveChanges();
                    for (int i = 0; i < room.images.Length; i++) {
                        string url = PhotoUpload.UploadPhoto(room.images[i], "roomPhoto/" + stay.StayId + '-' + newRoom.RoomId + '-' + i.ToString());
                        if (url != null) {
                            var photo = new RoomPhoto();
                            photo.StayId = stay.StayId;
                            photo.RoomId = newRoom.RoomId;
                            photo.RPhoto = url;
                            myContext.RoomPhotos.Add(photo);
                        }//目前上传失败不抛出异常
                    }
                    myContext.SaveChanges();

                    // 插入tag
                    var tags = JsonSerializer.Deserialize<List<string>>(Request.Form["stayTags"]);
                    foreach(var tag in tags) {
                        myContext.StayLabels.Add(
                            new StayLabel() {
                                StayId = stay.StayId,
                                LabelName = tag,
                            }
                        );
                    }
                    myContext.SaveChanges();

                }
                message.errorCode = 200;
            }
            catch {

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
                    stay.StartTime = Request.Form["startTime"];
                    stay.EndTime = Request.Form["endTime"];
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
                                                                roomBed.BedType = room.bedTypes[i];
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

                    // 插入tag
                    var tags = JsonSerializer.Deserialize<List<string>>(Request.Form["stayTags"]);
                    foreach (var tag in tags) {
                        myContext.StayLabels.Add(
                            new StayLabel() {
                                StayId = stay.StayId,
                                LabelName = tag,
                            }
                        );
                    }
                    message.errorCode = 200;
                }
                catch
                {

                }
            }
            return message.ReturnJson();
        }

        [HttpGet("infos")]
        public string GetStayInfos(int stayId = -1) {
            GetStayInfosMessage message = new GetStayInfosMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    try {
                        var stay = myContext.Stays.Single(b => b.StayId == stayId);
                        message.data["stayType"] = stay.StayType;
                        message.data["maxTenantNum"] = stay.StayCapacity;
                        message.data["roomNum"] = stay.RoomNum;
                        message.data["bedNum"] = stay.BedNum;
                        message.data["pubRestNum"] = stay.PublicToilet;
                        message.data["pubBathNum"] = stay.PublicBathroom;
                        message.data["barrierFree"] = stay.NonBarrierFacility == 0 ? false : true;
                        message.data["Longitude"] = stay.Longitude;
                        message.data["Latitude"] = stay.Latitude;
                        message.data["stayName"] = stay.StayName;
                        message.data["stayChars"] = stay.Characteristic;
                        message.data["startTime"] = stay.StartTime;
                        message.data["endTime"] = stay.EndTime;
                        message.data["maxDay"] = stay.DaysMax;
                        message.data["minDay"] = stay.DaysMin;
                        message.data["roomInfo"] = new List<RoomInfo>();
                        foreach(var room in stay.Rooms) {
                            var roomInfo = new RoomInfo();
                            roomInfo.roomId = room.RoomId;
                            roomInfo.price = room.Price;
                            roomInfo.roomArea = room.RoomArea;
                            roomInfo.bathNum = room.BathroomNum;
                            var bedTypes = new List<string>();
                            var bedNums = new List<byte>();
                            var images = new List<string>();
                            foreach(var bed in room.RoomBeds) {
                                bedTypes.Add(bed.BedType);
                                bedNums.Add(bed.BedNum);
                            }
                            foreach (var roomPhoto in room.RoomPhotos)
                                images.Add(roomPhoto.RPhoto);
                            roomInfo.bedTypes = bedTypes.ToArray();
                            roomInfo.bedNums = bedNums.ToArray();
                            roomInfo.images = images.ToArray();
                            message.data["roomInfo"].Add(roomInfo);
                        }
                        message.data["stayStatus"] = stay.StayStatus;
                        message.data["stayTags"] = new List<string>();
                        foreach (var tag in stay.StayLabels)
                            message.data["stayTags"].Add(tag.LabelName);
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

        // 搜索
        [HttpGet("getStaysDetails")]
        public string GetStaysDetails(string name) {
            Message message = new Message();
            message.data.Add("staysDetails", new List<Dictionary<string, dynamic>>());
            try {
                var stayList = myContext.Stays.Where(b => b.StayStatus == 2 && b.StayName.Contains(name));
                foreach (var stay in stayList) {
                    var details = new Dictionary<string, dynamic>();
                    details["stayId"] = stay.StayId;
                    details["stayImages"] = new List<string>();
                    foreach (var room in stay.Rooms) {
                        foreach (var roomPhoto in room.RoomPhotos) {
                            details["stayImages"].Add(roomPhoto.RPhoto);
                        }
                    }
                    details["stayName"] = stay.StayName;
                    details["stayDescription"] = stay.Characteristic;
                    details["characteristic"] = stay.StayTypeNavigation.Characteristic;
                    details["hostAvatar"] = stay.Host.HostAvatar;
                    details["hostLevel"] = stay.Host.HostLevelNavigation == null ? null : stay.Host.HostLevelNavigation.HostLevelName;
                    details["hostCommentNum"] = stay.CommentNum;
                    details["stayPosition"] = new List<float> { (float)stay.Longitude, (float)stay.Latitude };
                    details["hostName"] = stay.Host.HostUsername;
                    details["roomNum"] = stay.Rooms.Count();
                    int bedNum = 0;
                    foreach (var room in stay.Rooms)
                        bedNum += room.RoomBeds.Count();
                    details["bedNum"] = bedNum;
                    details["stayCapacity"] = stay.StayCapacity;
                    details["publicBathroom"] = stay.PublicBathroom;
                    details["publicToilet"] = stay.PublicToilet;
                    details["nonBarrierFacility"] = stay.NonBarrierFacility == 0 ? false : true;
                    details["startTime"] = stay.StartTime;
                    details["endTime"] = stay.EndTime;
                    details["stayStatus"] = stay.StayStatus;
                    details["rooms"] = new List<Dictionary<string, dynamic>>();
                    foreach (var room in stay.Rooms) {
                        var dict = new Dictionary<string, dynamic>();
                        dict.Add("id", room.RoomId);
                        dict.Add("area", room.RoomArea);
                        dict.Add("bathroom", room.BathroomNum);
                        dict.Add("price", room.Price);
                        int roomCapacity = 0;
                        foreach (var roomBed in room.RoomBeds)
                            roomCapacity += roomBed.BedTypeNavigation.PersonNum;
                        dict.Add("roomCapacity", roomCapacity);
                        dict.Add("roomImage", new List<string>());
                        foreach (var roomPhoto in room.RoomPhotos)
                            dict["roomImage"].Add(roomPhoto.RPhoto);
                        dict.Add("beds", new List<Dictionary<string, dynamic>>());
                        foreach (var roomBed in room.RoomBeds)
                            dict["beds"].Add(new Dictionary<string, dynamic> { { "bedType", roomBed.BedType }, { "num", roomBed.BedNum } });
                        dict.Add("unavailable", new List<Dictionary<string, DateTime>>());
                        var unavailableList = myContext.Generates.Where(b => b.StayId == stay.StayId).Distinct().ToList();
                        foreach (var unavailable in unavailableList)
                            dict["unavailable"].Add(new Dictionary<string, DateTime> { { "startData", unavailable.StartTime }, { "endData", unavailable.EndTime } });
                        details["rooms"].Add(dict);
                    }
                    message.data["staysDetails"].Add(details);
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

        // 获取房源详细信息
        [HttpGet("getStayDetails")]
        public string GetStayDetails(int stayId = -1) {
            GetStayDetailsMessage message = new GetStayDetailsMessage();
            try {
                var stay = myContext.Stays.Single(b => b.StayId == stayId);
                message.data["stayId"] = stayId;
                message.data["stayImages"] = new List<string>();
                foreach (var room in stay.Rooms) {
                    foreach (var roomPhoto in room.RoomPhotos) {
                        message.data["stayImages"].Add(roomPhoto.RPhoto);
                    }
                }
                message.data["stayName"] = stay.StayName;
                message.data["stayDescription"] = stay.Characteristic;
                message.data["characteristic"] = stay.StayTypeNavigation.Characteristic;
                message.data["hostAvatar"] = stay.Host.HostAvatar;
                message.data["hostLevel"] = stay.Host.HostLevelNavigation == null ? null : stay.Host.HostLevelNavigation.HostLevelName;
                message.data["hostCommentNum"] = stay.CommentNum;
                message.data["stayPosition"] = new List<float> { (float)stay.Longitude, (float)stay.Latitude };
                message.data["hostName"] = stay.Host.HostUsername;
                message.data["roomNum"] = stay.Rooms.Count();
                int bedNum = 0;
                foreach (var room in stay.Rooms)
                    bedNum += room.RoomBeds.Count();
                message.data["bedNum"] = bedNum;
                message.data["stayCapacity"] = stay.StayCapacity;
                message.data["publicBathroom"] = stay.PublicBathroom;
                message.data["publicToilet"] = stay.PublicToilet;
                message.data["nonBarrierFacility"] = stay.NonBarrierFacility == 0 ? false : true;
                message.data["startTime"] = stay.StartTime;
                message.data["endTime"] = stay.EndTime;
                message.data["stayStatus"] = stay.StayStatus;
                message.data["rooms"] = new List<Dictionary<string, dynamic>>();
                foreach (var room in stay.Rooms) {
                    var dict = new Dictionary<string, dynamic>();
                    dict.Add("id", room.RoomId);
                    dict.Add("area", room.RoomArea);
                    dict.Add("bathroom", room.BathroomNum);
                    dict.Add("price", room.Price);
                    int roomCapacity = 0;
                    foreach (var roomBed in room.RoomBeds)
                        roomCapacity += roomBed.BedTypeNavigation.PersonNum;
                    dict.Add("roomCapacity", roomCapacity);
                    dict.Add("roomImage", new List<string>());
                    foreach (var roomPhoto in room.RoomPhotos)
                        dict["roomImage"].Add(roomPhoto.RPhoto);
                    dict.Add("beds", new List<Dictionary<string, dynamic>>());
                    foreach (var roomBed in room.RoomBeds)
                        dict["beds"].Add(new Dictionary<string, dynamic> { { "bedType", roomBed.BedType }, { "num", roomBed.BedNum } });
                    dict.Add("unavailable", new List<Dictionary<string, DateTime>>());
                    var unavailableList = myContext.Generates.Where(b => b.StayId == stayId).Distinct().ToList();
                    foreach (var unavailable in unavailableList)
                        dict["unavailable"].Add(new Dictionary<string, DateTime> { { "startData", unavailable.StartTime }, { "endData", unavailable.EndTime } });
                    message.data["rooms"].Add(dict);
                }
                message.errorCode = 200;
                return message.ReturnJson();
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                message.errorCode = 300;
                return message.ReturnJson();
            }
        }


        [HttpGet("getComments")]
        // 获取房源评价
        public string GetComments(int stayId = -1) {
            GetCommentsMessage message = new GetCommentsMessage();
            try {
                var stay = myContext.Stays.Single(b => b.StayId == stayId);
                message.data["ratings"] = stay.CommentScore == 0 ? 0 : (float)stay.CommentScore / (float)stay.CommentNum;
                message.data["commentNum"] = stay.CommentNum;
                message.data["comments"] = new List<Dictionary<string, dynamic>>();
                var comments = myContext.CustomerComments.Where(b => b.Order.Generates.First().StayId == stayId).ToList();
                for(int i = 0; i < comments.Count; i++) {
                    message.data["comments"].Add(
                        new Dictionary<string, dynamic> {
                            {"id", i},
                            {"nickName", comments[i].Order.Customer.CustomerName },
                            {"avatar", comments[i].Order.Customer.CustomerPhoto },
                            {"date",comments[i].CommentTime },
                            {"commentContent", comments[i].CustomerComment1 }
                        }    
                    );
                }
                message.errorCode = 200;
                return message.ReturnJson();
            }
            catch (Exception e){
                Console.WriteLine(e.ToString());
                message.errorCode = 300;
                return message.ReturnJson();
            }
        }

        [HttpGet("getPrice")]
        public string GetRoomPrice(DateTime startDate, DateTime endDate, int stayId = -1, int roomId = -1) {
            GetPriceMessage message = new GetPriceMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    try {
                        int customerId = int.Parse(data["id"]);
                        int roomPrice = myContext.Rooms.Single(b => b.StayId == stayId && b.RoomId == roomId).Price;
                        TimeSpan span = endDate.Subtract(startDate);
                        int price = roomPrice * span.Days;
                        message.data["perPrice"] = roomPrice;
                        message.data["dateCount"] = span.Days;
                        message.data["priceWithoutCoupon"] = price;
                        message.data["serviceFee"] = 0;
                        var couponList = myContext.Coupons.Where(b => b.CustomerId == customerId).ToList();
                        var useCoupon = new Dictionary<string, dynamic> {
                            { "couponId", null },
                            { "couponAvailable" , false},
                            { "couponName", null },
                            { "couponValue", 0 }
                        };
                        foreach (var coupon in couponList) {
                            if (price >= coupon.CouponType.CouponLimit && useCoupon["couponValue"] < coupon.CouponType.CouponAmount) {
                                useCoupon["couponAvailable"] = true;
                                useCoupon["couponId"] = coupon.CouponId;
                                useCoupon["couponName"] = coupon.CouponType.CouponName;
                                useCoupon["couponValue"] = coupon.CouponType.CouponAmount;
                            }
                        }
                        message.data["couponUsage"] = useCoupon;
                        message.data["totalPrice"] = price - useCoupon["couponValue"];
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
            else {
                try {
                    int roomPrice = myContext.Rooms.Single(b => b.StayId == stayId && b.RoomId == roomId).Price;
                    TimeSpan span = endDate.Subtract(startDate);
                    int price = roomPrice * span.Days;
                    message.data["perPrice"] = roomPrice;
                    message.data["dateCount"] = span.Days;
                    message.data["priceWithoutCoupon"] = price;
                    message.data["serviceFee"] = 0;
                    var useCoupon = new Dictionary<string, dynamic> {
                            { "couponAvailable" , false},
                            { "couponName", null },
                            { "couponValue", 0 }
                        };
                    message.data["couponUsage"] = useCoupon;
                    message.data["totalPrice"] = price - useCoupon["couponValue"];
                    message.errorCode = 200;
                    return message.ReturnJson();
                }
                catch (Exception e) {
                    Console.WriteLine(e.ToString());
                    message.errorCode = 300;
                    return message.ReturnJson();
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
                        var stay = myContext.Stays.Single(b => b.StayId == stayId);
                        stay.StayStatus = 4;
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

        [HttpGet("tag")]
        public string GetAllTags()
        {
            GetStayTagMessage message = new GetStayTagMessage();
            try
            {
                List<string> tagList = myContext.StayLabels.Select(s => s.LabelName).ToList();
                message.errorCode = 200;
                message.data["tagList"] = tagList;
            }
            catch
            { }
            return message.ReturnJson();

        }

        // 获取房东某个房源的订单数据
        [HttpGet("StayOrderInfo")]
        public string GetStayOrderInfo(int stayId = -1) {
            StayOrderInfoMessage message = new StayOrderInfoMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    try {
                        var stay = myContext.Stays.Single(b => b.StayId == stayId);
                        message.data["averageScore"] =stay.CommentNum==0?0: (float)stay.CommentScore / (float)stay.CommentNum;
                        int year = DateTime.Now.Year;
                        int month = DateTime.Now.Month;
                        int day = DateTime.Now.Day - 1;
                        DateTime time = DateTime.Now.AddMonths(-month).AddDays(-day);
                        var orderInfoOfDateList = new List<Dictionary<string, dynamic>>();
                        var orderDateList = myContext.Orders.Where(b => DateTime.Compare((DateTime)b.OrderTime, time) > 0 && b.Generates != null && b.Generates.First().StayId == stayId);
                        message.data["orderInfoOfDateList"] = new List<Dictionary<string, dynamic>>();

                        int maleOrderNum = 0, femaleOrderNum = 0, unkownOrderNum = 0;
                        int orderNum0 = 0, orderNum1 = 0, orderNum2 = 0, orderNum3 = 0, orderNum4 = 0, orderNum5 = 0, orderNum6 = 0;
                        foreach (var order in myContext.Orders.Where(b => b.Generates != null && b.Generates.First().StayId== stayId)) {
                            if (order.Customer.CustomerGender == null)
                                unkownOrderNum++;
                            else if (order.Customer.CustomerGender == "f")
                                femaleOrderNum++;
                            else
                                maleOrderNum++;
                            if (order.Customer.CustomerBirthday == null)
                                orderNum0++;
                            else {
                                DateTime birthday = (DateTime)order.Customer.CustomerBirthday;
                                if (DateTime.Now.Year - birthday.Year <= 10)
                                    orderNum1++;
                                else if (DateTime.Now.Year - birthday.Year <= 20)
                                    orderNum2++;
                                else if (DateTime.Now.Year - birthday.Year <= 30)
                                    orderNum3++;
                                else if (DateTime.Now.Year - birthday.Year <= 40)
                                    orderNum4++;
                                else if (DateTime.Now.Year - birthday.Year <= 50)
                                    orderNum5++;
                                else
                                    orderNum6++;
                            }
                        }
                        message.data["orderOfSexList"] = new Dictionary<string, dynamic> {
                            { "maleOrderNum", maleOrderNum },
                            { "femaleOrderNum", femaleOrderNum },
                            { "unkownOrderNum", unkownOrderNum }
                        };

                        message.data["orderInfoOfAgeList"] = new Dictionary<string, dynamic> {
                            {"orderNum0",orderNum0 },
                            {"orderNum1",orderNum1 },
                            {"orderNum2",orderNum2},
                            {"orderNum3",orderNum3 },
                            {"orderNum4",orderNum4 },
                            {"orderNum5",orderNum5 },
                            {"orderNum6",orderNum6 }
                        };

                        for(int i = 1; i <= 12; i++) {
                            message.data["orderInfoOfDateList"].Add(
                            new Dictionary<string, dynamic> {
                                {"data", year.ToString() + "-" + i.ToString() + "月" },
                                {"orderNum", 0 },
                                {"reviewNUm", 0 },
                                {"averageScore", 0 },
                                {"totalScore",0 }
                            }
                        );
                        }

                        foreach (var order in orderDateList) {
                            var index = ((DateTime)order.OrderTime).Month - 1;
                            message.data["orderInfoOfDateList"][index]["orderNum"]++;
                            if (order.CustomerComment != null) {
                                message.data["orderInfoOfDateList"][index]["reviewNum"]++;
                                message.data["orderInfoOfDateList"][index]["totalScore"] += (int)order.CustomerComment.HouseStars;
                            }                           
                        }

                        for (int i = 0; i < 12; i++) {
                            message.data["orderInfoOfDateList"][i]["averageScore"] =
                                message.data["orderInfoOfDateList"][i]["totalScore"] == 0 ? 0 :
                                (float)message.data["orderInfoOfDateList"][i]["totalScore"] /
                                (float)message.data["orderInfoOfDateList"][i]["reviewNum"];
                        }

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
    }
}
