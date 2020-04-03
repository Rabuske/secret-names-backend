using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Models
{
    public class Team
    {
        public ICollection<Player> Players { get; set; }
        public string TeamName { get; set; }

        public Team(string teamName)
        {
            Players = new List<Player>();
            TeamName = teamName;
        }
    }
}
