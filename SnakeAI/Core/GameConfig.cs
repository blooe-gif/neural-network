namespace SnakeAI.Core;

public sealed class GameConfig
{
    public int Width { get; init; } = 20;
    public int Height { get; init; } = 20;
    public int MaxStepsWithoutFood { get; init; } = 150;

    public int MaxStepsPerEpisode => Width * Height * 8;
}
