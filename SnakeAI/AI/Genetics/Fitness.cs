using SnakeAI.Simulation;

namespace SnakeAI.AI.Genetics;

public static class Fitness
{
    public static double Evaluate(StepResult result)
    {
        // Reward food strongly, then sustained survival.
        // Penalize starvation-like behavior to reduce wall-hugging and indecisive loops.
        var foodReward = result.FoodEaten * 1200;
        var survivalReward = result.StepsSurvived * 1.5;
        var stallingPenalty = result.StepsSinceLastFood * 1.2;

        return foodReward + survivalReward - stallingPenalty;
    }
}
