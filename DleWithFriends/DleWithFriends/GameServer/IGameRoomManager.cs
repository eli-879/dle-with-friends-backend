namespace DleWithFriends.GameServer
{
    public interface IGameRoomManager
    {
        public Task<string> CreateRoomAsync();

    }
}
