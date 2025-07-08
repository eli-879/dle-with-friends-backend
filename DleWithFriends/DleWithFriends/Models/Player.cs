namespace DleWithFriends.Models
{
    public class Player
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        public GameState GameState { get; set; } = new GameState();
    }
}
