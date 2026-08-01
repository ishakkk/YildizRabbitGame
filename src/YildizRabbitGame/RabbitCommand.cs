namespace YildizRabbitGame;

public enum RabbitCommand
{
    Forward,
    Backward,
    TurnRight,
    TurnLeft,
    Jump,
    Duck
}

public static class RabbitCommandParser
{

    public static List<RabbitCommand> Parse(string scenario)
    {
        if (string.IsNullOrWhiteSpace(scenario))
            return new List<RabbitCommand>();

        var tokens = scenario.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var commands = new List<RabbitCommand>(tokens.Length);

        foreach (var token in tokens)
        {
            commands.Add(ParseSingle(token));
        }

        return commands;
    }

    private static RabbitCommand ParseSingle(string token)
    {

        string key = token.Trim().ToUpperInvariant().Replace("İ", "I");

        return key switch
        {
            "N" => RabbitCommand.Forward,
            "P" => RabbitCommand.Backward,
            "R" => RabbitCommand.TurnRight,
            "L" => RabbitCommand.TurnLeft,
            "J" => RabbitCommand.Jump,
            "I" => RabbitCommand.Duck,
            _ => throw new FormatException(
                $"Geçersiz komut: '{token}'. Kabul edilen komutlar: N, P, R, L, J, İ")
        };
    }
}
