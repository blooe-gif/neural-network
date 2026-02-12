using SnakeAI.Core;

namespace SnakeAI.Simulation;

public sealed class StepResult
{
    public required GameState State { get; init; }
    public int StepsSurvived { get; init; }
    public int FoodEaten { get; init; }
    public int StepsSinceLastFood { get; init; }
}
