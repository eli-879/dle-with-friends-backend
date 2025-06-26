using Microsoft.AspNetCore.SignalR;

namespace DleWithFriends.GameServer;

public class GameHub : Hub
{
    private readonly IGameRoomManager _gameRoomManager; 

    public GameHub(IGameRoomManager gameRoomManager)
    {
        _gameRoomManager = gameRoomManager;
    }
    public async Task SendGuess(string roomId)
    {
        await Clients.Group(roomId).SendAsync("ReceiveGuess");
    }

    public async Task CreateRoom()
    {
        var gameRoom = await _gameRoomManager.CreateRoomAsync();
        Console.WriteLine($"New room created! {gameRoom}");
        await Groups.AddToGroupAsync(Context.ConnectionId, gameRoom);
        await Clients.Group(gameRoom).SendAsync("SendNewRoom", gameRoom);
    }

    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("UserJoined", Context.ConnectionId);
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("UserLeft", Context.ConnectionId);
    }

    public async Task SendChatMessage(string roomId, string user, string message)
    {
        Console.WriteLine($"Received message from {user} - {message}");
        await Clients.Group(roomId).SendAsync("ReceiveChatMessage", user, message);
    }

}
