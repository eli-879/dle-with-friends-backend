﻿using DleWithFriends.Models;

namespace DleWithFriends.GameServer
{
    public interface IGameRoomManager
    {
        public Task<string> CreateRoomAsync(Player roomOwner);

        public Task<List<Player>> GetPlayersAsync(string roomId);

        public Task<bool> AddPlayerToRoom(string roomId, Player newPlayer);

        public Task<bool> IsPlayerAlreadyInRoom(string roomId, string nickname);

        public Task<List<Player>> ValidateGuess(string roomId, string nickname, string guess);

    }
}
