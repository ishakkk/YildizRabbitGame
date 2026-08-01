namespace YildizRabbitGame;

public enum StepOutcome
{
    Moved,
    TurnedOnly,
    BlockedByBoundary,
    BlockedByObstacle,
    Died,
    ArrivedHome
}


public record StepResult(
    int StepNumber,
    RabbitCommand Command,
    Cell FromCell,
    Cell ToCell,
    Direction FacingAfter,
    ObstacleType EncounteredObstacle,
    StepOutcome Outcome)
{
    public string Describe(int boardSize)
    {
        string cmdLabel = Command switch
        {
            RabbitCommand.Forward => "İleri (N)",
            RabbitCommand.Backward => "Geri (P)",
            RabbitCommand.TurnRight => "Sağ (R)",
            RabbitCommand.TurnLeft => "Sol (L)",
            RabbitCommand.Jump => "Zıpla (J)",
            RabbitCommand.Duck => "Eğil (İ)",
            _ => Command.ToString()
        };

        string outcomeText = Outcome switch
        {
            StepOutcome.Moved => $"{FromCell.ToChessNotation(boardSize)} -> {ToCell.ToChessNotation(boardSize)}"
                + (EncounteredObstacle == ObstacleType.None ? "" : $" ({EncounteredObstacle.ToTurkishName()} geçildi)"),
            StepOutcome.TurnedOnly => $"Yön değişti -> {FacingAfter.ToTurkishLabel()}",
            StepOutcome.BlockedByBoundary => $"{FromCell.ToChessNotation(boardSize)} konumunda kaldı (tahta sınırı dışına çıkıyordu)",
            StepOutcome.BlockedByObstacle => $"{FromCell.ToChessNotation(boardSize)} konumunda kaldı (önünde {EncounteredObstacle.ToTurkishName()} var, yanlış komut)",
            StepOutcome.Died => $"{ToCell.ToChessNotation(boardSize)} konumunda {EncounteredObstacle.ToTurkishName()} ile karşılaştı ve öldü!",
            StepOutcome.ArrivedHome => $"{ToCell.ToChessNotation(boardSize)} konumundaki tavşan deliğine ulaştı!",
            _ => ""
        };

        return $"Adım {StepNumber,2}: {cmdLabel,-11} -> {outcomeText}";
    }
}
