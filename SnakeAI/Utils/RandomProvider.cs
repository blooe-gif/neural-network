using System;

namespace SnakeAI.Utils;

public sealed class RandomProvider
{
    private readonly Random _random;

    public RandomProvider(int seed)
    {
        _random = new Random(seed);
    }

    public int Next(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);

    public double NextDouble() => _random.NextDouble();

    public double NextDouble(double minInclusive, double maxInclusive)
    {
        return minInclusive + (maxInclusive - minInclusive) * _random.NextDouble();
    }
}
