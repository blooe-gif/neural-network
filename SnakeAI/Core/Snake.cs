using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace SnakeAI.Core;

public sealed class Snake
{
    private readonly LinkedList<Point> _body = new();

    public Snake(Point start)
    {
        _body.AddFirst(start);
        _body.AddLast(new Point(start.X - 1, start.Y));
        _body.AddLast(new Point(start.X - 2, start.Y));
    }

    public IReadOnlyCollection<Point> Body => _body;
    public Point Head => _body.First!.Value;
    public Direction Direction { get; private set; } = Direction.Right;

    public void SetDirection(Direction direction)
    {
        if (IsReverse(direction))
        {
            return;
        }

        Direction = direction;
    }

    public Point NextHeadPosition()
    {
        var head = Head;
        return Direction switch
        {
            Direction.Up => new Point(head.X, head.Y - 1),
            Direction.Right => new Point(head.X + 1, head.Y),
            Direction.Down => new Point(head.X, head.Y + 1),
            Direction.Left => new Point(head.X - 1, head.Y),
            _ => head
        };
    }

    public void Move(bool grow)
    {
        _body.AddFirst(NextHeadPosition());
        if (!grow)
        {
            _body.RemoveLast();
        }
    }

    public bool Occupies(Point p, bool includeTail = true)
    {
        if (includeTail)
        {
            return _body.Contains(p);
        }

        return _body.Take(_body.Count - 1).Contains(p);
    }

    private bool IsReverse(Direction next)
    {
        return (Direction == Direction.Up && next == Direction.Down) ||
               (Direction == Direction.Down && next == Direction.Up) ||
               (Direction == Direction.Left && next == Direction.Right) ||
               (Direction == Direction.Right && next == Direction.Left);
    }
}
