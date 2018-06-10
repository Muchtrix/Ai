using System.Collections.Generic;
using System.Drawing;

namespace Reversi.Players {
    class MonteCarloPlayer : IPlayer
    {
        public Piece Color { get; set; }

        public Point Move(GameState state, List<Point> possibleMoves)
        {
            throw new System.NotImplementedException();
        }
    }
}