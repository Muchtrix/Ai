using System;
using System.Collections.Generic;
using System.Drawing;

namespace Zadanie1 {
    class RandomPlayer : IPlayer
    {
        private Random RNG = new Random();

        public Piece Color { get; set; }

        public Point Move(GameState state, List<Point> possibleMoves)
        {
            return possibleMoves[RNG.Next(possibleMoves.Count)];
        }
    }
}