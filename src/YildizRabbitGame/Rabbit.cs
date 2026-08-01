namespace YildizRabbitGame;


public class Rabbit
{
    public Cell Position { get; set; }
    public Direction Facing { get; set; }
    public bool IsAlive { get; private set; } = true;
    public bool HasArrivedHome { get; private set; } = false;

    public Rabbit(Cell start, Direction facing)
    {
        Position = start;
        Facing = facing;
    }

    public void Kill() => IsAlive = false;

    public void MarkArrivedHome() => HasArrivedHome = true;
}
