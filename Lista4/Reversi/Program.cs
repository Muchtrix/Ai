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
            // uczenie sieci opartej na wyliczonych cechach
            // SlimNN nn = new SlimNN();
            // nn.Learn("./smaller.dat");
            // return ;

            // uczenie sieci opartej na pełnej sytuacji na planszy
            // FullBoardNN nn = new FullBoardNN();
            // nn.Learn("./smaller.dat");
            // return ;


            IPlayer randomBot = new RandomPlayer();
            IPlayer minMaxBot = new MinMaxPlayer(new PositionalHeuristic(), 5);
            IPlayer mctsBot = new MonteCarloPlayer();
            IPlayer neuralBot = new NeuralPlayer("./LastNetwork");
            IPlayer neuralSlimBot = new NeuralPlayerSlim("./LastNetworkSlim");
            IPlayer humanBot = new HumanPlayer();
            GameMaster GM = new GameMaster();

            Stopwatch st = Stopwatch.StartNew();
            GM.PlayEqualStarts(mctsBot, minMaxBot, 5);
            GM.PlayGame(mctsBot, minMaxBot);
            //GM.PlayManyGames(neuralBot, minMaxBot, 1000, 10);
            //Console.Error.WriteLine($"{(GM.PlayGame(humanBot, new HumanPlayer()) == Piece.White ? "Białe" : "Czerwone")} wygrały!");
            Console.Error.WriteLine($"Czas: {st.Elapsed}");
        }
    }
}
