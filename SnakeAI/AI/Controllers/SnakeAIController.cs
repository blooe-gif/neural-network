using System;
using System.Drawing;
using SnakeAI.AI.Neural;
using SnakeAI.Core;

namespace SnakeAI.AI.Controllers;

public sealed class SnakeAIController
{
    private readonly NeuralNetwork _network;

    public SnakeAIController(NeuralNetwork network)
    {
        _network = network;
    }

    public NeuralNetwork Network => _network;
    public double[] LastInputs { get; private set; } = Array.Empty<double>();
    public double[] LastOutputs { get; private set; } = Array.Empty<double>();

    public Direction Decide(GameState state)
    {
        LastInputs = BuildInputs(state);
        var rawOutputs = _network.Forward(LastInputs);
        LastOutputs = Softmax(rawOutputs);

        if (LastOutputs.Length < 4)
        {
            return state.Snake.Direction;
        }

        var direction = ArgMaxDirection(LastOutputs);
        if (IsReverse(state.Snake.Direction, direction))
        {
            return state.Snake.Direction;
        }

        return direction;
    }

    private static double[] BuildInputs(GameState state)
    {
        var head = state.Snake.Head;
        var food = state.Food.Position;

        var dx = (food.X - head.X) / (double)state.Config.Width;
        var dy = (food.Y - head.Y) / (double)state.Config.Height;

        var front = NextPoint(head, state.Snake.Direction);
        var left = NextPoint(head, TurnLeft(state.Snake.Direction));
        var right = NextPoint(head, TurnRight(state.Snake.Direction));

        return
        [
            dx,
            dy,
            state.IsDanger(front) ? 1.0 : 0.0,
            state.IsDanger(left) ? 1.0 : 0.0,
            state.IsDanger(right) ? 1.0 : 0.0,
            (int)state.Snake.Direction / 3.0
        ];
    }

    private static Direction ArgMaxDirection(double[] outputs)
    {
        var maxIndex = 0;
        var maxValue = outputs[0];

        for (var i = 1; i < outputs.Length; i++)
        {
            if (outputs[i] > maxValue)
            {
                maxValue = outputs[i];
                maxIndex = i;
            }
        }

        return (Direction)maxIndex;
    }

    private static double[] Softmax(double[] values)
    {
        if (values.Length == 0)
        {
            return Array.Empty<double>();
        }

        var result = new double[values.Length];
        var max = values[0];
        for (var i = 1; i < values.Length; i++)
        {
            if (values[i] > max)
            {
                max = values[i];
            }
        }

        var sum = 0.0;
        for (var i = 0; i < values.Length; i++)
        {
            result[i] = Math.Exp(values[i] - max);
            sum += result[i];
        }

        if (sum <= 0)
        {
            var uniform = 1.0 / values.Length;
            for (var i = 0; i < result.Length; i++)
            {
                result[i] = uniform;
            }

            return result;
        }

        for (var i = 0; i < result.Length; i++)
        {
            result[i] /= sum;
        }

        return result;
    }

    private static Point NextPoint(Point p, Direction d)
    {
        return d switch
        {
            Direction.Up => new Point(p.X, p.Y - 1),
            Direction.Right => new Point(p.X + 1, p.Y),
            Direction.Down => new Point(p.X, p.Y + 1),
            Direction.Left => new Point(p.X - 1, p.Y),
            _ => p
        };
    }

    private static Direction TurnLeft(Direction direction) => (Direction)(((int)direction + 3) % 4);
    private static Direction TurnRight(Direction direction) => (Direction)(((int)direction + 1) % 4);

    private static bool IsReverse(Direction current, Direction next)
    {
        return (current == Direction.Up && next == Direction.Down) ||
               (current == Direction.Down && next == Direction.Up) ||
               (current == Direction.Left && next == Direction.Right) ||
               (current == Direction.Right && next == Direction.Left);
    }
}
