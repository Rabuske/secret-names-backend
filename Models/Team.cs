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
        public Player Coder { get; set; }

        public Team(string teamName, string teamID)
        {
            Players = new List<Player>();
            Name = teamName;
            Id = teamID;
        }

        public void AddPlayer(Player player)
        {
            Players.Add(player);
            if(Players.Count == 1)
            {
                Coder = player;
            }
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
            if (Coder.Equals(player))
            {
                SelectNextCoder();
            }
        }

        public void SelectNextCoder()
        {
            int indexOfPreviousCoder = Players.FindIndex(player => player.Equals(Coder));
            if (indexOfPreviousCoder == -1)
            {
                if (Players.Count > 0)
                {
                    Coder = Players[0];
                }
                else
                {
                    Coder = null;
                }
                return;
            }
            var indexOfNextCoder = (indexOfPreviousCoder + 1) % Players.Count;
            Coder = Players[indexOfNextCoder];
        }
    }
}
