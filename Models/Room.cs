using SecretNamesBackend.Models.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Models
{
    public class Room
    {
        public string Name { get; set; }        
        public List<Player> Players { get; private set;  }
        public Player Host { get; private set; }
        public Team TeamA { get; set; }
        public Team TeamB { get; set; }
        public Board Board { get; set; }
        public bool HasGameStarted { get; private set; }

        public Room(string name) {
            Name = name;
            Players = new List<Player>();
            TeamA = new Team("Team A", Agent.TEAM_A) ;
            TeamB = new Team("Team B", Agent.TEAM_B);
            Board = new Board();
        }

        public void AddPlayer(Player player)
        {
            if(HasGameStarted)
            {
                throw new GameHasStartedException();
            }

            if (Players.Count == 0)
            {
                Host = player;
            }

            if (!Players.Contains(player))
            {
                Players.Add(player);
            }

            player.Room = this;
            AddPlayerToTeamWithLessMembers(player);
        }

        public void SwitchTeams(Player player)
        {
            Team playersTeam = GetTeamOf(player);
            Team otherTeam = playersTeam.Equals(TeamA) ? TeamB : TeamA;
            playersTeam.Players.Remove(player);
            otherTeam.Players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
            GetTeamOf(player).Players.Remove(player);
            if(player.IsHost && Players.Count > 0)
            {
                Host = Players[0];
            }
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

        public void StartGame()
        {
            if(TeamA.Players.Count < 2 || TeamB.Players.Count < 2)
            {
                throw new TeamNotCompleteException();
            }

            // Get the players that will be the "Know All"
            Player knowAllTeamA = GetNextPlayer(TeamA, Board.KnowAllFromTeamA);
            Player knowAllTeamB = GetNextPlayer(TeamB, Board.KnowAllFromTeamB);

            // Create WordMap
            Board.InitializeBoard(knowAllTeamA, knowAllTeamB);

            HasGameStarted = true;
        }

        public void ReceiveClue(string clue, int numberOfWords)
        {
            Board.CurrentRound.Clue = clue;
            Board.CurrentRound.CalculateRemainingGuesses(numberOfWords);
        }

        private Player GetNextPlayer(Team team, Player knowAllFromTeam)
        {
            if (knowAllFromTeam == null) return team.Players[0];

            // Previous Player has left the tean
            int indexOfPreviousPlayer = team.Players.FindIndex(player => player.UserName.Equals(knowAllFromTeam.UserName));
            if (indexOfPreviousPlayer == -1)
            {
                return team.Players[0];
            }

            var indexOfNexPlayer = (indexOfPreviousPlayer + 1) % team.Players.Count;

            return team.Players[indexOfNexPlayer];            
        }

        public void CastVote(string word, Player player)
        {
            Board.CastVote(player, word);
        }

        public void RemoveVote(Player player)
        {
            Board.RemoveVote(player);
        }

        public void ResetGame()
        {
            Board = new Board(Board.KnowAllFromTeamA, Board.KnowAllFromTeamB);
            HasGameStarted = false;
        }

        public void PassTurn()
        {
            Board.PassTurn();
        }
    }
}
