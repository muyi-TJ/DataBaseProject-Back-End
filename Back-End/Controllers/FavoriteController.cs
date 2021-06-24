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
    public class FavoriteController: ControllerBase {



        [HttpGet("GetCustomerFavorite")]
        public string GetCustomerFavorite(int customerId=-1) {
            Console.WriteLine(customerId);
            var content = ModelContext.Instance;
            content.DetachAll();
            CustomerFavoriteMessage message = new CustomerFavoriteMessage();
            message.data = new Dictionary<string, dynamic>();
            List<FavoriteMessage> favoriteList = new List<FavoriteMessage>();
            if (!content.Customers.Any(b => b.CustomerId == customerId)) {
                message.errorCode = 400;
                message.data.Add("isSuccess", false);
                message.data.Add("message", "invalid customerId");
                message.data.Add("favoriteList", favoriteList);
                return JsonSerializer.Serialize(message);
            }
            var favorites = content.Favorites.Where(b => b.CustomerId == customerId).ToList();
            
            message.errorCode = 200;//反正是表示成功了
            message.data.Add("isSuccess", true);
            message.data.Add("message", "success");

            foreach(var favorite in favorites) {
                int totalStay = content.Favoritestays.Count(b => b.FavoriteId == favorite.FavoriteId);
                favoriteList.Add(new FavoriteMessage {
                    favoriteId = favorite.FavoriteId,
                    name = favorite.Name,
                    totalStay = totalStay
                });
            }
            message.data.Add("favoriteList", favoriteList);
            return JsonSerializer.Serialize(message);
        }

        [HttpPost("InsertFavorite")]
        public string InsertFavorite() {
            int customerId = int.Parse(Request.Form["customerId"]);
            string name = Request.Form["name"];

            var content = ModelContext.Instance;
            content.DetachAll();
            CustomerFavoriteMessage message = new CustomerFavoriteMessage();
            message.data = new Dictionary<string, dynamic>();
            if (!content.Customers.Any(b => b.CustomerId == customerId)) {
                message.errorCode = 400;
                message.data.Add("isSuccess", false);
                message.data.Add("message", "invalid customerId");
                message.data.Add("favoriteId", null);
                return JsonSerializer.Serialize(message);
            }
            if (content.Favorites.Any(b => b.CustomerId == customerId && b.Name == name)) {
                message.errorCode = 400;
                message.data.Add("isSuccess", false);
                message.data.Add("message", "already have the name");
                message.data.Add("favoriteId", null);
                return JsonSerializer.Serialize(message);
            }
            Favorite favorite = new Favorite();
            favorite.CustomerId = customerId;
            favorite.Name = name;
            favorite.FavoriteId = content.Favorites.Count();
            content.Add(favorite);
            content.SaveChanges();

            message.errorCode = 200;
            message.data.Add("isSuccess", true);
            message.data.Add("message", "success");
            message.data.Add("favoriteId", favorite.FavoriteId);
            return JsonSerializer.Serialize(message);

        }
    }

    public class CustomerFavoriteMessage {
        public int errorCode { get; set; }
        public Dictionary<string, dynamic> data { get; set; }
    }

    public class FavoriteMessage {
        public int favoriteId { get; set; }
        public string name { get; set; }
        public int totalStay { get; set; }
    }

}
