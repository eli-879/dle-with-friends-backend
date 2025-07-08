using DleWithFriends.Models;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;

namespace DleWithFriends.GameServer
{
    public class RedisGameRoomManager 
    {
        private readonly IDatabase _db;

        public RedisGameRoomManager(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }


        public async Task<string> CreateRoomAsync(Player roomOwner)
        {
            var roomKey = GenerateRoomKey();
            var tries = 3;

            while (tries > 0)
            {
                if (! await RoomExistsAsync(roomKey))
                {
                    break;
                }
                else
                {
                    roomKey = GenerateRoomKey();
                    tries--;
                }
            }

            await _db.StringSetAsync(roomKey, "active", when: When.NotExists);

            return roomKey;
        }

        public async Task<bool> RoomExistsAsync(string roomId)
        {
            return await _db.KeyExistsAsync(roomId);
        }

        private static string GenerateRoomKey(int length = 5)
        {
            const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder result = new StringBuilder(length);
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] uintBuffer = new byte[sizeof(uint)];

                while (result.Length < length)
                {
                    rng.GetBytes(uintBuffer);
                    uint num = BitConverter.ToUInt32(uintBuffer, 0);
                    result.Append(validChars[(int)(num % (uint)validChars.Length)]);
                }
            }

            return result.ToString();
        }

        public Task<List<Player>> GetPlayersAsync(string roomId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddPlayerToRoom(string roomId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AddPlayerToRoom(string roomId, Player newPlayer)
        {
            throw new NotImplementedException();
        }
    }
}
