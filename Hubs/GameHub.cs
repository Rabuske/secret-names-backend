using Microsoft.AspNetCore.SignalR;
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

        public async Task RegisterNewUser(string userName, string room)
        {               
            // Make Sure that the UserName is Unique 
            var validatedUserName = userName;

            while (Players.ContainsKey(validatedUserName))
            {
                validatedUserName += new Random().Next(0, 9);
            }

            // Get or create the room
            Room gameRoom;
            if (!Rooms.ContainsKey(room))
            {
                gameRoom = new Room(room);
                Rooms.Add(room, gameRoom);
            } else
            {
                gameRoom = Rooms[room];
            }

            // Update model
            Player player = new Player { UserName = validatedUserName, Room = gameRoom };
            gameRoom.AddPlayer(player);

            // Update local mappings
            Players.Add(validatedUserName, player);
            ConnectionToUser.Add(Context.ConnectionId, validatedUserName);

            // Update global groups
            await Groups.AddToGroupAsync(Context.ConnectionId, room);

            // Notify others clients
            await Clients.OthersInGroup(room).SendAsync(WebSocketActions.USER_JOINED, validatedUserName);

            // Notify itself
            if (gameRoom.Host == player)
            {
                await Clients.Caller.SendAsync(WebSocketActions.CONNECTION_ACCEPTED_HOST, validatedUserName);
            }
            else
            {
                await Clients.Caller.SendAsync(WebSocketActions.CONNECTION_ACCEPTED_GUEST, validatedUserName);
            }
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
            await Clients.OthersInGroup(player.Room.Name).SendAsync(WebSocketActions.USER_LEFT, player.UserName);
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
        public static readonly string CHAT_MESSAGE_SENT = "chatMessageSent";
        public static readonly string CONNECTION_ACCEPTED_GUEST = "connectionAcceptedGuest";
        public static readonly string CONNECTION_ACCEPTED_HOST = "connectionAcceptedHost";
    }
}
