using Xunit;

namespace YildizRabbitGame.Tests;

public class GameEngineTests
{


    [Fact]
    public void Duck_Passes_Safely_Through_Wire()
    {
        var board = new Board(4);
        board.SetObstacleForTesting(new Cell(1, 0), ObstacleType.Wire);
        var rabbit = new Rabbit(board.RabbitStart, Direction.South);
        var engine = new GameEngine(board, rabbit);

        var result = engine.Run(new[] { RabbitCommand.Duck });

        Assert.True(rabbit.IsAlive);
        Assert.Equal(new Cell(1, 0), rabbit.Position);
        Assert.Equal(StepOutcome.Moved, engine.StepLog[0].Outcome);
        Assert.Equal(GameResult.ScenarioEndedWithoutArriving, result);
    }

    [Fact]
    public void Forward_Is_Blocked_By_Wire_Without_Ducking()
    {
        var board = new Board(4);
        board.SetObstacleForTesting(new Cell(1, 0), ObstacleType.Wire);
        var rabbit = new Rabbit(board.RabbitStart, Direction.South);
        var engine = new GameEngine(board, rabbit);

        _ = engine.Run(new[] { RabbitCommand.Forward });

        Assert.True(rabbit.IsAlive);
        Assert.Equal(board.RabbitStart, rabbit.Position);
        Assert.Equal(StepOutcome.BlockedByObstacle, engine.StepLog[0].Outcome);
    }

    [Fact]
    public void Jump_Clears_A_Fence_On_The_Intermediate_Cell()
    {
        var board = new Board(4);
        board.SetObstacleForTesting(new Cell(1, 0), ObstacleType.Fence);
        var rabbit = new Rabbit(board.RabbitStart, Direction.South);
        var engine = new GameEngine(board, rabbit);

        _ = engine.Run(new[] { RabbitCommand.Jump });

        Assert.True(rabbit.IsAlive);
        Assert.Equal(new Cell(2, 0), rabbit.Position);
        Assert.Equal(StepOutcome.Moved, engine.StepLog[0].Outcome);
    }

    [Fact]
    public void Jump_Is_Blocked_If_Landing_Cell_Is_Also_A_Fence()
    {
        var board = new Board(4);
        board.SetObstacleForTesting(new Cell(2, 0), ObstacleType.Fence);
        var rabbit = new Rabbit(board.RabbitStart, Direction.South);
        var engine = new GameEngine(board, rabbit);

        _ = engine.Run(new[] { RabbitCommand.Jump });

        Assert.True(rabbit.IsAlive);
        Assert.Equal(board.RabbitStart, rabbit.Position);
        Assert.Equal(StepOutcome.BlockedByObstacle, engine.StepLog[0].Outcome);
    }

    [Fact]
    public void Moving_Into_Wolf_Kills_The_Rabbit()
    {
        var board = new Board(4);
        board.SetObstacleForTesting(new Cell(1, 0), ObstacleType.Wolf);
        var rabbit = new Rabbit(board.RabbitStart, Direction.South);
        var engine = new GameEngine(board, rabbit);

        var result = engine.Run(new[] { RabbitCommand.Forward, RabbitCommand.Forward });

        Assert.False(rabbit.IsAlive);
        Assert.Equal(GameResult.Died, result);
        _ = Assert.Single(engine.StepLog);
    }

    [Fact]
    public void Moving_Into_Fox_Kills_The_Rabbit_Regardless_Of_Command()
    {
        var board = new Board(4);
        board.SetObstacleForTesting(new Cell(1, 0), ObstacleType.Fox);
        var rabbit = new Rabbit(board.RabbitStart, Direction.South);
        var engine = new GameEngine(board, rabbit);


        var result = engine.Run(new[] { RabbitCommand.Duck });

        Assert.False(rabbit.IsAlive);
        Assert.Equal(GameResult.Died, result);
    }

    [Fact]
    public void Moving_Off_The_Board_Is_Blocked_Not_Fatal()
    {
        var board = new Board(4);
        var rabbit = new Rabbit(board.RabbitStart, Direction.North);
        var engine = new GameEngine(board, rabbit);
        _ = engine.Run(new[] { RabbitCommand.Forward });

        Assert.True(rabbit.IsAlive);
        Assert.Equal(board.RabbitStart, rabbit.Position);
        Assert.Equal(StepOutcome.BlockedByBoundary, engine.StepLog[0].Outcome);
    }

    [Theory]
    [InlineData(Direction.North, Direction.East)]
    [InlineData(Direction.East, Direction.South)]
    [InlineData(Direction.South, Direction.West)]
    [InlineData(Direction.West, Direction.North)]
    public void TurnRight_Rotates_Clockwise(Direction start, Direction expected)
    {
        Assert.Equal(expected, start.TurnRight());
    }

    [Theory]
    [InlineData(Direction.North, Direction.West)]
    [InlineData(Direction.West, Direction.South)]
    [InlineData(Direction.South, Direction.East)]
    [InlineData(Direction.East, Direction.North)]
    public void TurnLeft_Rotates_Counterclockwise(Direction start, Direction expected)
    {
        Assert.Equal(expected, start.TurnLeft());
    }

    [Fact]
    public void Backward_Moves_Opposite_Facing_Without_Turning()
    {
        var board = new Board(4);
        var rabbit = new Rabbit(new Cell(1, 1), Direction.South);
        var engine = new GameEngine(board, rabbit);

        _ = engine.Run(new[] { RabbitCommand.Backward });

        Assert.Equal(new Cell(0, 1), rabbit.Position);
        Assert.Equal(Direction.South, rabbit.Facing);
    }

    [Fact]
    public void Clear_Path_Reaches_The_Rabbit_Hole()
    {
        var board = new Board(4);
        var rabbit = new Rabbit(board.RabbitStart, Direction.South);
        var engine = new GameEngine(board, rabbit);


        var commands = new[]
        {
            RabbitCommand.Forward, RabbitCommand.Forward, RabbitCommand.Forward,
            RabbitCommand.TurnLeft,
            RabbitCommand.Forward, RabbitCommand.Forward, RabbitCommand.Forward
        };

        var result = engine.Run(commands);

        Assert.Equal(GameResult.ReachedHole, result);
        Assert.True(rabbit.HasArrivedHome);
        Assert.Equal(board.RabbitHole, rabbit.Position);
    }

    [Fact]
    public void PlaceObstaclesRandomly_Never_Exceeds_Max_Per_Type_And_Never_Overlaps()
    {
        var board = new Board(8);
        board.PlaceObstaclesRandomly(new Random(1234), maxPerType: 4);

        var counts = new Dictionary<ObstacleType, int>
        {
            [ObstacleType.Wolf] = 0,
            [ObstacleType.Fox] = 0,
            [ObstacleType.Wire] = 0,
            [ObstacleType.Fence] = 0
        };

        for (int r = 0; r < board.Size; r++)
        {
            for (int c = 0; c < board.Size; c++)
            {
                var cell = new Cell(r, c);
                var obstacle = board.GetObstacleAt(cell);
                if (obstacle == ObstacleType.None) continue;

                Assert.NotEqual(board.RabbitStart, cell);
                Assert.NotEqual(board.RabbitHole, cell);
                counts[obstacle]++;
            }
        }

        foreach (var count in counts.Values)
        {
            Assert.InRange(count, 0, 4);
        }
    }
}
