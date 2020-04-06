using SecretNamesBackend.Hubs.CommunicationObjects.DTO;
using SecretNamesBackend.Models;
using System;
using System.Linq;

namespace SecretNamesBackend.Hubs.CommunicationObjects
{
    public class Adapter
    {
        public static DTO.Room Convert(Models.Room room)
        {
            return new DTO.Room
            {
                Name = room.Name,
                Host = ConvertPlayer(room.Host),
                TeamA = ConvertTeam(room.TeamA),
                TeamB = ConvertTeam(room.TeamB),
                Board = ConvertBoard(room.Board),
                HasGameStarted = room.HasGameStarted,
                Round = ConvertRound(room.Board.CurrentRound)
            };
        }

        private static DTO.Round ConvertRound(Models.Round currentRound)
        {
            if(currentRound == null)
            {
                return new DTO.Round();
            }
            return new DTO.Round
            {
                Clue = currentRound.Clue,
                RemainingGuesses = currentRound.RemainingGuesses,
                Team = currentRound.Team.Id,
                Votes = currentRound.Votes.Select(vote => new DTO.Vote
                {
                    UserName = vote.Key.UserName,
                    Word = vote.Value.Word
                }).ToList(),
                IsClueSubmitted = currentRound.IsClueSubmitted,
                NumberOfWordsRelatedToGuess = currentRound.NumberOfWordsRelatedToGuess
            };
        }

        private static DTO.Player ConvertPlayer(Models.Player player)
        {            
            if(player == null)
            {
                return new DTO.Player { UserName = "", IsHost = false };
            }
            return new DTO.Player
            {
                UserName = player.UserName,
                IsHost = player.Equals(player.Room.Host)
            };
        }

        private static DTO.Team ConvertTeam(Models.Team team)
        {
            return new DTO.Team
            {
                Name = team.Name,
                Players = team.Players.Select(player => ConvertPlayer(player)).ToList(),
                Id = team.Id
            };
        }

        private static DTO.Board ConvertBoard(Models.Board board)
        {
            return new DTO.Board
            {
                Cards = board.Cards.Select(card => new DTO.Card
                {
                    Agent = card.Agent,
                    Word = card.Word,
                    HasBeenRevealed = card.HasBeenRevealed
                }).ToList(),
                KnowAllForTeamA = ConvertPlayer(board.KnowAllFromTeamA),
                KnowAllForTeamB = ConvertPlayer(board.KnowAllFromTeamB)
            };
        }
    }
}
