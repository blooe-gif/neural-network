using System;
using System.Collections.Generic;
using System.Drawing;
using SnakeAI.AI.Neural;

namespace SnakeAI.Rendering;

public sealed class NeuralRenderer
{
    public void Draw(Graphics g, Rectangle area, NeuralNetwork network)
    {
        g.Clear(Color.FromArgb(22, 22, 26));

        var layers = network.Layers;
        var positions = new List<PointF[]>();
        var fullSizes = new List<int> { network.LayerSizes[0] };
        for (var i = 0; i < layers.Count; i++)
        {
            fullSizes.Add(network.LayerSizes[i + 1]);
        }

        for (var li = 0; li < fullSizes.Count; li++)
        {
            var count = fullSizes[li];
            var nodes = new PointF[count];
            var x = area.Left + (li + 1) * (area.Width / (float)(fullSizes.Count + 1));
            var gap = area.Height / (float)(count + 1);
            for (var ni = 0; ni < count; ni++)
            {
                nodes[ni] = new PointF(x, area.Top + (ni + 1) * gap);
            }

            positions.Add(nodes);
        }

        for (var li = 0; li < layers.Count; li++)
        {
            var layer = layers[li];
            var src = positions[li];
            var dst = positions[li + 1];
            for (var o = 0; o < dst.Length; o++)
            {
                for (var i = 0; i < src.Length; i++)
                {
                    var w = layer.Weights[o, i];
                    var mag = (float)Math.Min(Math.Abs(w), 1.0);
                    var color = w >= 0
                        ? Color.FromArgb(40 + (int)(180 * mag), 80, 220, 120)
                        : Color.FromArgb(40 + (int)(180 * mag), 230, 80, 80);
                    using var pen = new Pen(color, 1f + 3f * mag);
                    g.DrawLine(pen, src[i], dst[o]);
                }
            }
        }

        for (var li = 0; li < positions.Count; li++)
        {
            for (var ni = 0; ni < positions[li].Length; ni++)
            {
                var p = positions[li][ni];
                float value = 0f;

                if (li > 0)
                {
                    var layer = layers[li - 1];
                    if (ni < layer.LastOutput.Length)
                    {
                        value = (float)Math.Clamp((layer.LastOutput[ni] + 1) * 0.5, 0.0, 1.0);
                    }
                }

                var nodeColor = Color.FromArgb(
                    255,
                    (int)(50 + 180 * value),
                    (int)(50 + 120 * value),
                    (int)(70 + 140 * value));

                using var brush = new SolidBrush(nodeColor);
                using var border = new Pen(Color.FromArgb(20, 20, 20), 1.5f);
                g.FillEllipse(brush, p.X - 8, p.Y - 8, 16, 16);
                g.DrawEllipse(border, p.X - 8, p.Y - 8, 16, 16);
            }
        }
    }
}
