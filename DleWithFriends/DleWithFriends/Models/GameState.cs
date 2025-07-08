namespace DleWithFriends.Models;

public class GameState
{
    public string RoomId { get; set; }
    
    public List<string> Guesses { get; set; } = new List<string>();

    public List<List<GuessFeedback>> GuessFeedbacks { get; set; } = new List<List<GuessFeedback>>();
}
