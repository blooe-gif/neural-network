# SnakeAI

SnakeAI is a .NET 8 WinForms application that simulates Snake with a small neural network trained via a minimal genetic algorithm.

## Build

```bash
dotnet build SnakeAI/SnakeAI.csproj
```

## Run

```bash
dotnet run --project SnakeAI/SnakeAI.csproj
```

## Modes

- **Training**: click **Start Training** to evolve the population in the background while the best genome is replayed in the game view.
- **Replay/Pause**: click **Stop** to pause training and continue observing the current best snake in real time.
- **Reset**: click **Reset** to reinitialize simulation and population from the configured seed.
