namespace Server.Domain.Entities;

public class Car
{
    private static readonly Random Random = new();
    public required string Id { get; set; }
    public float X { get; set; } = Random.Next(25, 400);
    public float Y { get; set; } = Random.Next(25, 400);
    public float Speed { get; set; } = 0;
    public float Rotation { get; set; } = 0;
    public float SpeedX { get; set; } = 0;
    public float SpeedY { get; set; } = 0;
    public float Angle { get; set; } 
    public float Velocity { get; set; } 
    public float MaxVelocity { get; set; } = 5f; 
    public float Acceleration { get; set; } = 0.1f; 
    public float RotationSpeed { get; set; } = 0.05f; 

    public int Color { get; set; } = Random.Next(1, 7);
    public float Hitbox { get; set; } = 35f;
}
