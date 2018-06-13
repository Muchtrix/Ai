using System;
using System.Collections.Generic;
using System.Drawing;
using Reversi.Players;

namespace Reversi {
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
// #if DEBUG
//             Console.Clear();
//             CurrentState.PrintBoard();
//             Console.Error.WriteLine();
//             Console.Error.WriteLine($"Czerwone: {CurrentState.BlackScore} / Białe: {CurrentState.WhiteScore}");
// #endif
            if (CurrentState.WhiteScore == CurrentState.BlackScore) return Piece.Empty;
            else if (CurrentState.WhiteScore > CurrentState.BlackScore) return Piece.White;
            else return Piece.Black;
        }

        public void PlayEqualStarts(IPlayer player1, IPlayer player2, int startsPerPlayer) {
            Console.WriteLine($"White: {player1.GetType()}, Black: {player2.GetType()}");
            for(int i = 0; i < startsPerPlayer; ++i) {
                Console.Error.Write($"Game {i + 1}...  ");
                Console.WriteLine(PlayGame(player1, player2));
            }

            Console.WriteLine($"White: {player2.GetType()}, Black: {player1.GetType()}");
            for(int i = 0; i < startsPerPlayer; ++i) {
                Console.Error.Write($"Game {i + 1 + startsPerPlayer}...  ");
                Console.WriteLine(PlayGame(player2, player1));
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