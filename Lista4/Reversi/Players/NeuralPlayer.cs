using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using NeuralNetworkNET.APIs;
using NeuralNetworkNET.APIs.Enums;
using NeuralNetworkNET.APIs.Interfaces;

namespace Reversi.Players {
    class NeuralPlayer : IPlayer
    {
        public Piece Color { get; set; }

        private INeuralNetwork Network;

        public NeuralPlayer(string NeuralFileName) {
            Network = NetworkLoader.TryLoad(new System.IO.FileInfo(NeuralFileName), ExecutionModePreference.Cpu);
        }

        public Point Move(GameState state, List<Point> possibleMoves)
        {
            var ordered = possibleMoves.Select(x => (x, state.AddPiece(x))).Select(x => (Network.Forward(PrepareBoard(x.Item2))[0], x.Item1)).OrderBy(x => x.Item1);
            return (Color == Piece.White? ordered.Last() : ordered.First()).Item2;
        }

        private float[] PrepareBoard(GameState state) {
            var res = new float[64];
            for (int x = 0; x < 8; ++x) {
                for (int y = 0; y < 8; ++y) {
                    res[y*8 + x] = (float) (state.Board[x,y]);
                }
            }
            return res;
        }
    }
}