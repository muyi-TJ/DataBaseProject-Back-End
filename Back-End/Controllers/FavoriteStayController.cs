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

        public class FavoriteStayMessage {
            public int errorCode { get; set; }
            public Dictionary<string, dynamic> data { get; set; } = new Dictionary<string, dynamic>();

            public FavoriteStayMessage() {
                errorCode = 400;
                data.Add("isSuccess", false);
                data.Add("msg", "invalid token");
            }
            public string ReturnJson() {
                return JsonSerializer.Serialize(this);
            }
        }

        public class StayInfo {
            public int stayId { get; set; }
            public string name { get; set; }
        }

        [HttpDelete]
        public string DeleteFavorite(int favoriteId = -1, int stayId = -1) {
            FavoriteStayMessage message = new FavoriteStayMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    var context = ModelContext.Instance;
                    context.DetachAll();

                    try {
                        Favoritestay favoritestay = new Favoritestay() {
                            FavoriteId = favoriteId,
                            StayId = stayId
                        };
                        context.Remove(favoritestay);
                        context.SaveChanges();
                        message.errorCode = 200;
                        message.data["isSuccess"] = true;
                        message.data["msg"] = "success";
                        return message.ReturnJson();
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.ToString());
                        message.data["msg"] = "invalid favoriteId or stayId";
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
                    var context = ModelContext.Instance;
                    context.DetachAll();

                    List<StayInfo> stayList = new List<StayInfo>();
                    // 如果不存在这个收藏夹
                    if (!context.Favorites.Any(b => b.FavoriteId == favoriteId)) {
                        message.data["msg"] = "invalid favoriteId";
                        message.data.Add("favoriteList", stayList);
                        return JsonSerializer.Serialize(message);
                    }
                    try {
                        var stayIdList = context.Favoritestays.Where(b => b.FavoriteId == favoriteId).Select(b => b.StayId).ToList();
                        message.errorCode = 200;
                        message.data["isSuccess"] = true;
                        message.data["msg"] = "success";

                        foreach (var stayId in stayIdList) {
                            var stay = context.Stays.Single(b => b.StayId == stayId);
                            stayList.Add(new StayInfo() {
                                stayId = stay.StayId,
                                name = stay.StayName
                            });
                        }
                        message.data.Add("stayList", stayList);
                        return message.ReturnJson();
                    }
                    catch(Exception e) {
                        Console.WriteLine(e.ToString());
                        message.data["msg"] = "invalid favoriteId";
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
                    var context = ModelContext.Instance;
                    context.DetachAll();

                    // 如果不存在这个收藏夹
                    if (!context.Favorites.Any(b => b.FavoriteId == favoriteId)) {
                        message.data["msg"] = "invalid favoriteId";
                        return message.ReturnJson();
                    }
                    // 如果已经添加了
                    if (context.Favoritestays.Any(b => b.FavoriteId == favoriteId && b.StayId == stayId)) {
                        message.data["msg"] = "this stay already in the favorite";
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
                        message.data["isSuccess"] = true;
                        message.data["msg"] = "success";

                        return message.ReturnJson();
                    }
                    catch(Exception e) {
                        Console.WriteLine(e.ToString());
                        message.data["msg"] = "invalid favoriteId or stayId";
                        return message.ReturnJson();
                    }
                }
            }
            return message.ReturnJson();  
        }
    }
}
