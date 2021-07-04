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
    [Route("[controller]")]
    public class CustomerFavoriteController : ControllerBase {

        public class CustomerFavoriteMessage {
            public int errorCode { get; set; }
            public Dictionary<string, dynamic> data { get; set; } = new Dictionary<string, dynamic>();
        }

        public class FavoriteMessage {
            public int favoriteId { get; set; }
            public string name { get; set; }
            public int totalStay { get; set; }
        }

        [HttpDelete]
        public string DeleteFavorite(int favoriteId = -1) {
            Console.WriteLine("Favorite Id:", favoriteId);
            var context = ModelContext.Instance;
            context.DetachAll();
            CustomerFavoriteMessage message = new CustomerFavoriteMessage();
            try {
                Favorite favorite = new Favorite() { FavoriteId = favoriteId };
                context.Remove(favorite);
                context.SaveChanges();
                message.errorCode = 200;
                message.data.Add("isSuccess", true);
                message.data.Add("msg", "success");
                return JsonSerializer.Serialize(message);
            }
            catch(Exception e) {
                Console.WriteLine(e.ToString());
                message.errorCode = 400;
                message.data.Add("isSuccess", false);
                message.data.Add("message", "invalid favoriteId");
                return JsonSerializer.Serialize(message);
            }
        }

        [HttpGet]
        public string GetCustomerFavorite(int customerId = -1) {
            Console.WriteLine("Customer Id:", customerId);
            var context = ModelContext.Instance;
            context.DetachAll();
            CustomerFavoriteMessage message = new CustomerFavoriteMessage();
            List<FavoriteMessage> favoriteList = new List<FavoriteMessage>();
            // 如果不存在这个用户
            if (!context.Customers.Any(b => b.CustomerId == customerId)) {
                message.errorCode = 400;
                message.data.Add("isSuccess", false);
                message.data.Add("message", "invalid customerId");
                message.data.Add("favoriteList", favoriteList);
                return JsonSerializer.Serialize(message);
            }

            var favorites = context.Favorites.Where(b => b.CustomerId == customerId).ToList();

            message.errorCode = 200;
            message.data.Add("isSuccess", true);
            message.data.Add("message", "success");

            foreach (var favorite in favorites) {
                int totalStay = context.Favoritestays.Count(b => b.FavoriteId == favorite.FavoriteId);
                favoriteList.Add(new FavoriteMessage {
                    favoriteId = favorite.FavoriteId,
                    name = favorite.Name,
                    totalStay = totalStay
                });
            }
            message.data.Add("favoriteList", favoriteList);
            return JsonSerializer.Serialize(message);
        }

        [HttpPost]
        public string InsertFavorite() {
            int customerId = int.Parse(Request.Form["customerId"]);
            string name = Request.Form["name"];

            var context = ModelContext.Instance;
            context.DetachAll();
            CustomerFavoriteMessage message = new CustomerFavoriteMessage();
            // 如果不存在这个用户
            if (!context.Customers.Any(b => b.CustomerId == customerId)) {
                message.errorCode = 400;
                message.data.Add("isSuccess", false);
                message.data.Add("message", "invalid customerId");
                message.data.Add("favoriteId", null);
                return JsonSerializer.Serialize(message);
            }
            if (context.Favorites.Any(b => b.CustomerId == customerId && b.Name == name)) {
                message.errorCode = 400;
                message.data.Add("isSuccess", false);
                message.data.Add("message", "already have the favorite name");
                message.data.Add("favoriteId", null);
                return JsonSerializer.Serialize(message);
            }
 
            Favorite favorite = new Favorite();
            favorite.CustomerId = customerId;
            favorite.Name = name;
            context.Add(favorite);
            context.SaveChanges();
            
            message.errorCode = 200;
            message.data.Add("isSuccess", true);
            message.data.Add("message", "success");
            message.data.Add("favoriteId", context.Favorites.Single(b => b.CustomerId == customerId && b.Name == name).FavoriteId);
            return JsonSerializer.Serialize(message);
        }

        [HttpGet("image")]
        public string GetFavoriteImage(int favoriteId = -1) {
            Console.WriteLine("Favorite Id:", favoriteId);
            var context = ModelContext.Instance;
            context.DetachAll();
            CustomerFavoriteMessage message = new CustomerFavoriteMessage();
            try {
                var favoriteStay = context.Favoritestays.Where(b => b.FavoriteId == favoriteId).FirstOrDefault();
                var roomPhoto = context.RoomPhotos.Where(b => b.StayId == favoriteStay.StayId).First();

                message.errorCode = 200;
                message.data.Add("isSuccess", true);
                message.data.Add("msg", "success");
                message.data.Add("imageUrl", roomPhoto.RPhoto);
                return JsonSerializer.Serialize(message);
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                message.errorCode = 400;
                message.data.Add("isSuccess", false);
                message.data.Add("message", "this favorite have no stay");
                message.data.Add("imageUrl", null);
                return JsonSerializer.Serialize(message);
            }
        }
    }

}
