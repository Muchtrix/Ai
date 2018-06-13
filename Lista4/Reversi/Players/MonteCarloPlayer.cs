using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Reversi.Heuristics;

namespace Reversi.Players {
    class MonteCarloPlayer : IPlayer
    {
        public Piece Color { get; set; }

        private Stopwatch st = new Stopwatch();

        private class MCTSNode {

            private static MinMaxPlayer MinMaxer = new MinMaxPlayer(new PositionalHeuristic(), 0);
            private static RandomPlayer RngPlayer = new RandomPlayer();
            private static GameMaster GM = new GameMaster();
            public GameState State;

            public Point LastMove;

            double Wins;
            double Simulations;

            public List<Point> NotYetVisited;

            public List<MCTSNode> Children = new List<MCTSNode>();

            public int VisitNode(Piece color) {
                Simulations++;

                if (NotYetVisited.Any()) {
                    var ns = NotYetVisited[NotYetVisited.Count - 1];
                    NotYetVisited.RemoveAt(NotYetVisited.Count - 1);

                    MCTSNode newNode = new MCTSNode();
                    newNode.State = State.AddPiece(ns);
                    newNode.LastMove = ns;
                    newNode.NotYetVisited = newNode.State.PossibleMoves();
                    Children.Add(newNode);
                    int wasSuccesfull = newNode.Simulate(color);
                    Wins += wasSuccesfull;
                    return wasSuccesfull;
                }
                else {
                    if (!Children.Any()) return 1;
                    MCTSNode node = Children.OrderByDescending(x => UCB1(x)).First();
                    int wasSuccesfull = node.VisitNode(color);
                    Wins += wasSuccesfull;
                    return wasSuccesfull;
                }
            }

            private int Simulate(Piece color)
            {
                var res = color == Piece.White ? GM.PlayGame(MinMaxer, RngPlayer) : GM.PlayGame(RngPlayer, MinMaxer);
                int r = res == color ? 1 : 0;
                Simulations = 1;
                Wins = r;
                return r;
            }

            public Point BestMove() => Children.OrderByDescending(x => x.Simulations).First().LastMove;

            private double UCB1(MCTSNode child) => (child.Wins / child.Simulations) + 1.4142 * Math.Sqrt(Math.Log(Simulations) / child.Simulations);
        }

        public Point Move(GameState state, List<Point> possibleMoves)
        {
            var root = new MCTSNode {
                State = state,
                NotYetVisited = new List<Point>(possibleMoves)
            };

            st.Start();
            while (st.ElapsedMilliseconds < 500) {
                root.VisitNode(Color);
            }
            st.Reset();

            return root.BestMove();
        }
    }
}