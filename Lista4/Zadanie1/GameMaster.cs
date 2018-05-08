using System;
using System.Collections.Generic;
using System.Drawing;

namespace Zadanie1 {
    class GameMaster {

        private static Random RNG = new Random();
        public Piece PlayGame(IPlayer whitePlayer, IPlayer blackPlayer) {
            whitePlayer.Color = Piece.White;
            blackPlayer.Color = Piece.Black;
            GameState currentState = GameState.StartState();
            bool noMoreMoves = false;
            while (true) {
                if (currentState.WhiteScore == 0 && currentState.BlackScore == 0) break;
                List<Point> possibleMoves = currentState.PossibleMoves();
                if (possibleMoves.Count == 0) {
                    if (noMoreMoves) break;
                    else {
                        currentState.CurrentPlayer = 3 - currentState.CurrentPlayer;
                        noMoreMoves = true;
                        continue;
                    }
                }
                noMoreMoves = false;

                currentState = currentState.AddPiece(
                    currentState.CurrentPlayer == Piece.White ? 
                        whitePlayer.Move(currentState, possibleMoves) :
                        blackPlayer.Move(currentState, possibleMoves));
            }
#if DEBUG
            currentState.PrintBoard();
#endif
            if (currentState.WhiteScore == currentState.BlackScore) return Piece.Empty;
            else if (currentState.WhiteScore > currentState.BlackScore) return Piece.White;
            else return Piece.Black;
        }

        public int PlayManyGames(IPlayer player, IPlayer opponent, int gameCount) {
            int winCount = 0;
            for (int i = 0; i < gameCount; ++i) {
                if (RNG.Next(2) == 1) {
                    if (PlayGame(player, opponent) == Piece.White) ++winCount;
                } else {
                    if (PlayGame(opponent, player) == Piece.Black) ++ winCount;
                }
            }

            return winCount;
        }
    }
}