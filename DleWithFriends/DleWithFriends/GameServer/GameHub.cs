using DleWithFriends.Factory;
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

    public async Task CreateRoom(string roomOwnerNickname)
    {
        var roomOwner = PlayerFactory.CreatePlayer(roomOwnerNickname);

        var gameRoom = await _gameRoomManager.CreateRoomAsync(roomOwner);
        Console.WriteLine($"New room created! {gameRoom}");
        await Groups.AddToGroupAsync(Context.ConnectionId, gameRoom);
        await Clients.Group(gameRoom).SendAsync("SendNewRoom", gameRoom);
    }

    public async Task JoinRoom(string roomId, string playerNickname)
    {
        if (await _gameRoomManager.IsPlayerAlreadyInRoom(roomId, playerNickname))
        {
            Console.WriteLine($"Player {playerNickname} is already in room {roomId}");
            
        }

        var newPlayer = PlayerFactory.CreatePlayer(playerNickname);
        await _gameRoomManager.AddPlayerToRoom(roomId, newPlayer);
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("UserJoined", Context.ConnectionId);
    }

    public async Task StartGame(string roomId)
    {
        await Clients.Group(roomId).SendAsync("GameStarted", Context.ConnectionId);
    }

    public async Task GetGameState(string roomId)
    {
        Console.WriteLine($"Requesting game state for room {roomId}");
        var players = await _gameRoomManager.GetPlayersAsync(roomId);
        await Clients.Group(roomId).SendAsync("ReceiveGameState", players);
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
        await Clients.Group(roomId).SendAsync("UserLeft", Context.ConnectionId);
    }

    public async Task GetPlayers(string roomId)
    {
        Console.WriteLine($"Requesting players for room {roomId}");
        var players = await _gameRoomManager.GetPlayersAsync(roomId);
        await Clients.Group(roomId).SendAsync("ReceivePlayers", players);
    }


    public async Task SendChatMessage(string roomId, string user, string message)
    {
        Console.WriteLine($"Received message from {user} - {message}");
        await Clients.Group(roomId).SendAsync("ReceiveChatMessage", user, message);
    }

}
