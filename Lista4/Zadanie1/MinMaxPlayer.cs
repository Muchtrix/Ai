using System;
using System.Collections.Generic;
using System.Drawing;

namespace Zadanie1 {
    class MinMaxPlayer : IPlayer
    {
        private static double[,] PositionScore = new double[8,8];

        public Piece Color { get; set; }

        public Point Move(GameState state, List<Point> possibleMoves)
        {
            throw new NotImplementedException();
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