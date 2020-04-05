using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Hubs.CommunicationObjects.DTO
{
    public class Player
    {
        public string UserName { get; set; }
        public bool IsHost { get; set; }
    }
}
