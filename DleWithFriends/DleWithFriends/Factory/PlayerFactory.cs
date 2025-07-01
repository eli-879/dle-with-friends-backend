using DleWithFriends.Models;

namespace DleWithFriends.Factory
{
    public static class PlayerFactory
    {
        public static Player CreatePlayer(string name)
        {
            return new Player
            {
                Id = Guid.NewGuid(),
                Name = name
            };
        }
    }
}
