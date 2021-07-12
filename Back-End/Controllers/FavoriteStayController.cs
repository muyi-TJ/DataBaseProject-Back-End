using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Back_End.Contexts;
using Back_End.Models;
using System.Text.Json;
using Microsoft.Extensions.Primitives;

namespace Back_End.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteStayController : ControllerBase {

        private readonly ModelContext myContext;
        public FavoriteStayController(ModelContext modelContext)
        {
            myContext = modelContext;
        }
        public class FavoriteStayMessage {
            public int errorCode { get; set; }
            public Dictionary<string, dynamic> data { get; set; } = new Dictionary<string, dynamic>();

            public FavoriteStayMessage() {
                errorCode = 400;
            }
            public string ReturnJson() {
                return JsonSerializer.Serialize(this);
            }
        }

        public class StayInfo {
            public int stayId { get; set; }
            public string stayName { get; set; }
            public string stayCharacteristic { get; set; }
            public bool stayHasPath { get; set; } // 公共浴室
            public bool stayHasWashroom { get; set; } // 公共卫生间
            public bool stayHasFacility { get; set; } // 无障碍设施
            public float stayRate { get; set; } // 评分
            public int stayMinPrice { get; set; }
            public string stayPhoto { get; set; }
            public int commentNum { get; set; } // 评论数
            public string hostAvatar { get; set; }
        }

        [HttpDelete]
        public string DeleteFavorite(int favoriteId = -1, int stayId = -1) {
            FavoriteStayMessage message = new FavoriteStayMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    var context = myContext;
                    context.DetachAll();

                    try {
                        Favoritestay favoritestay = new Favoritestay() {
                            FavoriteId = favoriteId,
                            StayId = stayId
                        };
                        context.Remove(favoritestay);
                        context.SaveChanges();
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

        [HttpDelete("stay")]
        public string DeleteStay(int stayID = -1) {
            FavoriteStayMessage message = new FavoriteStayMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    var context = myContext;
                    context.DetachAll();

                    try {
                        int customerId = int.Parse(data["id"]);
                        context.RemoveRange(context.Favoritestays.Where(b => b.StayId == stayID && b.Favorite.CustomerId == customerId));
                        context.SaveChanges();
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


        [HttpGet]
        public string GetFavoriteStay(int favoriteId = -1) {
            FavoriteStayMessage message = new FavoriteStayMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    var context = myContext;
                    context.DetachAll();

                    List<StayInfo> stayList = new List<StayInfo>();
                    // 如果不存在这个收藏夹
                    if (!context.Favorites.Any(b => b.FavoriteId == favoriteId)) {
                        message.data.Add("favoriteList", stayList);
                        message.errorCode = 300;
                        return message.ReturnJson();
                    }
                    try {
                        var stayIdList = context.Favoritestays.Where(b => b.FavoriteId == favoriteId).Select(b => b.StayId).ToList();
                        
                        foreach (var stayId in stayIdList) {
                            var stay = context.Stays.Single(b => b.StayId == stayId);
                            int stayMinPrice = context.Rooms.Where(b => b.StayId == stayId).Min(x => x.Price);
                            string stayPhoto = context.RoomPhotos.Where(b => b.StayId == stayId).FirstOrDefault().RPhoto;
                            stayList.Add(new StayInfo() {
                                stayId = stay.StayId,
                                stayName = stay.StayName,
                                stayCharacteristic = stay.Characteristic,
                                stayHasPath = stay.PublicBathroom == 1 ? true : false,
                                stayHasWashroom = stay.PublicToilet == 1 ? true : false,
                                stayHasFacility = stay.NonBarrierFacility == 1 ? true : false,

                                stayRate = stay.CommentNum > 0 ? ((float)stay.CommentScore / (float)stay.CommentNum) : 0,
                                stayMinPrice = stayMinPrice,
                                stayPhoto = stayPhoto,
                                commentNum = (int)stay.CommentNum,
                                hostAvatar = context.Hosts.Single(b => b.HostId == stay.HostId).HostAvatar
                            }) ;
                        }
                        message.data.Add("stayList", stayList);
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

        [HttpPost]
        public string InsertFavoriteStay() {
            FavoriteStayMessage message = new FavoriteStayMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    int favoriteId = int.Parse(Request.Form["favoriteId"]);
                    int stayId = int.Parse(Request.Form["stayId"]);
                    var context = myContext;
                    context.DetachAll();

                    // 如果不存在这个收藏夹
                    if (!context.Favorites.Any(b => b.FavoriteId == favoriteId)) {
                        message.errorCode = 300;
                        return message.ReturnJson();
                    }
                    // 如果已经添加了
                    if (context.Favoritestays.Any(b => b.FavoriteId == favoriteId && b.StayId == stayId)) {
                        message.errorCode = 300;
                        return message.ReturnJson();
                    }
                    try {
                        Favoritestay favoritestay = new Favoritestay() {
                            FavoriteId = favoriteId,
                            StayId = stayId
                        };
                        context.Add(favoritestay);
                        context.SaveChanges();

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
    }
}
