using System;
using Jungle.Players;

namespace Jungle
{
    class Program
    {
        static Random RNG = new Random();
        static void Main(string[] args)
        {
            IPlayer runner = new RandomRunnerPlayer();
            IPlayer greedy = new GreedyPlayer();
            GameMaster GM = new GameMaster();
            for (int i = 1; i <= 10; ++i) {
                Console.Write($"Game {i}: ");
                if (RNG.Next(2) == 0) {
                    Console.WriteLine($"{(GM.Play(runner, greedy) == Player.Up ? "runner" : "greedy")}");
                } else {
                    Console.WriteLine($"{(GM.Play(greedy, runner) == Player.Down ? "runner" : "greedy")}");
                }
            }
            Console.WriteLine($"Runner time: {runner.Timer.Elapsed}");
            Console.WriteLine($"Greedy time: {greedy.Timer.Elapsed}");
        }
    }
}
