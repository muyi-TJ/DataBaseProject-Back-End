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
using System.Net;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly ModelContext myContext;
        private readonly string postUrl = "http://pay.guisu.fun:7001/api/order";
        private readonly string redirectUrl = "https://api.guisu.fun:6001/api/Order/afterPay";
        private readonly string key = "asdfg";
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
            public decimal stayLongitude { get; set; }
            public decimal stayLatitude { get; set; }
            public DateTime startTime { get; set; }
            public DateTime endTime { get; set; }
            public decimal totalCost { get; set; }
            public string name { get; set; }
            public string photo { get; set; }
            public int id { get; set; }
            public decimal commentStars { get; set; }
            public string comment { get; set; }
        }


        class CustomerOrderInfo:OrderInfo
        {
            public int reportState { get; set; }
            public string reportReason { get; set; }
            public string reportReply { get; set; }
        }


        [HttpGet("CustomerOrderInfo")]
        public string GetCustomerOrder()
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
                    if(customer!=null)
                    {
                        var orders = customer.Orders.ToList();
                        List<CustomerOrderInfo> orderInfos = new List<CustomerOrderInfo>();
                        foreach(var order in orders)
                        {
                            CustomerOrderInfo orderInfo = new CustomerOrderInfo();
                            orderInfo.orderId = order.OrderId;
                            Stay stay = order.Generates.First().Room.Stay;
                            orderInfo.stayId = stay.StayId;
                            orderInfo.stayName = stay.StayName;
                            orderInfo.stayLatitude = stay.Latitude;
                            orderInfo.stayLongitude = stay.Longitude;
                            orderInfo.startTime = order.Generates.First().StartTime;
                            orderInfo.endTime = order.Generates.First().EndTime;
                            orderInfo.name = stay.Host.HostUsername;
                            orderInfo.totalCost = order.TotalCost;
                            orderInfo.id =(int) stay.HostId;
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
                            if(order.Report == null) {
                                orderInfo.reportState = 0;
                            }
                            else {
                                if (order.Report.IsDealed == 0) {
                                    orderInfo.reportState = 1;
                                    orderInfo.reportReason = order.Report.Reason;
                                }
                                else {
                                    orderInfo.reportState = 2;
                                    orderInfo.reportReason = order.Report.Reason;
                                    orderInfo.reportReply = order.Report.Reply;
                                }
                            }
                        }
                        message.data.Add("customerOrderList",orderInfos);

                        message.errorCode = 200;
                    }
                }
            }
            return message.ReturnJson();
        }
        [HttpGet("HostOrderInfo")]
        public string GetHostOrder()
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
                            orderInfo.stayLatitude = stay.Latitude;
                            orderInfo.stayLongitude = stay.Longitude;
                            orderInfo.startTime = order.Generates.First().StartTime;
                            orderInfo.endTime = order.Generates.First().EndTime;
                            orderInfo.name = order.Customer.CustomerName;
                            orderInfo.totalCost = order.TotalCost;
                            orderInfo.id = order.Customer.CustomerId;
                            orderInfo.photo = order.Customer.CustomerPhoto;
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
                        message.data.Add("hostOrderList", orderInfos);
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
                                Stay stay = order.Generates.First().Room.Stay;
                                myContext.Entry(stay).State = EntityState.Unchanged;
                                stay.CommentNum += 1;
                                stay.CommentScore +=(int?) customerComment.HouseStars;
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

        [HttpGet("afterPay")]
        public void AfterPay(string order_id, string qr_price, string sign) {
            try {
                myContext.DetachAll();
                var customer = myContext.Orders.Single(b => b.OrderId == int.Parse(order_id))
                    .Customer;
                var host = myContext.Orders.Single(b => b.OrderId == int.Parse(order_id))
                    .Generates.First().Room.Stay.Host;
                customer.CustomerDegree += (int)decimal.Parse(qr_price);
                host.HostScore += (int)decimal.Parse(qr_price);
                var customerGroupList = myContext.CustomerGroups.ToList();
                foreach(var customerGroup in customerGroupList) {
                    if (customer.CustomerDegree >= customerGroup.CustomerLevelDegree)
                        customer.CustomerLevel = customerGroup.CustomerLevel;
                }
                var hostGroupList = myContext.HostGroups.ToList();
                foreach(var hostGroup in hostGroupList) {
                    if (host.HostScore >= hostGroup.HostLevelDegree)
                        host.HostLevel = hostGroup.HostLevel;
                }
                myContext.SaveChanges();
            }
            catch {

            }
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
                            var text = Request.Form["peopleNum"];

                            order.MemberNum = decimal.Parse(Request.Form["peopleNum"]);
                            int stayId = int.Parse(Request.Form["stayId"]);
                            int roomId = int.Parse(Request.Form["roomId"]);
                            DateTime startDate = DateTime.Parse(Request.Form["startDate"]);
                            DateTime endDate = DateTime.Parse(Request.Form["endDate"]);
                            TimeSpan span = endDate.Subtract(startDate);
                            decimal price = myContext.Rooms.Single(s => s.StayId == stayId && s.RoomId == roomId).Price * span.Days;


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

                            
                            //MD5 md5 = MD5.Create();
                            //byte[] strNoKey = md5.ComputeHash(Encoding.UTF8.GetBytes(order.OrderId.ToString() + price.ToString()));
                            //byte[] strWithKey = md5.ComputeHash(Encoding.UTF8.GetBytes(strNoKey.ToString() + key));

                            string strNoKey = GetStrMd5_32X(order.OrderId.ToString() + price.ToString());
                            string strWithKey = GetStrMd5_32X(strNoKey.ToString() + key.ToString());
                            Console.WriteLine(strNoKey);
                            Console.WriteLine(strWithKey);

                            var headers = new WebHeaderCollection();
                            headers["Content-Type"] = "application/json;charset=UTF-8";
                            var dict = new Dictionary<string, string>() {
                                { "order_id", order.OrderId.ToString()},
                                { "order_type", "alipay"},
                                { "order_price", price.ToString()},
                                { "order_name", "归宿房费支付" },
                                { "redirect_url", redirectUrl},
                                { "sign", strWithKey.ToString() }
                            };
                            var value = JsonSerializer.Serialize(dict);
                            Console.WriteLine(value);
                            var resp = PostUrl(postUrl, value);
                            Console.WriteLine(resp);
                            string pattern = @"qr_url"":""(?<type>\S+)"",";
                            string type = Regex.Matches(resp, pattern)[0].Groups["type"].Value;
                            Console.WriteLine(type);
                            message.errorCode = 200;
                            message.data["isSuccess"] = true;
                            message.data["payUrl"] = type;

                            return message.ReturnJson();

                        }
                        catch
                        {
                            message.errorCode = 200;
                            message.data["isSuccess"] = false;
                            message.data["payUrl"] = null;
                            return message.ReturnJson();
                        }

                    }
                }
            }
            return message.ReturnJson();
        }

        public string PostUrl(string url, string postData) {
            string result = "";

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);

            req.Method = "POST";

            req.Timeout = 8000;//设置请求超时时间，单位为毫秒

            req.ContentType = "application/json";

            byte[] data = Encoding.UTF8.GetBytes(postData);

            req.ContentLength = data.Length;

            using (Stream reqStream = req.GetRequestStream()) {
                reqStream.Write(data, 0, data.Length);

                reqStream.Close();
            }

            HttpWebResponse resp = (HttpWebResponse)req.GetResponse();

            Stream stream = resp.GetResponseStream();

            //获取响应内容
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
                result = reader.ReadToEnd();
            }

            return result;
        }

        public string GetStrMd5_32X(string ConvertString)
       //32位小写

       {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();

            string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(ConvertString)));

            t2 = t2.Replace("-", "");

            return t2.ToLower();

        }

        [HttpPost("reportCustomerOrder")]
        public string ReportCustomerOrder() {
            Message message = new Message();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token)) {
                var data = Token.VerifyToken(token);
                if (data != null) {
                    try {
                        int customerId = int.Parse(data["id"]);
                        int orderId = int.Parse(Request.Form["orderId"]);
                        var report = new Report() {
                            OrderId = orderId,
                            ReportTime = DateTime.Now,
                            Reason = Request.Form["reportReason"],
                            IsDealed = 0,
                        };
                        myContext.DetachAll();
                        myContext.Reports.Add(report);
                        myContext.SaveChanges();

                        message.errorCode = 200;
                        return message.ReturnJson();
                    }
                    catch (Exception e){
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
