namespace DleWithFriends.Models
{
    public class GameRoom(string roomId)
    {
        public string RoomId { get; set; } = roomId;
        public List<Player> Players { get; set; } = [];
        public string TargetWord { get; set; } = string.Empty;
    }
}
