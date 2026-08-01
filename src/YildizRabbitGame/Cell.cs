namespace YildizRabbitGame;

public readonly record struct Cell(int Row, int Col)
{
    public override string ToString() => $"({Row},{Col})";


    public string ToChessNotation(int boardSize)
    {
        char column = (char)('A' + Col);
        int rank = boardSize - Row;
        return $"{column}{rank}";
    }
}
