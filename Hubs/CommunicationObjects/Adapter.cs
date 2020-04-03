using SecretNamesBackend.Hubs.CommunicationObjects.DTO;
using SecretNamesBackend.Models;
using System.Linq;

namespace SecretNamesBackend.Hubs.CommunicationObjects
{
    public class Adapter
    {
        public static Game Convert(Room room)
        {
            return new Game
            {
                Name = room.Name,
                Host = ConvertPlayer(room.Host),
                TeamA = ConvertTeam(room.TeamA),
                TeamB = ConvertTeam(room.TeamB)
            };
        }

        private static DTO.Player ConvertPlayer(Models.Player player)
        {
            return new DTO.Player
            {
                UserName = player.UserName
            };
        }

        private static DTO.Team ConvertTeam(Models.Team team)
        {
            return new DTO.Team
            {
                Name = team.TeamName,
                Players = team.Players.Select(player => ConvertPlayer(player)).ToList()
            };
        }
    }
}
