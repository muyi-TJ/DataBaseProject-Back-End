using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Specialized;
using Back_End.Contexts;
using Back_End.Models;
using System.Text.Json;

namespace Back_End.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class FavoriteStayController : ControllerBase {

        public class FavoriteStayMessage {
            public int errorCode { get; set; }
            public Dictionary<string, dynamic> data { get; set; } = new Dictionary<string, dynamic>();
        }

        public class StayMessage {
            public int stayId { get; set; }
            public string name { get; set; }
        }

        [HttpDelete]
        public string DeleteFavorite(int favoriteId = -1, int stayId = -1) {
            //Console.WriteLine("Favorite Id:", favoriteId, "Stay Id:", stayId);
            var context = ModelContext.Instance;
            context.DetachAll();
            FavoriteStayMessage message = new FavoriteStayMessage();
            try {
                Favoritestay favoritestay = new Favoritestay() {
                    FavoriteId = favoriteId,
                    StayId = stayId
                };
                context.Remove(favoritestay);
                context.SaveChanges();
                message.errorCode = 200;
                message.data.Add("isSuccess", true);
                message.data.Add("msg", "success");
                return JsonSerializer.Serialize(message);
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                message.errorCode = 400;
                message.data.Add("isSuccess", false);
                message.data.Add("message", "invalid favoriteId or stayId");
                return JsonSerializer.Serialize(message);
            }
        }

        [HttpGet]
        public string GetFavoriteStay(int favoriteId = -1) {
            Console.WriteLine("Favorite Id:", favoriteId);
            var context = ModelContext.Instance;
            context.DetachAll();
            FavoriteStayMessage message = new FavoriteStayMessage();
            List<Stay> stayList = new List<Stay>();
            // 如果不存在这个收藏夹
            if (!context.Favorites.Any(b => b.FavoriteId == favoriteId)) {
                message.errorCode = 400;
                message.data.Add("isSuccess", false);
                message.data.Add("message", "invalid favoriteId");
                message.data.Add("favoriteList", stayList);
                return JsonSerializer.Serialize(message);
            }

            var stayIdList = context.Favoritestays.Where(b => b.FavoriteId == favoriteId).Select(b => b.StayId).ToList();
            message.errorCode = 200;
            message.data.Add("isSuccess", true);
            message.data.Add("message", "success");

            foreach (var stayId in stayIdList) {
                stayList.Add(context.Stays.Single(b => b.StayId == stayId));
                
            }
            message.data.Add("stayList", stayList);
            return JsonSerializer.Serialize(message);
        }

        [HttpPost]
        public string InsertFavoriteStay() {
            int favoriteId = int.Parse(Request.Form["favoriteId"]);
            int stayId = int.Parse(Request.Form["stayId"]);
            var context = ModelContext.Instance;
            context.DetachAll();
            FavoriteStayMessage message = new FavoriteStayMessage();
            // 如果不存在这个收藏夹
            if (!context.Favorites.Any(b => b.FavoriteId == favoriteId)) {
                message.errorCode = 400;
                message.data.Add("isSuccess", false);
                message.data.Add("message", "invalid favoriteId");
                return JsonSerializer.Serialize(message);
            }
            if (context.Favoritestays.Any(b => b.FavoriteId == favoriteId && b.StayId == stayId)) {
                message.errorCode = 400;
                message.data.Add("isSuccess", false);
                message.data.Add("message", "this stay already in the favorite");
                return JsonSerializer.Serialize(message);
            }
            Favoritestay favoritestay = new Favoritestay() {
                FavoriteId = favoriteId,
                StayId = stayId
            };
            context.Add(favoritestay);
            context.SaveChanges();

            message.errorCode = 200;
            message.data.Add("isSuccess", true);
            message.data.Add("message", "success");
            return JsonSerializer.Serialize(message);
        }
    }
}
