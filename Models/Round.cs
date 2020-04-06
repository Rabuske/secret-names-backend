using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Models
{
    public class Round
    {
        public string Clue { get; set; }
        public int RemainingGuesses { get; private set; }
        public Team Team { get; set; }
        public IDictionary<Player, Card> Votes { get; set; }
        public bool HasFinished { get; internal set; }
        public bool IsClueSubmitted { get => Clue != null && Clue.Length > 0;}
        public int NumberOfWordsRelatedToGuess { get; set; }

        public Round(Team team)
        {
            Team = team;
            Votes = new Dictionary<Player, Card>();
        }

        public void CalculateRemainingGuesses(int numberOfWordsRelatedToClue)
        {
            NumberOfWordsRelatedToGuess = numberOfWordsRelatedToClue;
            RemainingGuesses = 25;
            if (numberOfWordsRelatedToClue > 24 || numberOfWordsRelatedToClue <= 0)
            {
                return;
            }
            RemainingGuesses = numberOfWordsRelatedToClue;
        }

        public void AddVote(Player player, Card card)
        {
            if (Votes.ContainsKey(player))
            {
                Votes[player] = card;
            }
            else
            {
                Votes.Add(player, card);
            }

            // Vote Phase is finished when everyone casted their votes...
            if(Votes.Count != Team.Players.Count - 1)
            {
                return;
            }

            // And all the voted cards are the same...
            if (!Votes.All(voteCard => voteCard.Value.Word.Equals(card.Word)))
            {
                return;
            }

            // Reveal card
            card.HasBeenRevealed = true;
            RemainingGuesses--;

            // Round is finished when there are no remaining guesses...
            if(RemainingGuesses <= 0)
            {
                HasFinished = true;
            }

            // Or the card is not from the current team
            if (!card.Agent.Equals(Team.Id))
            {
                HasFinished = true;
            }
        }

        public void FinishTurn()
        {
            HasFinished = true;
        }
    }
}
