using System;
using System.Collections.Generic;
using System.Drawing;
using SnakeAI.Utils;

namespace SnakeAI.Core;

public sealed class GameState
{
    private readonly RandomProvider _random;

    public GameState(GameConfig config, RandomProvider random)
    {
        Config = config;
        _random = random;
        Snake = new Snake(new Point(config.Width / 2, config.Height / 2));
        Food = new Food();
        SpawnFood();
    }

    public GameConfig Config { get; }
    public Snake Snake { get; }
    public Food Food { get; }
    public bool IsAlive { get; private set; } = true;
    public int Score { get; private set; }
    public int Steps { get; private set; }
    public int StepsSinceFood { get; private set; }

    public void Step(Direction direction)
    {
        if (!IsAlive)
        {
            return;
        }

        Snake.SetDirection(direction);
        var next = Snake.NextHeadPosition();

        if (IsWall(next) || Snake.Occupies(next, includeTail: false))
        {
            IsAlive = false;
            return;
        }

        var ateFood = next == Food.Position;
        Snake.Move(ateFood);
        Steps++;
        StepsSinceFood++;

        if (ateFood)
        {
            Score++;
            StepsSinceFood = 0;
            SpawnFood();
        }

        if (StepsSinceFood >= Config.MaxStepsWithoutFood || Steps >= Config.MaxStepsPerEpisode)
        {
            IsAlive = false;
        }
    }

    public bool IsDanger(Point p)
    {
        return IsWall(p) || Snake.Occupies(p, includeTail: false);
    }

    private bool IsWall(Point p)
    {
        return p.X < 0 || p.Y < 0 || p.X >= Config.Width || p.Y >= Config.Height;
    }

    private void SpawnFood()
    {
        var free = new List<Point>();
        for (var y = 0; y < Config.Height; y++)
        {
            for (var x = 0; x < Config.Width; x++)
            {
                var p = new Point(x, y);
                if (!Snake.Occupies(p))
                {
                    free.Add(p);
                }
            }
        }

        if (free.Count == 0)
        {
            IsAlive = false;
            return;
        }

        Food.Position = free[_random.Next(0, free.Count)];
    }
}
