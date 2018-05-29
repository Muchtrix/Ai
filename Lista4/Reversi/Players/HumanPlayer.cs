using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Reversi.Players {
    class HumanPlayer : IPlayer
    {
        public Piece Color { get; set; }

        public Point Move(GameState state, List<Point> possibleMoves)
        {
            while (true) {
                Console.Clear();
                state.PrintBoard(possibleMoves);
                Console.Error.Write($"\nTwój ruch, {(Color == Piece.White ? "biały" : "czerwony")}: ");
                var words = Console.ReadLine();
                Point px = new Point(words[0] - 'a', words[1] - '1');
                if (possibleMoves.Contains(px)) return px;
            }
        }
    }
}