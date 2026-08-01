namespace YildizRabbitGame;

public enum ObstacleType
{

    None,


    Wolf,


    Fox,


    Wire,


    Fence
}

public static class ObstacleTypeExtensions
{
    public static bool IsDeadly(this ObstacleType type)
        => type is ObstacleType.Wolf or ObstacleType.Fox;

    public static string ToDisplaySymbol(this ObstacleType type) => type switch
    {
        ObstacleType.None => ".",
        ObstacleType.Wolf => "K",
        ObstacleType.Fox => "T",
        ObstacleType.Wire => "#",
        ObstacleType.Fence => "=",
        _ => "?"
    };

    public static string ToTurkishName(this ObstacleType type) => type switch
    {
        ObstacleType.None => "Boş",
        ObstacleType.Wolf => "Kurt",
        ObstacleType.Fox => "Tilki",
        ObstacleType.Wire => "Dikenli Tel",
        ObstacleType.Fence => "Çit",
        _ => type.ToString()
    };
}
