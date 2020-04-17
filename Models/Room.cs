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
            playersTeam.RemovePlayer(player);
            otherTeam.AddPlayer(player);
        }

        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
            GetTeamOf(player).RemovePlayer(player);
            if(player.IsHost && Players.Count > 0)
            {
                Host = Players[0];
            }
        }

        private void AddPlayerToTeamWithLessMembers(Player player)
        {
            Team teamWithLessMembers = TeamA.Players.Count <= TeamB.Players.Count ? TeamA : TeamB;
            teamWithLessMembers.AddPlayer(player);
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

            // Create WordMap
            ClearBoard();
            Board.InitializeBoard(TeamA.Coder, TeamB.Coder);

            HasGameStarted = true;
        }

        public void ReceiveClue(string clue, int numberOfWords)
        {
            Board.CurrentRound.Clue = clue;
            Board.CurrentRound.CalculateRemainingGuesses(numberOfWords);
        }

        public void CastVote(string word, Player player)
        {
            Board.CastVote(player, word);
        }

        public void RemoveVote(Player player)
        {
            Board.RemoveVote(player);
        }

        internal void ClearBoard()
        {
            Board = new Board();
        }

        public void ResetGame()
        {
            HasGameStarted = false;
            TeamA.SelectNextCoder();
            TeamB.SelectNextCoder();
        }

        public void PassTurn(Player player)
        {
            Board.PassTurn(player);
        }
    }
}
