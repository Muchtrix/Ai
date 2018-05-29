using System.Collections.Generic;
using System.Drawing;

namespace Reversi.Players {
    interface IPlayer {
        Point Move(GameState state, List<Point> possibleMoves);

        Piece Color {get; set;}
    }
}