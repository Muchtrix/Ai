using System;
using System.IO;
using System.Linq;
using NeuralNetworkNET.APIs;
using NeuralNetworkNET.APIs.Enums;
using NeuralNetworkNET.APIs.Interfaces;
using NeuralNetworkNET.APIs.Structs;

namespace Reversi.NeuralNetworks {
    class SlimNN {
        public void Learn(string fileName) {

            var input = File.ReadAllLines(fileName).Select(x => x.Split()).Select(x => (ParseBoard(x[1]), ParseWinner(x[0]))).ToArray();

            INeuralNetwork network = NetworkManager.NewSequential(TensorInfo.Linear(6),
                //NetworkLayers.FullyConnected(100, ActivationType.ReLU),
                NetworkLayers.FullyConnected(20, ActivationType.ReLU),
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
            network.Save(new FileInfo("./LastNetworkSlim"));
        }

        static float[] ParseBoard(string board) {
            var res = new float[6];
            res[0] = board.Count(x => x == '0');
            res[1] = board.Count(x => x == '1');
            switch (board[0]) {
                case '0':
                    res[2] +=1;
                    break;
                case '1':
                    res[3] +=1;
                    break;
                default:
                    foreach (char x in new[]{board[1], board[8], board[9]}) {
                        if (x == '0') res[4] += 1;
                        else if (x == '1') res[5] += 1;
                    }
                break;
            }

            switch (board[7]) {
                case '0':
                    res[2] +=1;
                    break;
                case '1':
                    res[3] +=1;
                    break;
                default:
                    foreach (char x in new[]{board[6], board[14], board[15]}) {
                        if (x == '0') res[4] += 1;
                        else if (x == '1') res[5] += 1;
                    }
                break;
            }

            switch (board[56]) {
                case '0':
                    res[2] +=1;
                    break;
                case '1':
                    res[3] +=1;
                    break;
                default:
                    foreach (char x in new[]{board[48], board[49], board[57]}) {
                        if (x == '0') res[4] += 1;
                        else if (x == '1') res[5] += 1;
                    }
                break;
            }

            switch (board[63]) {
                case '0':
                    res[2] +=1;
                    break;
                case '1':
                    res[3] +=1;
                    break;
                default:
                    foreach (char x in new[]{board[62], board[54], board[55]}) {
                        if (x == '0') res[4] += 1;
                        else if (x == '1') res[5] += 1;
                    }
                break;
            }
            return res;
        }

        static float[] ParseWinner(string winner) {
            if (winner == "-1") return new[]{1f, 0f};
            else return new[]{0f, 1f};
        }
    }
}