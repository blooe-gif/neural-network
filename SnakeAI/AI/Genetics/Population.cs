using System;
using System.Collections.Generic;
using System.Linq;
using SnakeAI.Utils;

namespace SnakeAI.AI.Genetics;

public sealed class Population
{
    private readonly List<Genome> _genomes;
    private readonly RandomProvider _random;

    public Population(int size, int genomeLength, int seed)
    {
        _genomes = new List<Genome>(size);
        _random = new RandomProvider(seed);
        for (var i = 0; i < size; i++)
        {
            var genome = new Genome(genomeLength);
            genome.Randomize(_random);
            _genomes.Add(genome);
        }
    }

    public int Generation { get; private set; }
    public IReadOnlyList<Genome> Genomes => _genomes;
    public Genome BestGenome => _genomes.OrderByDescending(g => g.Fitness).First();
    public double BestFitness => BestGenome.Fitness;

    public void NextGeneration(double mutationRate = 0.07, double mutationScale = 0.35, int eliteCount = 4)
    {
        var ordered = _genomes.OrderByDescending(g => g.Fitness).ToList();
        var next = new List<Genome>(_genomes.Count);

        for (var i = 0; i < eliteCount && i < ordered.Count; i++)
        {
            next.Add(ordered[i].Clone());
        }

        while (next.Count < _genomes.Count)
        {
            var parentA = TournamentPick(ordered, 4);
            var parentB = TournamentPick(ordered, 4);
            var child = Crossover(parentA, parentB);
            Mutate(child, mutationRate, mutationScale);
            next.Add(child);
        }

        _genomes.Clear();
        _genomes.AddRange(next);
        Generation++;
    }

    private Genome TournamentPick(List<Genome> pool, int tournamentSize)
    {
        Genome? best = null;
        for (var i = 0; i < tournamentSize; i++)
        {
            var contender = pool[_random.Next(0, pool.Count)];
            if (best is null || contender.Fitness > best.Fitness)
            {
                best = contender;
            }
        }

        return best!;
    }

    private Genome Crossover(Genome a, Genome b)
    {
        var child = new Genome(a.Genes.Length);
        for (var i = 0; i < child.Genes.Length; i++)
        {
            child.Genes[i] = _random.NextDouble() < 0.5 ? a.Genes[i] : b.Genes[i];
        }

        return child;
    }

    private void Mutate(Genome genome, double mutationRate, double mutationScale)
    {
        for (var i = 0; i < genome.Genes.Length; i++)
        {
            if (_random.NextDouble() < mutationRate)
            {
                genome.Genes[i] += _random.NextDouble(-mutationScale, mutationScale);
                genome.Genes[i] = Math.Clamp(genome.Genes[i], -2.5, 2.5);
            }
        }
    }
}
