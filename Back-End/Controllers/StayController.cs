using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Back_End.Contexts;
using System.Text.Json;
using Back_End.Models;

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
    }
}
