using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Models
{
    public class Player
    {
        public string UserName { get; set; }
        public Room Room { get; set; }
        public bool IsHost => Room.Host.Equals (this);
    }
}
