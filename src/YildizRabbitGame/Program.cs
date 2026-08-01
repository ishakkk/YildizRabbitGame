using YildizRabbitGame;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=====================================================");
Console.WriteLine("   YILDIZ - Tavşanı Eve Götür (.NET C# Console)");
Console.WriteLine("=====================================================");
Console.WriteLine();

int boardSize = AskBoardSize();
int? seed = AskOptionalSeed();
var random = seed.HasValue ? new Random(seed.Value) : new Random();

var board = new Board(boardSize);
board.PlaceObstaclesRandomly(random);

var rabbit = new Rabbit(board.RabbitStart, Direction.South);

Console.WriteLine();
Console.WriteLine($"Orman büyüklüğü: {boardSize}x{boardSize}");
Console.WriteLine($"Tavşan başlangıcı: {board.RabbitStart.ToChessNotation(boardSize)}  |  Yön: {rabbit.Facing.ToTurkishLabel()}");
Console.WriteLine($"Tavşan deliği: {board.RabbitHole.ToChessNotation(boardSize)}");
board.Print(rabbit.Position);

string scenario = AskScenario();

List<RabbitCommand> commands;
try
{
    commands = RabbitCommandParser.Parse(scenario);
}
catch (FormatException ex)
{
    Console.WriteLine();
    Console.WriteLine($"HATA: {ex.Message}");
    return;
}

var engine = new GameEngine(board, rabbit);
var result = engine.Run(commands);

Console.WriteLine();
Console.WriteLine("---------------------- Adım Adım İzleme ----------------------");
foreach (var step in engine.StepLog)
{
    Console.WriteLine(step.Describe(boardSize));
}

Console.WriteLine();
Console.WriteLine("------------------------------ Sonuç --------------------------");
board.Print(rabbit.Position);

switch (result)
{
    case GameResult.ReachedHole:
        Console.WriteLine("SONUÇ: Tebrikler! Yıldız tavşan deliğine güvenle ulaştı. 🐇🏡");
        break;
    case GameResult.Died:
        Console.WriteLine("SONUÇ: Yıldız ormanda bir engelle çarpıştı ve hayatını kaybetti. 💀");
        break;
    case GameResult.ScenarioEndedWithoutArriving:
        Console.WriteLine("SONUÇ: Senaryo sona erdi ancak Yıldız henüz eve ulaşamadı.");
        break;
}

Console.WriteLine();
Console.WriteLine("Çıkmak için bir tuşa basın...");
Console.ReadKey();



static int AskBoardSize()
{
    var options = new Dictionary<string, int> { ["1"] = 4, ["2"] = 8, ["3"] = 16 };

    while (true)
    {
        Console.WriteLine("Orman büyüklüğünü seçin (tek rakam):");
        Console.WriteLine("  1 -> 4x4");
        Console.WriteLine("  2 -> 8x8");
        Console.WriteLine("  3 -> 16x16");
        Console.Write("Seçiminiz: ");
        string? input = Console.ReadLine()?.Trim();

        if (input != null && options.TryGetValue(input, out int size))
            return size;


        if (input == "4" || input == "8" || input == "16")
            return int.Parse(input);

        Console.WriteLine("Geçersiz seçim, tekrar deneyin.");
        Console.WriteLine();
    }
}

static int? AskOptionalSeed()
{
    Console.Write("Rastgelelik için bir sayı (seed) girmek ister misiniz? (Boş bırakabilirsiniz): ");
    string? input = Console.ReadLine()?.Trim();

    if (int.TryParse(input, out int seed))
        return seed;

    return null;
}

static string AskScenario()
{
    Console.WriteLine();
    Console.WriteLine("Hareket senaryosunu girin (virgülle ayrılmış).");
    Console.WriteLine("Komutlar: İleri=N, Geri=P, Sağ=R, Sol=L, Zıpla=J (Çit için), Eğil=İ (Tel için)");
    Console.WriteLine("Örnek: N,N,L,J,N,N,İ,P,J");
    Console.Write("Senaryo: ");
    return Console.ReadLine() ?? string.Empty;
}
