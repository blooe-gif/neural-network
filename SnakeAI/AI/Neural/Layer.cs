using System;

namespace SnakeAI.AI.Neural;

public sealed class Layer
{
    public Layer(int inputSize, int outputSize)
    {
        Weights = new double[outputSize, inputSize];
        Biases = new double[outputSize];
        LastOutput = new double[outputSize];
    }

    public double[,] Weights { get; }
    public double[] Biases { get; }
    public double[] LastOutput { get; }

    public double[] Forward(double[] input, bool useActivation)
    {
        for (var i = 0; i < Biases.Length; i++)
        {
            var sum = Biases[i];
            for (var j = 0; j < input.Length; j++)
            {
                sum += Weights[i, j] * input[j];
            }

            LastOutput[i] = useActivation ? Activation.Tanh(sum) : sum;
        }

        var copy = new double[LastOutput.Length];
        Array.Copy(LastOutput, copy, copy.Length);
        return copy;
    }
}
