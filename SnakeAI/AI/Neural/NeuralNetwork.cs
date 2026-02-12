using System;
using System.Collections.Generic;
using SnakeAI.Utils;

namespace SnakeAI.AI.Neural;

public sealed class NeuralNetwork
{
    private readonly List<Layer> _layers = new();

    public NeuralNetwork(params int[] sizes)
    {
        if (sizes.Length < 2)
        {
            throw new ArgumentException("Network needs at least input and output layers.");
        }

        LayerSizes = sizes;
        for (var i = 0; i < sizes.Length - 1; i++)
        {
            _layers.Add(new Layer(sizes[i], sizes[i + 1]));
        }
    }

    public int[] LayerSizes { get; }
    public IReadOnlyList<Layer> Layers => _layers;

    public void Randomize(RandomProvider random, double scale = 1.0)
    {
        foreach (var layer in _layers)
        {
            for (var o = 0; o < layer.Biases.Length; o++)
            {
                layer.Biases[o] = random.NextDouble(-scale, scale);
                for (var i = 0; i < layer.Weights.GetLength(1); i++)
                {
                    layer.Weights[o, i] = random.NextDouble(-scale, scale);
                }
            }
        }
    }

    public double[] Forward(double[] input)
    {
        var current = input;
        for (var i = 0; i < _layers.Count; i++)
        {
            var isOutput = i == _layers.Count - 1;
            current = _layers[i].Forward(current, useActivation: !isOutput);
        }

        return current;
    }

    public double[] ToGenome()
    {
        var values = new List<double>();
        foreach (var layer in _layers)
        {
            for (var o = 0; o < layer.Biases.Length; o++)
            {
                for (var i = 0; i < layer.Weights.GetLength(1); i++)
                {
                    values.Add(layer.Weights[o, i]);
                }

                values.Add(layer.Biases[o]);
            }
        }

        return values.ToArray();
    }

    public void FromGenome(double[] genes)
    {
        var index = 0;
        foreach (var layer in _layers)
        {
            for (var o = 0; o < layer.Biases.Length; o++)
            {
                for (var i = 0; i < layer.Weights.GetLength(1); i++)
                {
                    layer.Weights[o, i] = genes[index++];
                }

                layer.Biases[o] = genes[index++];
            }
        }
    }
}
