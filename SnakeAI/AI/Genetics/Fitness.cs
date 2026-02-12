using SnakeAI.Simulation;

namespace SnakeAI.AI.Genetics;

public static class Fitness
{
    public static double Evaluate(StepResult result)
    {
        // Reward food heavily, then survival length. Small penalty for stalling.
        return result.FoodEaten * 1000 + result.StepsSurvived * 2 - result.StepsSinceLastFood;
    }
}
