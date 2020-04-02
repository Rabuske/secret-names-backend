using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SecretNamesBackend.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Controllers
{
    public class GameController : ControllerBase
    {
        private readonly IHubContext<GameHub> gametHub;

        public GameController(IHubContext<GameHub> gametHub)
        {
            this.gametHub = gametHub;
        }
    }
}
