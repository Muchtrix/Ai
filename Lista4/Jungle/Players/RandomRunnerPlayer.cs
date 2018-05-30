using System.Diagnostics;

namespace Jungle.Players {
    class RandomRunnerPlayer : IPlayer
    {
        public Player Color { get; set; }
        public Stopwatch Timer { get; set; } = new Stopwatch();

        private RandomPlayer Rand = new RandomPlayer();

        private GameMaster GM = new GameMaster();

        private const int AvailableMoves = 20_000;

        public (Point, Direction) Move(GameState state, System.Collections.Generic.IList<(Point, Direction)> possibleMoves)
        {
            Timer.Start();
            int perPossibility = AvailableMoves / possibleMoves.Count;
            int bestIdx = 0;
            double bestPercentage = 0;
            for (int i = 0; i < possibleMoves.Count; ++i) {
                int remaining = perPossibility;
                GameState n = state.ApplyMove(possibleMoves[i]);
                int won = 0, lost = 0;
                while (remaining > 0) {
                    if (GM.Play(Rand, Rand, n, remaining) == Color) {
                        won++;
                    } else {
                        lost++;
                    }
                    remaining -= GM.RoundCounter + 1;
                }
                if (won+lost > 0 && (won*1.0 / (won+lost) > bestPercentage)) {
                    bestPercentage = won*1.0 / (won+lost);
                    bestIdx = i;
                }
            }
            Timer.Stop();
            return possibleMoves[bestIdx];
        }
    }
}