using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Hubs.CommunicationObjects.DTO
{
    public class Room
    {
        public string Name { get; set; }
        public Player Host { get; set; }
        public Team TeamA { get; set; }
        public Team TeamB { get; set; }
        public Board Board { get; set; }
        public Round Round { get; set; }
        public bool HasGameStarted { get; set; }
    }
}
