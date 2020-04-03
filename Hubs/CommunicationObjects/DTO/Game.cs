using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Hubs.CommunicationObjects.DTO
{
    public class Game
    {
        public Player Host { get; set; }
        public string Name { get; set; }
        public Team TeamA { get; set; }
        public Team TeamB { get; set; }
    }
}
