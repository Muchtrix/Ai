using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Zadanie1 {
    class MinMaxPlayer : IPlayer
    {
        private static Random RNG = new Random();

        public static double[,] PositionScore = new double[8,8]{
            {100, -20,  5, 15, 15,  5, -20, 100},
            {-20, -40, -2, -1, -1, -2, -40, -20},
            {  5,  -2,  0,  1,  1,  0,  -2,   5},
            { 15,  -1,  1,  2,  2,  1,  -1,  15},
            { 15,  -1,  1,  2,  2,  1,  -1,  15},
            {  5,  -2,  0,  1,  1,  0,  -2,   5},
            {-20, -40, -2, -1, -1, -2, -40, -20},
            {100, -20,  5, 15, 15,  5, -20, 100}
            // {94.9999999999989,       -28.8000000000001,      28.7000000000002,       52.8000000000005,       3.30000000000002,       65.8000000000006,       -64.7000000000006,      94.5999999999989},
            // {-57.2000000000005,      -60.2000000000006,      -54.6000000000005,      -6.79999999999999,      -53.3000000000005,      -47.9000000000004,      -61.0000000000006,      -52.8000000000005},
            // {52.7000000000005,       27.8000000000001,       5.2,    28.2000000000001,       -15,    62.6000000000006,       14.7,   60.1000000000006},
            // {32.3000000000002,       -23.3000000000001,      -45.8000000000004,      1,      1,      5,      -40.4000000000003,      35.8000000000003},
            // {27.6000000000001,       -31.5000000000002,      -46.1000000000004,      1,      1,      -36.3000000000002,      -13.1,  11.3},
            // {50.4000000000005,       9.29999999999998,       35.2000000000002,       44.0000000000004,       -17.6,  63.5000000000006,       -13.4,  64.3000000000006},
            // {-62.6000000000006,      -53.5000000000005,      -54.9000000000005,      -13.2,  -33.6000000000002,      -49.4000000000004,      -57.9000000000006,      -61.6000000000006},
            // {92.099999999999,        -30.8000000000002,      43.7000000000004,       49.6000000000005,       7.80000000000001,       66.4000000000005,       -56.6000000000005,      88.8999999999992}
        };

        public Piece Color { get; set; }

        public Point Move(GameState state, List<Point> possibleMoves)
        {
            var evaluated = possibleMoves.OrderByDescending(p => AlphaBeta(state.AddPiece(p), 3, true, double.NegativeInfinity, double.PositiveInfinity));
            return evaluated.First();
        }

        private double AlphaBeta(GameState state, int depth, bool min, double alpha, double beta) {
            if (depth == 0) return HeuristicScore(state);
            if (state.WhiteScore + state.BlackScore == 64) {
                if (Color == Piece.White && state.WhiteScore > state.BlackScore ||
                    Color == Piece.Black && state.BlackScore > state.WhiteScore) return double.PositiveInfinity;
                else return double.NegativeInfinity;
            }
            var possibleMoves = state.PossibleMoves();
            if (possibleMoves.Count == 0) return HeuristicScore(state);
            if (min) {
                foreach(var p in possibleMoves) {
                    beta = Math.Min(beta, AlphaBeta(state.AddPiece(p), depth-1, !min, alpha, beta));
                    if (alpha >= beta) break;
                }
                return beta;
            } else {
                foreach(var p in possibleMoves) {
                    alpha = Math.Max(alpha, AlphaBeta(state.AddPiece(p), depth-1, !min, alpha, beta));
                    if (alpha >= beta) break;
                }
                return alpha;
            }
        }

        public double HeuristicScore(GameState state) {
            double whiteScore = 0;
            double blackScore = 0;
            for (int x = 0; x < 8; ++x) {
                for (int y = 0; y < 8; ++y) {
                    if (state.Board[x,y] == Piece.Black) blackScore += PositionScore[x,y];
                    else if (state.Board[x,y] == Piece.White) whiteScore += PositionScore[x,y];
                }
            }

            return Color == Piece.White ? whiteScore - blackScore : blackScore - whiteScore;
        }
    }
}