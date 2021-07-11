using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.Json;
using System.Text.RegularExpressions;
using Back_End.Contexts;
using Back_End.Models;
using Microsoft.AspNetCore.Http;

namespace Back_End.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BedController : ControllerBase
    {
        public static readonly string[] BedType = { "1米宽单人床", "1.2米宽双人床", "1.4米宽双人床", "1.5米宽双人床", "1.8米宽双人床"
            ,"双层床","婴儿床","吊床","水床","沙发床","地板床垫"};

        public static Bed SearchById(int id)
        {
            try
            {
                var bed = ModelContext.Instance.Beds
                    .Single(b => b.BedId == id);
                return bed;
            }
            catch
            {
                return null;
            }

        }

    }
}
