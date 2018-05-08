using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;

namespace Zadanie1
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch st = Stopwatch.StartNew();
            IPlayer randomBot = new RandomPlayer();
            GameMaster GM = new GameMaster();
            Console.Error.WriteLine($"{(GM.PlayGame(randomBot, randomBot) == Piece.White ? "White" : "Black")} won!");
            Console.Error.WriteLine($"Ellapsed: {st.Elapsed}");
        }
    }
}
