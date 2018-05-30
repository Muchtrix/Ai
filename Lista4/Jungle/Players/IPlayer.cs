using System.Collections.Generic;
using System.Diagnostics;

namespace Jungle.Players {
    interface IPlayer {

        Player Color {get; set;}

        Stopwatch Timer {get; set;}

        (Point, Direction) Move(GameState state, IList<(Point, Direction)> possibleMoves);
    }
}