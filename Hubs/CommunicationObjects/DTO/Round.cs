using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Hubs.CommunicationObjects.DTO
{
    public class Round
    {
        public List<Vote> Votes { get; set; }
        public string Team { get; set; }
        public string Clue { get; set; }
        public int RemainingGuesses { get; set; }
        public bool IsClueSubmitted { get; set; }
        public int NumberOfWordsRelatedToGuess { get; set; }

        public Round()
        {
            Votes = new List<Vote>();
            Team = "";
            Clue = "";
        }
    }
}
