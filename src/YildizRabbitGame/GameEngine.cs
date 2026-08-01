namespace YildizRabbitGame;


public enum GameResult
{
    ReachedHole,
    Died,
    ScenarioEndedWithoutArriving
}


public class GameEngine
{
    private readonly Board _board;
    private readonly Rabbit _rabbit;

    public IReadOnlyList<StepResult> StepLog => _stepLog;
    private readonly List<StepResult> _stepLog = new();

    public GameResult Result { get; private set; } = GameResult.ScenarioEndedWithoutArriving;

    public GameEngine(Board board, Rabbit rabbit)
    {
        _board = board ?? throw new ArgumentNullException(nameof(board));
        _rabbit = rabbit ?? throw new ArgumentNullException(nameof(rabbit));
    }

    public GameResult Run(IEnumerable<RabbitCommand> commands)
    {
        ArgumentNullException.ThrowIfNull(commands);

        int stepNumber = 0;

        foreach (var command in commands)
        {
            stepNumber++;

            if (!_rabbit.IsAlive || _rabbit.HasArrivedHome)
                break;

            ExecuteCommand(stepNumber, command);

            if (!_rabbit.IsAlive)
            {
                Result = GameResult.Died;
                return Result;
            }

            if (_rabbit.HasArrivedHome)
            {
                Result = GameResult.ReachedHole;
                return Result;
            }
        }

        Result = GameResult.ScenarioEndedWithoutArriving;
        return Result;
    }

    private void ExecuteCommand(int stepNumber, RabbitCommand command)
    {
        switch (command)
        {
            case RabbitCommand.TurnRight:
                _rabbit.Facing = _rabbit.Facing.TurnRight();
                LogTurn(stepNumber, command);
                return;

            case RabbitCommand.TurnLeft:
                _rabbit.Facing = _rabbit.Facing.TurnLeft();
                LogTurn(stepNumber, command);
                return;

            case RabbitCommand.Forward:
                MoveBy(stepNumber, command, distance: 1, direction: _rabbit.Facing);
                return;

            case RabbitCommand.Backward:

                MoveBy(stepNumber, command, distance: 1, direction: _rabbit.Facing.Opposite());
                return;

            case RabbitCommand.Jump:
                MoveBy(stepNumber, command, distance: 2, direction: _rabbit.Facing);
                return;

            case RabbitCommand.Duck:
                MoveBy(stepNumber, command, distance: 1, direction: _rabbit.Facing);
                return;
        }
    }

    private void LogTurn(int stepNumber, RabbitCommand command)
    {
        _stepLog.Add(new StepResult(
            stepNumber, command, _rabbit.Position, _rabbit.Position,
            _rabbit.Facing, ObstacleType.None, StepOutcome.TurnedOnly));
    }

    private void MoveBy(int stepNumber, RabbitCommand command, int distance, Direction direction)
    {
        var (rowDelta, colDelta) = direction.ToStep();
        var from = _rabbit.Position;
        var target = new Cell(from.Row + rowDelta * distance, from.Col + colDelta * distance);

        if (!_board.IsInside(target))
        {
            Log(stepNumber, command, from, from, ObstacleType.None, StepOutcome.BlockedByBoundary);
            return;
        }

        var obstacle = _board.GetObstacleAt(target);


        if (obstacle == ObstacleType.Fence)
        {
            Log(stepNumber, command, from, from, obstacle, StepOutcome.BlockedByObstacle);
            return;
        }


        if (obstacle == ObstacleType.Wire && command != RabbitCommand.Duck)
        {
            Log(stepNumber, command, from, from, obstacle, StepOutcome.BlockedByObstacle);
            return;
        }


        if (obstacle.IsDeadly())
        {
            _rabbit.Position = target;
            _rabbit.Kill();
            Log(stepNumber, command, from, target, obstacle, StepOutcome.Died);
            return;
        }

        _rabbit.Position = target;

        if (_board.IsRabbitHole(target))
        {
            _rabbit.MarkArrivedHome();
            Log(stepNumber, command, from, target, obstacle, StepOutcome.ArrivedHome);
            return;
        }

        Log(stepNumber, command, from, target, obstacle, StepOutcome.Moved);
    }

    private void Log(int stepNumber, RabbitCommand command, Cell from, Cell to, ObstacleType obstacle, StepOutcome outcome)
    {
        _stepLog.Add(new StepResult(stepNumber, command, from, to, _rabbit.Facing, obstacle, outcome));
    }
}
