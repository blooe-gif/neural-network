namespace SnakeAI.UI;

public sealed class UIConfig
{
    public int WindowWidth { get; init; } = 1200;
    public int WindowHeight { get; init; } = 760;
    public int TimerIntervalMs { get; init; } = 45;
    public int TrainingBurstPerTick { get; init; } = 2;
}
