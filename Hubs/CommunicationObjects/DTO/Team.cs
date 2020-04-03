using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Hubs.CommunicationObjects.DTO
{
    public class Team
    {
        public List<Player> Players { get; set; }
        public string Name { get; set; }

    }
}
