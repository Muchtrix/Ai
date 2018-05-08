using System.Collections.Generic;
using System.Drawing;

namespace Zadanie1 {
    interface IPlayer {
        Point Move(GameState state, List<Point> possibleMoves);

        Piece Color {get; set;}
    }
}