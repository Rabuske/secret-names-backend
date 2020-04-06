using SecretNamesBackend.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Models
{
    public class Board
    {
        public List<Card> Cards { get; set; }
        public Player KnowAllFromTeamA { get; set; }
        public Player KnowAllFromTeamB { get; set; }
        public Round CurrentRound { get; set; }
        public bool HasGameFinished { get; set; }
        public Team WinningTeam { get; set; }

        public Board()
        {
            Cards = new List<Card>();
        }

        public Board(Player knowAllFromTeamA, Player knowAllFromTeamB)
        {
            KnowAllFromTeamA = knowAllFromTeamA;
            KnowAllFromTeamB = knowAllFromTeamB;
            Cards = new List<Card>();
        }

        public void InitializeBoard(Player playerFromTeamA, Player playerFromTeamB)
        {
            KnowAllFromTeamA = playerFromTeamA;
            KnowAllFromTeamB = playerFromTeamB;

            // Decides which team starts randomly
            Random random = new Random();
            var startingTeam = random.Next(0, 2) == 0 ? playerFromTeamA.Room.TeamA : playerFromTeamB.Room.TeamB;

            // Get the cards for this Board
            var poolOfAgents = GetPoolOfAgents(startingTeam.Id);
            Cards = GetCardsMappedToAgents(poolOfAgents, random);

            // Initialize first Round
            CurrentRound = new Round(startingTeam);
        }

        private List<Card> GetCardsMappedToAgents(List<string> poolOfAgents, Random random)
        {
            WordProvider wordProvider = WordProvider.GetInstance();

            Cards = new List<Card>();
            for (int i = 0; i < 25; i++)
            {
                var randomAgentIndex = random.Next(0, poolOfAgents.Count);
                Cards.Add(new Card
                {
                    Word = wordProvider.GetRandomWord(),
                    HasBeenRevealed = false,
                    Agent = poolOfAgents[randomAgentIndex]
                });

                poolOfAgents.RemoveAt(randomAgentIndex);
            }

            return Cards;
        }

        private List<string> GetPoolOfAgents(string startingTeam)
        {
            List<string> poolOfAgents = new List<string>();
            // Fill the pool of card owners
            for (int i = 0; i < 8; i++)
            {
                poolOfAgents.Add(Agent.TEAM_A);
                poolOfAgents.Add(Agent.TEAM_B);
                if (i < 7)
                {
                    poolOfAgents.Add(Agent.CIVILIAN);
                }
            }

            poolOfAgents.Add(Agent.ASSASSIN);
            poolOfAgents.Add(startingTeam);
            return poolOfAgents;
        }

        public void CastVote(Player player, string word)
        {
            var card = Cards.Find(card => card.Word.Equals(word));
            CurrentRound.AddVote(player, card);

            if (!CurrentRound.HasFinished)
            {
                return;
            }

            // Initiate New Round
            Team nextTeam = CurrentRound.Team.Equals(player.Room.TeamA) ? player.Room.TeamB : player.Room.TeamA;
            CurrentRound = new Round(nextTeam);

            // Check if game has finished
            // Assassing has been revealed: currentTeam loses
            if(Cards.Exists(card => card.Agent.Equals(Agent.ASSASSIN) && card.HasBeenRevealed))
            {
                HasGameFinished = true;
                WinningTeam = nextTeam;
            }

            // Check if there are some team whose all the cards have been revealed
            if(!Cards.Exists(card => card.Agent.Equals(Agent.TEAM_A) && !card.HasBeenRevealed))
            {
                HasGameFinished = true;
                WinningTeam = player.Room.TeamA;
            }

            if (!Cards.Exists(card => card.Agent.Equals(Agent.TEAM_B) && !card.HasBeenRevealed))
            {
                HasGameFinished = true;
                WinningTeam = player.Room.TeamB;
            }

        }

        internal void RemoveVote(Player player)
        {
            CurrentRound.Votes.Remove(player);
        }

        internal void PassTurn()
        {
            CurrentRound.FinishTurn();
        }
    }
}
