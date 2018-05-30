using System;
using System.Collections.Generic;
using System.Linq;
using Jungle.Heuristics;
using Jungle.Players;

namespace Jungle {
    class GameMaster {
        private GameState CurrentState;

        private IHeuristic Heuristic = new DistanceHeuristic();

        private static Point UpCave = new Point{X = 3, Y = 0};

        private static Point DownCave = new Point{X = 3, Y = 8};

        public int RoundCounter;

        public Player Play(IPlayer up, IPlayer down, GameState startingState = null, int? roundLimit = null) {
            up.Color = Player.Up;
            down.Color = Player.Down;
            CurrentState = startingState ?? GameState.InitialState();
            RoundCounter = 0;
            while(true) {
                if (roundLimit.HasValue && RoundCounter >= roundLimit.Value) {
                    return Heuristic.Evaluate(CurrentState, Player.Up) > 0 ? Player.Up : Player.Down;
                }
                if (CurrentState.UpPlayer.ContainsKey(DownCave)) return Player.Up;
                if (CurrentState.DownPlayer.ContainsKey(UpCave)) return Player.Down;
                if (CurrentState.HitCounter >= 50) {
                    if (!roundLimit.HasValue)Console.WriteLine($"TimeOut");
                    var ranks = CompareList(CurrentState.UpPlayer.Values.Cast<int>().OrderByDescending(x => x).ToList(), CurrentState.DownPlayer.Values.Cast<int>().OrderByDescending(x => x).ToList());
                    if (ranks.HasValue) return ranks.Value;
                    var dists = CompareList(
                        CurrentState.UpPlayer.Keys.Select(x => (CurrentState.UpPlayer[x], DownCave.ManhattanDistance(x))).OrderBy(x => x.Item1).Select(x => x.Item2).ToList(), 
                        CurrentState.DownPlayer.Keys.Select(x => (CurrentState.DownPlayer[x], UpCave.ManhattanDistance(x))).OrderBy(x => x.Item1).Select(x => x.Item2).ToList());
                    if (dists.HasValue) return dists.Value;
                    return Player.Down;
                }
                var possibleMoves = CurrentState.PossibleMoves();
                if (!possibleMoves.Any()) return CurrentState.CurrentPlayer == Player.Down ? Player.Up : Player.Down;

                var mv = CurrentState.CurrentPlayer == Player.Up ? up.Move(CurrentState, possibleMoves) : down.Move(CurrentState, possibleMoves);
                CurrentState = CurrentState.ApplyMove(mv);
                RoundCounter++;
            }
        }

        private Player? CompareList(IList<int> up, IList<int> down){
            int m = Math.Min(up.Count, down.Count);
            for (int i = 0; i < m; i++) {
                if (up[i].CompareTo(down[i]) < 0) return Player.Down;
                else if (up[i].CompareTo(down[i]) > 0) return Player.Up;
            }
            if (up.Count < down.Count) return Player.Down;
            if (up.Count > down.Count) return Player.Up;
            return null;
        }
    }
}