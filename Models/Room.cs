using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Models
{
    public class Room
    {
        private ICollection<Player> _players;

        public string Name { get; set; }        
        public ICollection<Player> Players { get => _players.ToList().AsReadOnly();  }
        public Player Host { get; private set; }
        public Team TeamA { get; set; }
        public Team TeamB { get; set; }

        public Room(string name) {
            this.Name = name;
            this._players = new List<Player>();
            this.TeamA = new Team("Team A");
            this.TeamB = new Team("Team B");
        }

        public void AddPlayer(Player player)
        {
            if (this._players.Count == 0)
            {
                Host = player;
            }

            if (!_players.Contains(player))
            {
                _players.Add(player);
            }

            player.Room = this;
            AddPlayerToTeamWithLessMembers(player);
        }

        public void RemovePlayer(Player player)
        {
            _players.Remove(player);
            GetTeamOf(player).Players.Remove(player);
        }

        private void AddPlayerToTeamWithLessMembers(Player player)
        {
            Team teamWithLessMembers = TeamA.Players.Count <= TeamB.Players.Count ? TeamA : TeamB;
            teamWithLessMembers.Players.Add(player);
        }

        public Team GetTeamOf(Player player)
        {
            if (TeamA.Players.Contains(player))
            {
                return TeamA;
            }
            return TeamB;
        }
    }
}
