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
        private readonly ModelContext myContext;
        public BedController(ModelContext modelContext)
        {
            myContext = modelContext;
        }

        public static Bed SearchById(int id)
        {
            try
            {
                ModelContext context = new ModelContext();
                var bed = context.Beds
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
