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

namespace Back_End.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdministratorController : ControllerBase
    {
        class PagedExamines
        {
            public int stayId { get; set; }
            public int hostId { get; set; }
            public string stayCity { get; set; }
        }
        public readonly int pageSize = 10;

        public static Administrator SearchById(int id)
        {
            try
            {
                var admin = ModelContext.Instance.Administrators
                    .Single(b => b.AdminId == id);
                return admin;
            }
            catch
            {
                return null;
            }

        }

        [HttpGet]
        public string GetAdmin()
        {
            //UNDONE:等待修改api
            return null;
        }

        [HttpGet("examineStay")]
        public string GetExamineByPage()
        {
            GetExamineByPageMessage message = new GetExamineByPageMessage();
            StringValues token = default(StringValues);
            if (Request.Headers.TryGetValue("token", out token))
            {
                message.errorCode = 300;
                var data = Token.VerifyToken(token);
                if(data!=null)
                {
                    int id = int.Parse(data["id"]);
                    var admin = SearchById(id);
                    if(admin!=null)
                    {
                        message.errorCode = 200;
                        int page = int.Parse(Request.Query["pagenum"]);
                        var pageInfo = ModelContext.Instance.Stays.Where(s => s.StayStatus == 0).OrderBy(b => b.StayId).Skip((page - 1) * pageSize)
                            .Take(pageSize).Select(c => new PagedExamines{ stayId = c.StayId, hostId = (int)c.HostId, stayCity = c.Area.AreaName });
                        var examines = pageInfo.ToList();
                        message.data["examineStayList"] = examines;
                    }
                }
            }
            return message.ReturnJson();
        }

    }
}
