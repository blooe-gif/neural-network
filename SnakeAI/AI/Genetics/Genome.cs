using System;
using SnakeAI.Utils;

namespace SnakeAI.AI.Genetics;

public sealed class Genome
{
    public Genome(int size)
    {
        Genes = new double[size];
    }

    public double[] Genes { get; }
    public double Fitness { get; set; }

    public Genome Clone()
    {
        var g = new Genome(Genes.Length);
        Array.Copy(Genes, g.Genes, Genes.Length);
        g.Fitness = Fitness;
        return g;
    }

    public void Randomize(RandomProvider random, double scale = 0.25)
    {
        for (var i = 0; i < Genes.Length; i++)
        {
            Genes[i] = random.NextDouble(-scale, scale);
        }
    }
}
