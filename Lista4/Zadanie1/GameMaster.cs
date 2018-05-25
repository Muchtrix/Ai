using System;
using System.Collections.Generic;
using System.Drawing;

namespace Zadanie1 {
    class GameMaster {

        private static Random RNG = new Random();

        private List<(Piece, Point)> History = new List<(Piece, Point)>();

        private GameState CurrentState;
        public Piece PlayGame(IPlayer whitePlayer, IPlayer blackPlayer) {
            whitePlayer.Color = Piece.White;
            blackPlayer.Color = Piece.Black;
            History.Clear();
            CurrentState = GameState.StartState();
            bool noMoreMoves = false;
            while (true) {
                if (CurrentState.WhiteScore + CurrentState.BlackScore == 64) break;
                List<Point> possibleMoves = CurrentState.PossibleMoves();
                if (possibleMoves.Count == 0) {
                    if (noMoreMoves) break;
                    else {
                        CurrentState.CurrentPlayer = 3 - CurrentState.CurrentPlayer;
                        noMoreMoves = true;
                        continue;
                    }
                }
                noMoreMoves = false;

                Point mv = CurrentState.CurrentPlayer == Piece.White ? whitePlayer.Move(CurrentState, possibleMoves) : blackPlayer.Move(CurrentState, possibleMoves);
                History.Add((CurrentState.CurrentPlayer, mv));

                CurrentState = CurrentState.AddPiece(mv);
            }
#if DEBUG
            Console.Clear();
            CurrentState.PrintBoard();
            Console.Error.WriteLine();
            Console.Error.WriteLine($"Czerwone: {CurrentState.BlackScore} / Białe: {CurrentState.WhiteScore}");
#endif
            if (CurrentState.WhiteScore == CurrentState.BlackScore) return Piece.Empty;
            else if (CurrentState.WhiteScore > CurrentState.BlackScore) return Piece.White;
            else return Piece.Black;
        }

        public void Map(int turns) {
            RandomPlayer rand = new RandomPlayer();
            MinMaxPlayer pl = new MinMaxPlayer();
            for (int i = 0; i < turns; ++i) {
                if (i % 100 == 0) Console.Error.WriteLine(i);
                Piece winner = (RNG.Next(2) == 0)? PlayGame(pl, rand) : PlayGame(rand, pl);
                double delta = (winner == pl.Color) ? 0.1: -0.1;
                foreach(var h in History) {
                    MinMaxPlayer.PositionScore[h.Item2.X, h.Item2.Y] += delta * ((h.Item1 == pl.Color)?1:-1);
                }
            } 

            for(int y = 0; y < 8; y++) {
                for (int x = 0; x < 8; x++) {
                    Console.Error.Write("{0},\t", MinMaxPlayer.PositionScore[x,y]);
                }
                Console.Error.WriteLine();
            }
        }

        public int PlayManyGames(IPlayer player, IPlayer opponent, int gameCount, int logFrequency = 100) {
            int loseCount = 0;
            for (int i = 0; i < gameCount; ++i) {
                if (i % logFrequency == 0) Console.Error.WriteLine($"Progress: {i}/{gameCount}");
                if (RNG.Next(2) == 1) {
                    if (PlayGame(player, opponent) == Piece.Black) ++loseCount;
                } else {
                    if (PlayGame(opponent, player) == Piece.White) ++loseCount;
                }
            }
            Console.Error.WriteLine($"Winning ratio: {gameCount - loseCount}/{gameCount}");
            return loseCount;
        }
    }
}