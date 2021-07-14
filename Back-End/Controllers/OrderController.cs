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
using Microsoft.EntityFrameworkCore;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;

namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ModelContext myContext;
        private readonly string postUrl = "http://trisbox.xyz:7001/api/order";
        private readonly string redirectUrl = "";
        private readonly string key = "wrnmsjk";
        public OrderController(ModelContext modelContext)
        {
            myContext = modelContext;
        }

        public static Order SearchById(int id)
        {
            try
            {
                ModelContext context = new ModelContext();
                var order = context.Orders
                    .Single(b => b.OrderId == id);
                return order;
            }
            catch
            {
                return null;
            }

        }

        class OrderInfo
        {
            public int orderId { get; set; }
            public List<string> stayImage { get; set; }
            public int stayId { get; set; }
            public string stayName { get; set; }
            public string stayLocation { get; set; }
            public DateTime startTime { get; set; }
            public DateTime endTime { get; set; }
            public decimal totalCost { get; set; }
            public string name { get; set; }
            public string photo { get; set; }
            public int hostId { get; set; }
            public decimal commentStars { get; set; }
            public string comment { get; set; }
        }


        [HttpGet("CustomerOrderInfo")]
        public string GetCustomerOrder()
        {
            GetOrderMessage message = new GetOrderMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var customer = CustomerController.SearchById(id);
                    if(customer!=null)
                    {
                        var orders = customer.Orders.ToList();
                        List<OrderInfo> orderInfos = new List<OrderInfo>();
                        foreach(var order in orders)
                        {
                            OrderInfo orderInfo = new OrderInfo();
                            orderInfo.orderId = order.OrderId;
                            Stay stay = order.Generates.First().Room.Stay;
                            orderInfo.stayId = stay.StayId;
                            orderInfo.stayName = stay.StayName;
                            orderInfo.stayLocation = stay.DetailedAddress;
                            orderInfo.startTime = order.Generates.First().StartTime;
                            orderInfo.endTime = order.Generates.First().EndTime;
                            orderInfo.name = stay.Host.HostUsername;
                            orderInfo.totalCost = order.TotalCost;
                            orderInfo.hostId =(int) stay.HostId;
                            orderInfo.photo = stay.Host.HostAvatar;
                            List<string> photos = new List<string>();
                            foreach(var room in stay.Rooms)
                            {
                                foreach(var photo in room.RoomPhotos)
                                {
                                    photos.Add(photo.RPhoto);
                                }
                            }
                            orderInfo.stayImage = photos;
                            if(order.CustomerComment!=null)
                            {
                                orderInfo.commentStars = order.CustomerComment.HouseStars;
                                orderInfo.comment = order.CustomerComment.CustomerComment1;
                            }
                            else
                            {
                                orderInfo.commentStars = 0;
                                orderInfo.comment = null;
                            }
                            orderInfos.Add(orderInfo);
                        }
                        message.data["customerOrderList"] = orderInfos;
                        message.errorCode = 200;
                    }
                }
            }
            return message.ReturnJson();
        }
        [HttpGet("HostOrderInfo")]
        public string GetHostOrder()
        {
            GetOrderMessage message = new GetOrderMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var host = HostController.SearchById(id);
                    if (host != null)
                    {
                        List<Order> orders = new List<Order>();
                        foreach(var stay in host.Stays)
                        {
                            orders.AddRange(myContext.Orders.Where(s => s.Generates.First().StayId == stay.StayId)
                                .Select(c => c).ToList());
                        }
                        List<OrderInfo> orderInfos = new List<OrderInfo>();
                        foreach (var order in orders)
                        {
                            OrderInfo orderInfo = new OrderInfo();
                            orderInfo.orderId = order.OrderId;
                            Stay stay = order.Generates.First().Room.Stay;
                            orderInfo.stayId = stay.StayId;
                            orderInfo.stayName = stay.StayName;
                            orderInfo.stayLocation = stay.DetailedAddress;
                            orderInfo.startTime = order.Generates.First().StartTime;
                            orderInfo.endTime = order.Generates.First().EndTime;
                            orderInfo.name = stay.Host.HostUsername;
                            orderInfo.totalCost = order.TotalCost;
                            orderInfo.hostId = (int)stay.HostId;
                            orderInfo.photo = stay.Host.HostAvatar;
                            List<string> photos = new List<string>();
                            foreach (var room in stay.Rooms)
                            {
                                foreach (var photo in room.RoomPhotos)
                                {
                                    photos.Add(photo.RPhoto);
                                }
                            }
                            orderInfo.stayImage = photos;
                            if (order.HostComment!= null)
                            {
                                orderInfo.commentStars = order.HostComment.CustomerStars;
                                orderInfo.comment = order.HostComment.HostComment1;
                            }
                            else
                            {
                                orderInfo.commentStars = 0;
                                orderInfo.comment = null;
                            }
                            orderInfos.Add(orderInfo);
                        }
                        message.data["customerOrderList"] = orderInfos;
                        message.errorCode = 200;
                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpPost("addHostComment")]
        public string AddHostComment()
        {
            Message message = new Message();
            message.errorCode = 400;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var host = HostController.SearchById(id);
                    if (host != null)
                    {
                        int orderId = int.Parse(Request.Form["orderId"]);
                        try
                        {
                            Order order = SearchById(orderId);
                            if (order != null)
                            {
                                HostComment hostComment = new HostComment();
                                hostComment.OrderId = orderId;
                                hostComment.CustomerStars = int.Parse(Request.Form["commentStars"]);
                                hostComment.HostComment1 = Request.Form["commentText"];
                                hostComment.CommentTime = DateTime.Now;
                                myContext.HostComments.Add(hostComment);
                                myContext.SaveChanges();
                                message.errorCode = 200;
                            }
                        }
                        catch
                        {

                        }
                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpPost("addCustomerComment")]
        public string AddCustomerComment()
        {
            Message message = new Message();
            message.errorCode = 400;
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var customer = CustomerController.SearchById(id);
                    if (customer != null)
                    {
                        int orderId = int.Parse(Request.Form["orderId"]);
                        try
                        {
                            Order order = SearchById(orderId);
                            if (order != null)
                            {
                                CustomerComment customerComment = new CustomerComment();
                                customerComment.OrderId = orderId;
                                customerComment.HouseStars = int.Parse(Request.Form["commentStars"]);
                                customerComment.CustomerComment1 = Request.Form["commentText"];
                                customerComment.CommentTime = DateTime.Now;
                                order.Generates.First().Room.Stay.CommentNum += 1;
                                order.Generates.First().Room.Stay.CommentScore +=(int?) customerComment.HouseStars;
                                myContext.CustomerComments.Add(customerComment);
                                myContext.SaveChanges();
                                message.errorCode = 200;
                            }
                        }
                        catch
                        {

                        }

                    }
                }
            }
            return message.ReturnJson();
        }

        [HttpPost("addOrder")]
        public string AddOrder()
        {
            AddOrderMessage message = new AddOrderMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if (data != null)
                {
                    int id = int.Parse(data["id"]);
                    var customer = CustomerController.SearchById(id);
                    if (customer != null)
                    {
                        try
                        {
                            Order order = new Order();
                            Generate generate = new Generate();
                            order.CustomerId = id;
                            order.OrderTime = DateTime.Now;
                            order.MemberNum = decimal.Parse(Request.Form["peopleNum"]);
                            int stayId = int.Parse(Request.Form["stayId"]);
                            int roomId = int.Parse(Request.Form["roomId"]);
                            decimal price = myContext.Rooms.Single(s => s.StayId == stayId && s.RoomId == roomId).Price;
                            generate.Money = price;
                            if (Request.Form["couponId"].ToString() != "")
                            {
                                int couponId = int.Parse(Request.Form["couponId"]);
                                var coupon = myContext.Coupons.Single(c => c.CouponId == couponId);
                                decimal amount = coupon.CouponType.CouponAmount;
                                price -= (int)amount;
                                myContext.Coupons.Remove(coupon);
                                
                            }
                            order.TotalCost = price;
                            myContext.Orders.Add(order);

                            generate.OrdersId = order.OrderId;
                            generate.RoomId = roomId;
                            generate.StayId = stayId;
                            generate.StartTime = DateTime.Parse(Request.Form["startDate"]);
                            generate.EndTime = DateTime.Parse(Request.Form["endDate"]);
                            myContext.Generates.Add(generate);
                            myContext.SaveChanges();

                            message.errorCode = 200;
                            MD5 md5 = MD5.Create();
                            byte[] strNoKey = md5.ComputeHash(Encoding.UTF8.GetBytes((order.OrderId + price).ToString()));
                            byte[] strWithKey = md5.ComputeHash(Encoding.UTF8.GetBytes(strNoKey.ToString() + key));
                            NameValueCollection parameters = new NameValueCollection();
                            parameters.Add("order_id", order.OrderId.ToString());
                            parameters.Add("order_type", "alipay");
                            parameters.Add("order_price", price.ToString());
                            parameters.Add("order_name", "归宿房费支付");
                            parameters.Add("redirect_url", redirectUrl);
                            parameters.Add("sign", strWithKey.ToString());
                            string postInfo = HttpClient.HttpPost(postUrl, parameters);
                            NameValueCollection collection = JsonSerializer.Deserialize<NameValueCollection>(postInfo);
                            message.data["isSuccess"] = true;
                            message.data["payUrl"] = collection["qr_url"];
                            message.data["redirectUrl"] = collection["redirect_url"];
                        }
                        catch
                        {

                        }

                    }
                }
            }
            return message.ReturnJson();
        }
    }
}
