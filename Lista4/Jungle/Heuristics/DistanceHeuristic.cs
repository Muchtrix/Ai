using System.Linq;

namespace Jungle.Heuristics {
    class DistanceHeuristic : IHeuristic
    {

        static Point UpCave = new Point{X = 3, Y = 0};
        static Point DownCave = new Point{X = 3, Y = 8};

        const int MaxDist = 11;
        
        public double Evaluate(GameState state, Player player)
        {   
            if (state.UpPlayer.ContainsKey(DownCave)) return player == Player.Up ? double.PositiveInfinity : double.NegativeInfinity;
            if (state.DownPlayer.ContainsKey(UpCave)) return player == Player.Down ? double.PositiveInfinity : double.NegativeInfinity;
            int upScore = state.UpPlayer.Select(x => (MaxDist - DownCave.ManhattanDistance(x.Key)) * ((int)x.Value + 1)).Sum();
            int downScore = state.DownPlayer.Select(x => (MaxDist - UpCave.ManhattanDistance(x.Key)) * ((int)x.Value + 1)).Sum();
            return player == Player.Up ? upScore - downScore : downScore - upScore;
        }
    }
}