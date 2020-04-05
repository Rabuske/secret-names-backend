using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Hubs.CommunicationObjects.DTO
{
    public class Board
    {
        public Player KnowAllForTeamA { get; set; }
        public Player KnowAllForTeamB { get; set; }
        public List<Card> Cards { get; set; }
    }
}
