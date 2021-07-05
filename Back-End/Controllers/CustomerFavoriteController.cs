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
    public class CustomerFavoriteController : ControllerBase {

        public class CustomerFavoriteMessage {
            public int errorCode { get; set; }
            public Dictionary<string, dynamic> data { get; set; } = new Dictionary<string, dynamic>();

            public CustomerFavoriteMessage() {
                errorCode = 400;
                data.Add("isSuccess", false);
                data.Add("msg", "invalid token");
            }
            public string ReturnJson() {
                return JsonSerializer.Serialize(this);
            }
        }


        public class FavoriteInfo {
            public int favoriteId { get; set; }
            public string name { get; set; }
            public int totalStay { get; set; }
        }

        [HttpDelete]
        public string DeleteFavorite(int favoriteId = -1) {
            CustomerFavoriteMessage message = new CustomerFavoriteMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                try {
                    var context = ModelContext.Instance;
                    context.DetachAll();
                    Favorite favorite = new Favorite() { FavoriteId = favoriteId };
                    context.Remove(favorite);
                    context.SaveChanges();
                    message.errorCode = 200;
                    message.data["isSuccess"] = true;
                    message.data["msg"] = "success";
                    return message.ReturnJson();
                }
                catch (Exception e) {
                    message.data["msg"] = "no such favorite";
                    Console.WriteLine(e.ToString());
                    return message.ReturnJson();
                }

            }
            return message.ReturnJson();
        }

        [HttpGet]
        public string GetCustomerFavorite() {
            CustomerFavoriteMessage message = new CustomerFavoriteMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    var context = ModelContext.Instance;
                    context.DetachAll();
                    int customerId = int.Parse(data["id"]);
                    List<FavoriteInfo> favoriteList = new List<FavoriteInfo>();
                    try {
                        var favorites = context.Favorites.Where(b => b.CustomerId == customerId).ToList();
                        foreach (var favorite in favorites) {
                            int totalStay = context.Favoritestays.Count(b => b.FavoriteId == favorite.FavoriteId);
                            favoriteList.Add(new FavoriteInfo {
                                favoriteId = favorite.FavoriteId,
                                name = favorite.Name,
                                totalStay = totalStay
                            });
                        }
                        message.errorCode = 200;
                        message.data["isSuccess"] = true;
                        message.data["msg"] = "success";
                        message.data.Add("favoriteList", favoriteList);
                        return message.ReturnJson();
                    }
                    catch(Exception e) {
                        Console.WriteLine(e.ToString());
                        return message.ReturnJson();
                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpPost]
        public string InsertFavorite() {
            string name = Request.Form["name"];

            CustomerFavoriteMessage message = new CustomerFavoriteMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    var context = ModelContext.Instance;
                    context.DetachAll();
                    int customerId = int.Parse(data["id"]);

                    if (context.Favorites.Any(b => b.CustomerId == customerId && b.Name == name)) {
                        message.data["msg"] = "aleady have the name";
                        return message.ReturnJson();
                    }
                    try {
                        Favorite favorite = new Favorite() {
                            CustomerId = customerId,
                            Name = name
                        };
                        context.Add(favorite);
                        context.SaveChanges();
                        message.errorCode = 200;
                        message.data["msg"] = "success";
                        message.data["isSuccess"] = true;
                        message.data.Add("favoriteId", favorite.FavoriteId);
                        return message.ReturnJson();
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.ToString());
                        return message.ReturnJson();
                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpGet("image")]
        public string GetFavoriteImage(int favoriteId = -1) {
            CustomerFavoriteMessage message = new CustomerFavoriteMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    var context = ModelContext.Instance;
                    context.DetachAll();
                    int customerId = int.Parse(data["id"]);

                    try {
                        var favoriteStay = context.Favoritestays.Where(b => b.FavoriteId == favoriteId).FirstOrDefault();
                        var roomPhoto = context.RoomPhotos.Where(b => b.StayId == favoriteStay.StayId).First();

                        message.errorCode = 200;
                        message.data["msg"] = "success";
                        message.data["isSuccess"] = true;
                        message.data.Add("imageUrl", roomPhoto.RPhoto);
                        return message.ReturnJson();
                    }
                    catch (Exception e) {
                        message.data["msg"] = "there is no stay in this favorite";
                        message.errorCode = 200;
                        message.data["isSuccess"] = true;
                        message.data.Add("imageUrl", null);
                        Console.WriteLine(e.ToString());
                        return message.ReturnJson();
                    }
                }
            }
            return message.ReturnJson();
        }
    }

}
