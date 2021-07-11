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
    public class ReportController : ControllerBase
    {
        public static Report SearchById(int id)
        {
            try
            {
                var report = ModelContext.Instance.Reports
                    .Single(b => b.OrderId == id);
                return report;
            }
            catch
            {
                return null;
            }

        }

    }
}
