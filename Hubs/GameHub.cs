using Microsoft.AspNetCore.SignalR;
using SecretNamesBackend.Hubs.CommunicationObjects;
using SecretNamesBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecretNamesBackend.Hubs
{
    public class GameHub : Hub
    {
        static readonly Dictionary<string, Room> RoomNameToRoomMapping = new Dictionary<string, Room>();
        static readonly Dictionary<string, Player> PlayerNameToPlayerMapping = new Dictionary<string, Player>();
        static readonly Dictionary<string, string> ConnectionToPlayerMapping = new Dictionary<string, string>();

        public async Task RegisterNewUser(string userName, string roomName)
        {
            // Make Sure that the UserName is Unique 
            var validatedUserName = userName;

            while (PlayerNameToPlayerMapping.ContainsKey(validatedUserName))
            {
                validatedUserName += new Random().Next(0, 9);
            }

            // Get or create the room
            Room gameRoom;
            if (!RoomNameToRoomMapping.ContainsKey(roomName))
            {
                gameRoom = new Room(roomName);
                RoomNameToRoomMapping.Add(roomName, gameRoom);
            } else
            {
                gameRoom = RoomNameToRoomMapping[roomName];
            }

            // Update model
            Player player = new Player { UserName = validatedUserName, Room = gameRoom };

            try
            {
                gameRoom.AddPlayer(player);
            }
            catch (Exception e)
            {
                await Clients.Caller.SendAsync(WebSocketActions.DISPLAY_MESSAGE, MessageTypes.ERROR, e.Message);
                Context.Abort();
            }

            // Update local mappings
            PlayerNameToPlayerMapping.Add(validatedUserName, player);
            ConnectionToPlayerMapping.Add(Context.ConnectionId, validatedUserName);

            // Update global groups
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

            // Notify itself
            await Clients.Caller.SendAsync(WebSocketActions.CONNECTION_ACCEPTED, validatedUserName, player.Equals(gameRoom.Host));

            // Update game
            await Clients.Group(roomName).SendAsync(WebSocketActions.UPDATE_GAME, Adapter.Convert(gameRoom));
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            if (!ConnectionToPlayerMapping.ContainsKey(Context.ConnectionId) || !PlayerNameToPlayerMapping.ContainsKey(ConnectionToPlayerMapping[Context.ConnectionId]))
            {
                return;
            }

            Player player = PlayerNameToPlayerMapping[ConnectionToPlayerMapping[Context.ConnectionId]];

            // Check if game must be finshed due to lack of players
            bool gameHasToBeFinished = player.Room.HasGameStarted && player.Room.GetTeamOf(player).Players.Count == 2;

            // Update model
            player.Room.RemovePlayer(player);
            
            // Update Local Mappings
            if (player.Room.Players.Count == 0)
            {
                RoomNameToRoomMapping.Remove(player.Room.Name);
            }            

            PlayerNameToPlayerMapping.Remove(player.UserName);
            ConnectionToPlayerMapping.Remove(Context.ConnectionId);

            // Update Global Groups
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, player.Room.Name);

            // Finish Game
            if (gameHasToBeFinished)
            {
                player.Room.ResetGame();
                await Clients.OthersInGroup(player.Room.Name).SendAsync(WebSocketActions.DISPLAY_MESSAGE, MessageTypes.WARNING, "Game Finished due to insufficient number of players");
            }

            // Notify others clients
            await Clients.OthersInGroup(player.Room.Name).SendAsync(WebSocketActions.UPDATE_GAME, Adapter.Convert(player.Room));
        }

        public async Task SendChatMessage(string message)
        {
            Player player = PlayerNameToPlayerMapping[ConnectionToPlayerMapping[Context.ConnectionId]];
            await Clients.Group(player.Room.Name).SendAsync(WebSocketActions.CHAT_MESSAGE_SENT, message, player.UserName);
        }

        public async Task SwitchTeamMember(string playerName)
        {
            // Check if player has authorization to change team
            Player caller = PlayerNameToPlayerMapping[ConnectionToPlayerMapping[Context.ConnectionId]];
            if (!caller.IsHost || caller.Room.HasGameStarted) return;

            Player player = PlayerNameToPlayerMapping[playerName];
            player.Room.SwitchTeams(player);

            // Notify clients
            await Clients.Group(player.Room.Name).SendAsync(WebSocketActions.UPDATE_GAME, Adapter.Convert(player.Room));
        }

        public async Task StartGame()
        {
            Player caller = PlayerNameToPlayerMapping[ConnectionToPlayerMapping[Context.ConnectionId]];

            // Validations
            if (!caller.IsHost)
            {
                await Clients.Caller.SendAsync(WebSocketActions.DISPLAY_MESSAGE, MessageTypes.ERROR, "You are not the host!");
                return;
            }

            try
            {
                caller.Room.StartGame();
            }
            catch (Exception e)
            {
                await Clients.Caller.SendAsync(WebSocketActions.DISPLAY_MESSAGE, MessageTypes.ERROR, e.Message);
                return;
            }

            // Notify clients
            await Clients.Group(caller.Room.Name).SendAsync(WebSocketActions.UPDATE_GAME, Adapter.Convert(caller.Room));
        }

        public async Task ReceiveClue(string clue, string numberOfWords)
        {
            Player player = PlayerNameToPlayerMapping[ConnectionToPlayerMapping[Context.ConnectionId]];

            // Validate that the player is the known all
            if (!player.Room.HasGameStarted)
            {
                await Clients.Caller.SendAsync(WebSocketActions.DISPLAY_MESSAGE, MessageTypes.ERROR, "Game has not started yet!");
            }

            player.Room.ReceiveClue(clue, int.Parse(numberOfWords));

            // Notify clients
            await Clients.Group(player.Room.Name).SendAsync(WebSocketActions.UPDATE_GAME, Adapter.Convert(player.Room));
        }

        public async Task VoteForWord(string word) { 
            Player player = PlayerNameToPlayerMapping[ConnectionToPlayerMapping[Context.ConnectionId]];
            player.Room.CastVote(word, player);

            await Clients.Group(player.Room.Name).SendAsync(WebSocketActions.CHAT_MESSAGE_SENT, $"{player.UserName} voted in word {word}", "Secret Names");

            // Check if game has finished for this board
            if (player.Room.Board.HasGameFinished)
            {
                await SendGameFinishedMessageAsync(player.Room.Name, player.Room.Board.WinningTeam);
                player.Room.ResetGame();
            }

            // Notify clients
            await Clients.Group(player.Room.Name).SendAsync(WebSocketActions.UPDATE_GAME, Adapter.Convert(player.Room));
        }

        private async Task SendGameFinishedMessageAsync(string roomName, Team winningTeam)
        {
            await Clients.Group(roomName).SendAsync(WebSocketActions.CHAT_MESSAGE_SENT, $"Game has end. {winningTeam.Name} has won!", "Secret Names");
        }

        public async Task RemoveVote()
        {
            Player player = PlayerNameToPlayerMapping[ConnectionToPlayerMapping[Context.ConnectionId]];
            player.Room.RemoveVote(player);

            await Clients.Group(player.Room.Name).SendAsync(WebSocketActions.CHAT_MESSAGE_SENT, $"{player.UserName} removed their vote", "Secret Names");

            // Notify clients
            await Clients.Group(player.Room.Name).SendAsync(WebSocketActions.UPDATE_GAME, Adapter.Convert(player.Room));
        }
    }

    public struct MessageTypes
    {
        public static readonly string ERROR = "ERROR";
        public static readonly string WARNING = "WARNING";
        public static readonly string SUCCESS = "SUCCESS";
        public static readonly string INFO = "INFO";
    }

    public struct WebSocketActions
    {
        public static readonly string UPDATE_GAME = "updateGame";
        public static readonly string CHAT_MESSAGE_SENT = "chatMessageSent";
        public static readonly string CONNECTION_ACCEPTED = "connectionAccepted";
        public static readonly string DISPLAY_MESSAGE = "displayMessage";
        public static readonly string GAME_HAS_FINISHED = "gameHasFinished";
    }
}
