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

namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StayController : ControllerBase
    {
        [HttpGet("getstay")]
        public string GetStaysByPos()
        {
            GetStaysByPosMessage message = new GetStaysByPosMessage();
            return message.ReturnJson();
        }

        public static Stay SearchById(int id)
        {
            try
            {
                var stay = ModelContext.Instance.Stays
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
            var context = ModelContext.Instance;
            context.DetachAll();
            var message = new GetStayMessage();
            message.data.Add("stayList", new List<StayInfo>());
            try {
                var staySelectList = context.Rooms
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
                    var stay = context.Stays.Single(b => b.StayId == staySelect.stayId);
                    staySelect.stayCharcateristic = stay.Characteristic;
                    staySelect.stayName = stay.StayName;
                    staySelect.stayPhoto = context.RoomPhotos.Where(b => b.StayId == staySelect.stayId).FirstOrDefault().RPhoto;
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
            var context = ModelContext.Instance;
            context.DetachAll();
            var message = new GetStayMessage();
            message.data.Add("stayList", new List<StayInfo>());
            try {
                var staySelectList = context.Stays.
                    OrderByDescending(r => r.CommentScore > 0 ? ((float)r.CommentScore / (float)r.CommentNum) : 0)
                    .Select(g => new StayInfo {
                        stayId = g.StayId
                    })
                    .ToList();

                if (staySelectList.Count > 4)
                    staySelectList.RemoveRange(4, staySelectList.Count - 4);

                foreach (var staySelect in staySelectList) {
                    var stay = context.Stays.Single(b => b.StayId == staySelect.stayId);
                    staySelect.stayPhoto = context.RoomPhotos.Where(b => b.StayId == staySelect.stayId).FirstOrDefault().RPhoto;
                    staySelect.stayPrice = context.Rooms.Where(b => b.StayId == staySelect.stayId).Min(x => x.Price);
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
            var context = ModelContext.Instance;
            context.DetachAll();
            var message = new GetStayMessage();
            message.data.Add("stayList", new List<StayInfo>());
            try {
                var staySelectList = context.Stays.
                    OrderByDescending(r => r.CommentNum)
                    .Select(g => new StayInfo {
                        stayId = g.StayId
                    })
                    .ToList();

                if (staySelectList.Count > 4)
                    staySelectList.RemoveRange(4, staySelectList.Count - 4);

                foreach (var staySelect in staySelectList) {
                    var stay = context.Stays.Single(b => b.StayId == staySelect.stayId);
                    staySelect.stayPhoto = context.RoomPhotos.Where(b => b.StayId == staySelect.stayId).FirstOrDefault().RPhoto;
                    staySelect.stayPrice = context.Rooms.Where(b => b.StayId == staySelect.stayId).Min(x => x.Price);
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
    }
}
