using SnakeAI.AI.Controllers;
using SnakeAI.AI.Genetics;
using SnakeAI.AI.Neural;
using SnakeAI.Core;
using SnakeAI.Utils;

namespace SnakeAI.Simulation;

public sealed class Simulator
{
    private readonly int[] _networkShape;

    public Simulator(GameConfig config, int seed, int populationSize = 64, params int[] networkShape)
    {
        Config = config;
        Seed = seed;
        _networkShape = networkShape.Length > 0 ? networkShape : [6, 10, 4];

        var template = new NeuralNetwork(_networkShape);
        var geneLength = template.ToGenome().Length;
        Population = new Population(populationSize, geneLength, seed);

        PlaybackController = CreateControllerFromGenome(Population.BestGenome);
        PlaybackState = CreateInitialState(seed + 999);
    }

    public GameConfig Config { get; }
    public int Seed { get; }
    public Population Population { get; }
    public GameState PlaybackState { get; private set; }
    public SnakeAIController PlaybackController { get; private set; }
    public int LastFoodEaten { get; private set; }

    public void EvolveOneGeneration()
    {
        for (var i = 0; i < Population.Genomes.Count; i++)
        {
            var result = EvaluateGenome(Population.Genomes[i], Seed + Population.Generation * 1000 + i * 31);
            Population.Genomes[i].Fitness = Fitness.Evaluate(result);
        }

        var best = Population.BestGenome.Clone();
        PlaybackController = CreateControllerFromGenome(best);
        PlaybackState = CreateInitialState(Seed + Population.Generation * 17 + 7);
        LastFoodEaten = 0;

        Population.NextGeneration();
    }

    public void UpdatePlaybackStep()
    {
        if (!PlaybackState.IsAlive)
        {
            PlaybackState = CreateInitialState(Seed + Population.Generation * 17 + 7);
            LastFoodEaten = 0;
        }

        var direction = PlaybackController.Decide(PlaybackState);
        PlaybackState.Step(direction);
        LastFoodEaten = PlaybackState.Score;
    }

    public StepResult EvaluateGenome(Genome genome, int evalSeed)
    {
        var controller = CreateControllerFromGenome(genome);
        var state = CreateInitialState(evalSeed);

        while (state.IsAlive)
        {
            var direction = controller.Decide(state);
            state.Step(direction);
        }

        return new StepResult
        {
            State = state,
            StepsSurvived = state.Steps,
            FoodEaten = state.Score,
            StepsSinceLastFood = state.StepsSinceFood
        };
    }

    private GameState CreateInitialState(int seed)
    {
        return new GameState(Config, new RandomProvider(seed));
    }

    private SnakeAIController CreateControllerFromGenome(Genome genome)
    {
        var net = new NeuralNetwork(_networkShape);
        net.FromGenome(genome.Genes);
        return new SnakeAIController(net);
    }
}
