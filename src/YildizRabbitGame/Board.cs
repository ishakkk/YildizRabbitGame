namespace YildizRabbitGame;


public class Board
{
    public int Size { get; }
    public Cell RabbitStart { get; }
    public Cell RabbitHole { get; }

    private readonly ObstacleType[,] _grid;

    public Board(int size)
    {
        if (size <= 1)
            throw new ArgumentOutOfRangeException(nameof(size), "Board size must be greater than 1.");

        Size = size;
        _grid = new ObstacleType[size, size];


        RabbitStart = new Cell(0, 0);
        RabbitHole = new Cell(size - 1, size - 1);
    }

    public ObstacleType GetObstacleAt(Cell cell)
    {
        if (!IsInside(cell))
            throw new ArgumentOutOfRangeException(nameof(cell), $"Cell {cell} is outside the board.");
        return _grid[cell.Row, cell.Col];
    }

    public bool IsInside(Cell cell)
        => cell.Row >= 0 && cell.Row < Size && cell.Col >= 0 && cell.Col < Size;

    public bool IsRabbitHole(Cell cell) => cell == RabbitHole;


    public void PlaceObstaclesRandomly(Random random, int maxPerType = 4)
    {
        ArgumentNullException.ThrowIfNull(random);

        var obstacleTypes = new[] { ObstacleType.Wolf, ObstacleType.Wire, ObstacleType.Fox, ObstacleType.Fence };

        var freeCells = new List<Cell>();
        for (int r = 0; r < Size; r++)
        {
            for (int c = 0; c < Size; c++)
            {
                var cell = new Cell(r, c);
                if (cell == RabbitStart || cell == RabbitHole) continue;
                freeCells.Add(cell);
            }
        }


        for (int i = freeCells.Count - 1; i > 0; i--)
        {
            int j = random.Next(i + 1);
            (freeCells[i], freeCells[j]) = (freeCells[j], freeCells[i]);
        }

        int cursor = 0;
        foreach (var obstacleType in obstacleTypes)
        {
            int count = random.Next(0, maxPerType + 1);
            for (int placed = 0; placed < count && cursor < freeCells.Count; placed++, cursor++)
            {
                var cell = freeCells[cursor];
                _grid[cell.Row, cell.Col] = obstacleType;
            }
        }
    }


    internal void SetObstacleForTesting(Cell cell, ObstacleType type)
    {
        if (!IsInside(cell))
            throw new ArgumentOutOfRangeException(nameof(cell), $"Cell {cell} is outside the board.");
        if (cell == RabbitStart || cell == RabbitHole)
            throw new ArgumentException("Cannot place an obstacle on the start or hole cell.", nameof(cell));

        _grid[cell.Row, cell.Col] = type;
    }


    public void Print(Cell rabbitPosition)
    {
        Console.WriteLine();
        Console.Write("   ");
        for (int c = 0; c < Size; c++)
            Console.Write($" {(char)('A' + c)} ");
        Console.WriteLine();

        for (int r = 0; r < Size; r++)
        {
            int rank = Size - r;
            Console.Write($"{rank,2} ");
            for (int c = 0; c < Size; c++)
            {
                var cell = new Cell(r, c);
                string symbol;
                if (cell == rabbitPosition) symbol = "R";
                else if (cell == RabbitHole) symbol = "H";
                else symbol = GetObstacleAt(cell).ToDisplaySymbol();

                Console.Write($" {symbol} ");
            }
            Console.WriteLine();
        }
        Console.WriteLine();
        Console.WriteLine("R = Tavşan (Yıldız), H = Tavşan Deliği, K = Kurt, T = Tilki, # = Dikenli Tel, = = Çit");
        Console.WriteLine();
    }
}
