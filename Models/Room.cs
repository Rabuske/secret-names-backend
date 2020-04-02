using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Models
{
    public class Room
    {
        private ICollection<Player> _players;
        private Player _host;

        public string Name { get; set; }        
        public ICollection<Player> Players { get => this._players.ToList().AsReadOnly();  }       
        public Player Host { get; }

        public Room(string name) {
            this.Name = name;
            this._players = new List<Player>();
        }

        public void AddPlayer(Player player)
        {
            if (this._players.Count == 0)
            {
                this._host = player;
            }

            if (!this._players.Contains(player))
            {
                this._players.Add(player);
            }

            player.Room = this;
        }

        public void RemovePlayer(Player player)
        {
            this._players.Remove(player);
        }


    }
}
