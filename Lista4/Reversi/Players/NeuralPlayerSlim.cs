using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NeuralNetworkNET.APIs;
using NeuralNetworkNET.APIs.Enums;
using NeuralNetworkNET.APIs.Interfaces;

namespace Reversi.Players {
    class NeuralPlayerSlim : IPlayer
    {
        public Piece Color { get; set; }

        private INeuralNetwork Network;

        public NeuralPlayerSlim(string NeuralFileName) {
            Network = NetworkLoader.TryLoad(new System.IO.FileInfo(NeuralFileName), ExecutionModePreference.Cpu);
        }

        public Point Move(GameState state, List<Point> possibleMoves)
        {
            var ordered = possibleMoves.Select(x => (x, state.AddPiece(x))).Select(x => (Network.Forward(PrepareBoard(x.Item2))[0], x.Item1)).OrderBy(x => x.Item1);
            return (Color == Piece.White? ordered.Last() : ordered.First()).Item2;
        }

        private float[] PrepareBoard(GameState state) {
            var res = new float[6];

            for (int x = 0; x < 8; ++x) {
                for (int y = 0; y < 8; ++y) {
                    if (state.Board[x,y] == Piece.White) ++res[0];
                    else if (state.Board[x, y] == Piece.Black) ++res[1];
                }
            }
            switch(state.Board[0,0]) {
                case Piece.White:
                    ++res[2];
                    break;
                case Piece.Black:
                    ++res[3];
                    break;
                case Piece.Empty:
                    foreach (var x in new[]{state.Board[1,0], state.Board[0,1], state.Board[1,1]}) {
                        if (x == Piece.White) ++res[4];
                        else if (x == Piece.Black) ++res[5];
                    }
                    break;
            }
            switch(state.Board[7,0]) {
                case Piece.White:
                    ++res[2];
                    break;
                case Piece.Black:
                    ++res[3];
                    break;
                case Piece.Empty:
                    foreach (var x in new[]{state.Board[6,0], state.Board[6,1], state.Board[7,1]}) {
                        if (x == Piece.White) ++res[4];
                        else if (x == Piece.Black) ++res[5];
                    }
                    break;
            }
            switch(state.Board[0,7]) {
                case Piece.White:
                    ++res[2];
                    break;
                case Piece.Black:
                    ++res[3];
                    break;
                case Piece.Empty:
                    foreach (var x in new[]{state.Board[0,6], state.Board[1,6], state.Board[1,7]}) {
                        if (x == Piece.White) ++res[4];
                        else if (x == Piece.Black) ++res[5];
                    }
                    break;
            }
            switch(state.Board[7,7]) {
                case Piece.White:
                    ++res[2];
                    break;
                case Piece.Black:
                    ++res[3];
                    break;
                case Piece.Empty:
                    foreach (var x in new[]{state.Board[6,6], state.Board[6,7], state.Board[7,6]}) {
                        if (x == Piece.White) ++res[4];
                        else if (x == Piece.Black) ++res[5];
                    }
                    break;
            }
            return res;
        }
    }
}