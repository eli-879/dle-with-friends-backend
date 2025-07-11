using DleWithFriends.Factory;
using DleWithFriends.Models;
using System.Security.Cryptography;
using System.Text;

namespace DleWithFriends.GameServer
{
    public class InMemGameRoomManager : IGameRoomManager
    {
        private readonly Dictionary<string, GameRoom> gameRooms = [];

        public Task<string> CreateRoomAsync(Player roomOwner)
        {
            var roomKey = GenerateRoomKey();
            var tries = 3;

            while (tries > 0)
            {
                if (!gameRooms.ContainsKey(roomKey))
                {
                    break;
                }
                else
                {
                    roomKey = GenerateRoomKey();
                    tries--;
                }
            }
            var gameRoom = new GameRoom(roomKey);
            gameRoom.Players = [roomOwner];

            gameRooms[roomKey] = gameRoom;

            return Task.FromResult(roomKey);
        }

        public Task<List<Player>> GetPlayersAsync(string roomId)
        {
            if (gameRooms.TryGetValue(roomId, out var gameRoom))
            {
                return Task.FromResult(gameRoom.Players);
            }
            else
            {
                return Task.FromResult(new List<Player>());
            }
        }

        public Task<bool> AddPlayerToRoom(string roomId, Player newPlayer)
        {
            if (gameRooms.TryGetValue(roomId, out var gameRoom))
            {
                gameRoom.Players.Add(newPlayer);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> IsPlayerAlreadyInRoom(string roomId, string nickname)
        {
            if (gameRooms.TryGetValue(roomId, out var gameRoom))
            {
                return Task.FromResult(gameRoom.Players.Any(p => p.Name.Equals(nickname, StringComparison.OrdinalIgnoreCase)));
            }

            return Task.FromResult(false);
        }

        public Task<List<Player>> ValidateGuess(string roomId, string nickname, string guess)
        {
            if (!gameRooms.TryGetValue(roomId, out var gameRoom))
            {
                return Task.FromResult(new List<Player>());
            }

            var guessFeedback = CheckGuess(gameRoom.TargetWord, guess);

            var player = gameRoom.Players.FirstOrDefault(p => p.Name == nickname);

            if (player == null)
            {
                return Task.FromResult(new List<Player>());
            }

            player.GameState.GuessFeedbacks.Add(guessFeedback);
            player.GameState.Guesses.Add(guess);

            return Task.FromResult(gameRoom.Players);

        }

        private string GenerateRoomKey(int length = 5)
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

        private List<GuessFeedback> CheckGuess(string targetWord, string guess)
        {
            var guessAsList = guess.ToList();
            var targetWordAsList = targetWord.ToList();
            var feedback = new List<GuessFeedback>() 
            { 
                GuessFeedback.Incorrect,
                GuessFeedback.Incorrect,
                GuessFeedback.Incorrect,
                GuessFeedback.Incorrect,
                GuessFeedback.Incorrect
            };

            foreach (var item in guessAsList.Select((value, i) => new { value, i }))
            {
                var letter = item.value;
                var index = item.i;

                if (targetWordAsList[index] == letter)
                {
                    feedback[index] = GuessFeedback.Correct;
                }
            }


            foreach (var item in guessAsList.Select((value, i) => new { value, i})) {
                var letter = item.value;
                var index = item.i;

                if (targetWordAsList.Contains(letter))
                {
                    var locations = targetWordAsList.Select((value, i) => new { value, i }).Where(l => letter == l.value);
                    
                    foreach (var location in locations)
                    {
                        if (feedback[location.i] != GuessFeedback.Correct && feedback[index] != GuessFeedback.Correct)
                        {
                            feedback[index] = GuessFeedback.Misplaced;
                            break;
                        }
                    }
                }
            }

            return feedback;
        }
    }
}
