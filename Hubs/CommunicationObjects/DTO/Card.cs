using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Hubs.CommunicationObjects.DTO
{
    public class Card
    {
        public string Word { get; set; }
        public string Agent { get; set; }
        public bool HasBeenRevealed { get; set; }
    }
}
