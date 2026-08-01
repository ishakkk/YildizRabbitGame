namespace YildizRabbitGame;


public enum Direction
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}

public static class DirectionExtensions
{

    public static Direction TurnRight(this Direction current)
        => (Direction)(((int)current + 1) % 4);


    public static Direction TurnLeft(this Direction current)
        => (Direction)(((int)current + 3) % 4);

    public static Direction Opposite(this Direction current)
        => (Direction)(((int)current + 2) % 4);


    public static (int rowDelta, int colDelta) ToStep(this Direction direction) => direction switch
    {
        Direction.North => (-1, 0),
        Direction.South => (1, 0),
        Direction.East => (0, 1),
        Direction.West => (0, -1),
        _ => (0, 0)
    };

    public static string ToTurkishLabel(this Direction direction) => direction switch
    {
        Direction.North => "Kuzey (N)",
        Direction.South => "Güney (S)",
        Direction.East => "Doğu (E)",
        Direction.West => "Batı (W)",
        _ => direction.ToString()
    };
}
