using System.Drawing;
using SnakeAI.Core;

namespace SnakeAI.Rendering;

public sealed class GameRenderer
{
    public void Draw(Graphics g, Rectangle area, GameState state)
    {
        g.Clear(Color.FromArgb(18, 18, 20));

        var cellW = area.Width / (float)state.Config.Width;
        var cellH = area.Height / (float)state.Config.Height;

        using var gridPen = new Pen(Color.FromArgb(35, 35, 40), 1f);
        for (var x = 0; x <= state.Config.Width; x++)
        {
            g.DrawLine(gridPen, x * cellW, 0, x * cellW, area.Height);
        }

        for (var y = 0; y <= state.Config.Height; y++)
        {
            g.DrawLine(gridPen, 0, y * cellH, area.Width, y * cellH);
        }

        using var foodBrush = new SolidBrush(Color.OrangeRed);
        var food = state.Food.Position;
        g.FillEllipse(foodBrush, food.X * cellW + 1, food.Y * cellH + 1, cellW - 2, cellH - 2);

        using var bodyBrush = new SolidBrush(Color.FromArgb(40, 180, 110));
        using var headBrush = new SolidBrush(Color.FromArgb(80, 240, 140));

        foreach (var segment in state.Snake.Body)
        {
            g.FillRectangle(bodyBrush, segment.X * cellW + 1, segment.Y * cellH + 1, cellW - 2, cellH - 2);
        }

        var head = state.Snake.Head;
        g.FillRectangle(headBrush, head.X * cellW + 1, head.Y * cellH + 1, cellW - 2, cellH - 2);
    }
}
