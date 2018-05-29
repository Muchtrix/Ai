using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Reversi.Heuristics;

namespace Reversi.Players {
    class MinMaxPlayer : IPlayer
    {
        private IHeuristic Heuristic;

        public MinMaxPlayer(IHeuristic heuristic) {
            Heuristic = heuristic;
        }
        
        public Piece Color { get; set; }

        public Point Move(GameState state, List<Point> possibleMoves)
        {
            var evaluated = possibleMoves.OrderByDescending(p => AlphaBeta(state.AddPiece(p), 3, true, double.NegativeInfinity, double.PositiveInfinity));
            return evaluated.First();
        }

        private double AlphaBeta(GameState state, int depth, bool min, double alpha, double beta) {
            if (depth == 0) return Heuristic.EvaluateBoard(state, this.Color);
            if (state.WhiteScore + state.BlackScore == 64) {
                if (Color == Piece.White && state.WhiteScore > state.BlackScore ||
                    Color == Piece.Black && state.BlackScore > state.WhiteScore) return double.PositiveInfinity;
                else return double.NegativeInfinity;
            }
            var possibleMoves = state.PossibleMoves();
            if (possibleMoves.Count == 0) return Heuristic.EvaluateBoard(state, this.Color);
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
    }
}