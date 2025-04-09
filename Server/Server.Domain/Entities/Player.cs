namespace Server.Domain.Entities;

public class Player
{
    public required string Id { get; set; }
    public required string Username { get; set; }
    public int Rating { get; set; } = 0;
    public required Car Car { get; set; }
}
