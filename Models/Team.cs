using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Models
{
    public class Team
    {
        public List<Player> Players { get; set; }
        public string Name { get; set; }
        public string Id { get; set; }

        public Team(string teamName, string teamID)
        {
            Players = new List<Player>();
            Name = teamName;
            Id = teamID;
        }
    }
}
