﻿using Microsoft.AspNetCore.Mvc;
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
                var stays = myContext.Stays.Where(s => IsInMap(left, right, up, down, s.Latitude, s.Longitude,cross))
                    .Select(c=>new StayInMapInfo{stayID=c.StayId,stayPrice=c.Rooms.First().Price,
                        stayPosition = new decimal[]{ c.Longitude, c.Latitude } }).ToList();
                message.errorCode = 200;
                message.data["stayPositionNum"] = stays.Count;
                message.data["stayPositionInfo"] = stays;
            }
            catch
            {

            }
            return message.ReturnJson();
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
                    info.stayPrice = stay.Rooms.First().Price;
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

    }
}
