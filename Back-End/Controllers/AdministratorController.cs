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


    }
}
