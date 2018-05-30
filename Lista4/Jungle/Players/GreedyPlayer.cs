using System.Diagnostics;
using System.Linq;
using Jungle.Heuristics;

namespace Jungle.Players {
    class GreedyPlayer : IPlayer
    {
        public Player Color { get; set; }

        public Stopwatch Timer { get; set; } = new Stopwatch();

        private IHeuristic Heuristic = new DistanceHeuristic();

        public (Point, Direction) Move(GameState state, System.Collections.Generic.IList<(Point, Direction)> possibleMoves)
        {
            Timer.Start();
            var a = possibleMoves.Select(mv => (mv, Heuristic.Evaluate(state.ApplyMove(mv), Color))).OrderByDescending(x => x.Item2).First().Item1;
            Timer.Stop();
            return a;
        }
    }
}