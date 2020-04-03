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
        static readonly Dictionary<string, Room> Rooms = new Dictionary<string, Room>();
        static readonly Dictionary<string, Player> Players = new Dictionary<string, Player>();
        static readonly Dictionary<string, string> ConnectionToUser = new Dictionary<string, string>();

        public async Task RegisterNewUser(string userName, string roomName)
        {               
            // Make Sure that the UserName is Unique 
            var validatedUserName = userName;

            while (Players.ContainsKey(validatedUserName))
            {
                validatedUserName += new Random().Next(0, 9);
            }

            // Get or create the room
            Room gameRoom;
            if (!Rooms.ContainsKey(roomName))
            {
                gameRoom = new Room(roomName);
                Rooms.Add(roomName, gameRoom);
            } else
            {
                gameRoom = Rooms[roomName];
            }

            // Update model
            Player player = new Player { UserName = validatedUserName, Room = gameRoom };
            gameRoom.AddPlayer(player);

            // Update local mappings
            Players.Add(validatedUserName, player);
            ConnectionToUser.Add(Context.ConnectionId, validatedUserName);

            // Update global groups
            await Groups.AddToGroupAsync(Context.ConnectionId, roomName);

            // Notify itself
            await Clients.Caller.SendAsync(WebSocketActions.CONNECTION_ACCEPTED, validatedUserName);

            // Update game
            await Clients.Group(roomName).SendAsync(WebSocketActions.UPDATE_GAME, Adapter.Convert(gameRoom));
        }

        public async override Task OnDisconnectedAsync(Exception exception)
        {
            if (!ConnectionToUser.ContainsKey(Context.ConnectionId) || !Players.ContainsKey(ConnectionToUser[Context.ConnectionId]))
            {
                return;
            }

            Player player = Players[ConnectionToUser[Context.ConnectionId]];

            // Update model
            player.Room.RemovePlayer(player);

            // Update Local Mappings
            if (player.Room.Players.Count == 0)
            {
                Rooms.Remove(player.Room.Name); 
            }

            Players.Remove(player.UserName);
            ConnectionToUser.Remove(Context.ConnectionId);

            // Update Global Groups
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, player.Room.Name);

            // Notify others clients
            await Clients.OthersInGroup(player.Room.Name).SendAsync(WebSocketActions.UPDATE_GAME, Adapter.Convert(player.Room));
        }

        public async Task SendChatMessage(string message)
        {
            Player player = Players[ConnectionToUser[Context.ConnectionId]];
            await Clients.Group(player.Room.Name).SendAsync(WebSocketActions.CHAT_MESSAGE_SENT, message, player.UserName);
        }

    }

    public struct WebSocketActions
    {
        public static readonly string USER_JOINED = "userJoined";
        public static readonly string USER_LEFT = "userLeft";
        public static readonly string UPDATE_GAME = "updateGame";
        public static readonly string CHAT_MESSAGE_SENT = "chatMessageSent";
        public static readonly string CONNECTION_ACCEPTED = "connectionAccepted";
    }
}
