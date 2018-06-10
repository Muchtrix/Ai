using System;
using System.IO;
using System.Linq;
using NeuralNetworkNET.APIs;
using NeuralNetworkNET.APIs.Enums;
using NeuralNetworkNET.APIs.Interfaces;
using NeuralNetworkNET.APIs.Structs;

namespace Reversi.NeuralNetworks {
    class FullBoardNN {
        public void Learn(string fileName) {

            var input = File.ReadAllLines(fileName).Select(x => x.Split()).Select(x => (ParseBoard(x[1]), ParseWinner(x[0]))).ToArray();

            INeuralNetwork network = NetworkManager.NewSequential(TensorInfo.Linear(64),
                //NetworkLayers.FullyConnected(100, ActivationType.ReLU),
                NetworkLayers.FullyConnected(64, ActivationType.ReLU),
                NetworkLayers.FullyConnected(10, ActivationType.ReLU),
                NetworkLayers.Softmax(2));

            var dataset = DatasetLoader.Training(input, 32);

            var result = NetworkManager.TrainNetwork(
                network,
                dataset,
                TrainingAlgorithms.Adam(),
                100,
                trainingCallback: x => {
                    Console.Error.WriteLine($"Iteration: {x.Iteration}, Accuracy: {x.Result.Accuracy}");
                }
            );
            network.Save(new FileInfo("./LastNetwork"));
            //Console.WriteLine(network.Forward(input[0].Item1));
            foreach(var e in network.Forward(input[0].Item1)) {
                Console.Write($"{e} ; ");
            }
        }

        static float[] ParseBoard(string board) {
            var res = new float[64];
            for (int i = 0; i < 64; i++) {
                if (board[i] == '0') res[i] = (float)Piece.White;
                else if (board[i] == '1') res[i] = (float)Piece.Black;
                else res[i] = (float)Piece.Empty;
            }
            return res;
        }

        static float[] ParseWinner(string winner) {
            if (winner == "-1") return new[]{1f, 0f};
            else return new[]{0f, 1f};
        }
    }
}