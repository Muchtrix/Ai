using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Reversi.Heuristics;
using Reversi.NeuralNetworks;
using Reversi.Players;

namespace Reversi
{
    class Program
    {
        static void Main(string[] args)
        {
            SlimNN nn = new SlimNN();
            nn.Learn("./smaller.dat");
            return ;


            // FullBoardNN nn = new FullBoardNN();
            // nn.Learn("./smaller.dat");
            // return ;


            IPlayer randomBot = new RandomPlayer();
            IPlayer minMaxBot = new MinMaxPlayer(new PositionalHeuristic());
            IPlayer neuralBot = new NeuralPlayer("./LastNetwork");
            IPlayer neuralSlimBot = new NeuralPlayerSlim("./LastNetworkSlim");
            IPlayer humanBot = new HumanPlayer();
            GameMaster GM = new GameMaster();

            Stopwatch st = Stopwatch.StartNew();
            //GM.Map(1000);
            GM.PlayManyGames(neuralBot, minMaxBot, 1000, 10);
            //Console.Error.WriteLine($"{(GM.PlayGame(humanBot, new HumanPlayer()) == Piece.White ? "Białe" : "Czerwone")} wygrały!");
            Console.Error.WriteLine($"Czas: {st.Elapsed}");
        }
    }
}
