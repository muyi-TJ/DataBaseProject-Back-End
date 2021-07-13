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

namespace Back_End.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class HostGroupController {
        private readonly ModelContext myContext;
        public HostGroupController(ModelContext modelContext) {
            myContext = modelContext;
        }

        [HttpGet]
        public string GetHostGroup() {
            Message message = new Message();
            try {
                message.data.Add("customerGroup", new List<Dictionary<string, dynamic>>());
                var hostGroupList = myContext.HostGroups.ToList();
                for(int i = 0; i < hostGroupList.Count; i++) { 
                    message.data["customerGroup"].Add(
                        new Dictionary<string, dynamic> {
                            {"hostLevel", hostGroupList[i].HostLevel },
                            {"hostLevelName", hostGroupList[i].HostLevelName},
                            {"hostNextLevelDegree", i == hostGroupList.Count - 1 ? 999 : hostGroupList[i+1].HostLevel}
                        }
                    );
                    message.errorCode = 200;
                    return message.ReturnJson();
                }
            }
            catch {
                message.errorCode = 300;
                return message.ReturnJson();
            }
            return message.ReturnJson();
        }
    }
}
