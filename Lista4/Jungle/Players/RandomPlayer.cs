using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Jungle.Players {
    class RandomPlayer : IPlayer
    {
        public Player Color { get; set; }
        
        public Stopwatch Timer { get; set; } = new Stopwatch();

        private Random RNG = new Random();
        public (Point, Direction) Move(GameState state, IList<(Point, Direction)> possibleMoves)
        {
            return possibleMoves[RNG.Next(possibleMoves.Count)];
        }
    }
}