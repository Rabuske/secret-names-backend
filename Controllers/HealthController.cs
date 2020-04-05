using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SecretNamesBackend.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Controllers
{
    [ApiController]
    public class HealthController : ControllerBase
    {
        public HealthController()
        {

        }

        [HttpGet("health")]
        public ActionResult<string> HealthCheck()
        {
            return "Up and Running";
        }
    }
}