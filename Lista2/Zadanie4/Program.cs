using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using Priority_Queue;

namespace Zadanie4
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1) {
                Console.Error.WriteLine($"Run with one of following: --bfs, --astar");
                return;
            }
            using (StreamWriter sr = new StreamWriter("./zad_output.txt"))
                TrooperSolver.Solve(sr, args[0]);
        }
    }

    static class TrooperSolver {
        public static char[,] Walls;

        static HashSet<Point> Destinations = new HashSet<Point>();

        static Direction[] Directions = new[] {Direction.U, Direction.D, Direction.L, Direction.R};
        static Random RNG = new Random();
        const int RANDOM_COUNT = 40;

        static List<Direction> MovePrefix = new List<Direction>();

        const int INCORRECTNESS_MOD = 2;

        static int[,] Heurs;

        public static void Solve(TextWriter tw, string mode) {
            var lines = File.ReadAllLines("./zad_input.txt");
            TrooperState startingState = new TrooperState() {
                Possibilities = new HashSet<Point>(PointComparer.Instance)
            };

            Walls = new char[lines[0].Length, lines.Length];
            for(int i = 0; i < lines.Length; ++i) {
                for(int j = 0; j < lines[0].Length; ++j) {
                    switch(lines[i][j]) {
                        case '#':
                            Walls[j,i] = '#';
                            break;
                        case 'S':
                            startingState.Possibilities.Add(new Point(j,i));
                            break;
                        case 'G':
                            Walls[j,i] = 'G';
                            Destinations.Add(new Point(j,i));
                            break;
                        case 'B':
                            startingState.Possibilities.Add(new Point(j,i));
                            Walls[j,i] = 'G';
                            Destinations.Add(new Point(j,i));
                            break;
                    }
                }
            }

            switch(mode) {
                case "--bfs":
                    int dl = Math.Max(lines[0].Length , lines.Length);
                    List<Direction> PrePrefix = new List<Direction>();

                    for(int i = 0; i < dl; ++i){
                        startingState = MoveState(startingState, Direction.L);
                        startingState = MoveState(startingState, Direction.D);
                        PrePrefix.Add(Direction.L);
                        PrePrefix.Add(Direction.D);
                    }
                    for(int i = 0; i < dl; ++i){
                        startingState = MoveState(startingState, Direction.U);
                        startingState = MoveState(startingState, Direction.R);
                        PrePrefix.Add(Direction.U);
                        PrePrefix.Add(Direction.R);
                    }

                    // for(int i = 0; i < dl/2; ++i){
                    //     startingState = MoveState(startingState, Direction.D);
                    //     startingState = MoveState(startingState, Direction.R);
                    //     PrePrefix.Add(Direction.D);
                    //     PrePrefix.Add(Direction.R);
                    // }

                    // for (int i = 0; i < 50; ++i) {
                    //     int count = startingState.Possibilities.Count;
                    //     Direction bestDir = Direction.U;
                    //     foreach(Direction dir in Directions) {
                    //         int c = MoveState(startingState, dir).Possibilities.Count;
                    //         if (c < count) {
                    //             bestDir = dir;
                    //             count = c;
                    //         }
                    //     }
                    //     startingState = MoveState(startingState, bestDir);
                    //     MovePrefix.Add(bestDir);
                    // }

                    // for(int i = 0; i < dl; ++i){
                    //     startingState = MoveState(startingState, Direction.U);
                    //     startingState = MoveState(startingState, Direction.R);
                    //     PrePrefix.Add(Direction.U);
                    //     PrePrefix.Add(Direction.R);
                    // }

                    // List<Direction> bestPref = new List<Direction>();
                    // int bestLen = int.MaxValue;
                    // int bestCount = startingState.Possibilities.Count;
                    // TrooperState bestState = startingState;
                    // for (int i = 0; i < 2000; ++i) {
                    //     var current = startingState;
                    //     MovePrefix.Clear();
                    //     for(int j = 0; j < 50; ++j) {
                    //         current = RandomMove(startingState);
                    //         if (current.Possibilities.Count <= 2) break;
                    //     }
                    //     if (current.Possibilities.Count <= bestCount && MovePrefix.Count <= bestLen) {
                    //         bestLen = MovePrefix.Count;
                    //         bestCount = current.Possibilities.Count;
                    //         bestPref = new List<Direction>(MovePrefix);
                    //         bestState = current;
                    //     }
                    // }
                    // MovePrefix = bestPref;
                    // startingState = bestState;



                    // while(startingState.Possibilities.Count > 2) {
                    //     mv++;
                    //     startingState = RandomMove(startingState);
                    // }

                    Console.Error.WriteLine($"After beginning {PrePrefix.Count + MovePrefix.Count} moves: {startingState.Possibilities.Count}");

                    var moves = RunBFS(startingState);
                    tw.Write(string.Concat(PrePrefix));
                    tw.Write(string.Concat(MovePrefix));
                    tw.WriteLine(string.Concat(moves));
                    break;
                case "--astar":
                    Precalc();
                    var movesStar = RunAStar(startingState);
                    tw.WriteLine(string.Concat(movesStar));
                    break;
            }
        }

        static void Precalc() {
            Heurs = new int[Walls.GetLength(0), Walls.GetLength(1)];
            for(int i = 0; i < Heurs.GetLength(0); ++i)
                for(int j = 0; j < Heurs.GetLength(1); ++j)
                    Heurs[i,j] = -1;

            Queue<(Point p, int dist)> q = new Queue<(Point, int)>();
            foreach(Point p in Destinations)  {
                q.Enqueue((p, 0));
                Heurs[p.X, p.Y] = 0;
            }
            while(q.Any()) {
                var current = q.Dequeue();
                foreach (var dir in Directions) {
                    var nw = current.p.Move(dir);
                    if (Heurs[nw.X, nw.Y] == -1 && Walls[nw.X, nw.Y] != '#') {
                        Heurs[nw.X, nw.Y] = current.dist + 1;
                        q.Enqueue((nw, current.dist + 1));
                    }
                }
            }
        }

        static bool WinCondition(TrooperState st) => st.Possibilities.All(x => Walls[x.X, x.Y] == 'G');

        static TrooperState RandomMove(TrooperState state) {
            Direction dir = Directions[RNG.Next(Directions.Length)];
            MovePrefix.Add(dir);
            return MoveState(state, dir);
        }

        static Point MoveOrStay(Point p, Direction Dir) {
            Point n = p.Move(Dir);
            return Walls[n.X, n.Y] == '#' ? p : n;
        }

        static TrooperState MoveState(TrooperState state, Direction dir) 
            => new TrooperState {Possibilities = state.Possibilities.Select(x => MoveOrStay(x, dir)).ToHashSet(PointComparer.Instance)};

        static List<Direction> RunBFS(TrooperState startingState) {
            Dictionary<TrooperState, (TrooperState Prev, Direction Dir)> previous = new Dictionary<TrooperState, (TrooperState, Direction)>();
            previous[startingState] = (null, 0);
            Queue<TrooperState> openStates = new Queue<TrooperState>();
            openStates.Enqueue(startingState);
            TrooperState finish = null;

            //Stopwatch st = Stopwatch.StartNew();
            //int iterations = 0;

            while(true) {
                // ++iterations;
                // if (st.ElapsedMilliseconds > 1000) {
                //     Console.Error.WriteLine($"Time: {st.ElapsedMilliseconds}, iterations: {iterations}");
                //     st.Restart();
                // }
                var current = openStates.Dequeue();
                if(WinCondition(current)) {
                    finish = current;
                    break;
                }
                foreach(Direction dir in Directions) {
                    TrooperState newState = MoveState(current, dir);
                    
                    if(!previous.ContainsKey(newState)) {
                        previous[newState] = (current, dir);
                        openStates.Enqueue(newState);
                    }
                }
            }

            var backtrackTuple = previous[finish];
            List<Direction> result = new List<Direction>();
            while(backtrackTuple.Prev != null) {
                result.Add(backtrackTuple.Dir);
                backtrackTuple = previous[backtrackTuple.Prev];
            }
            result.Reverse();
            //Console.Error.WriteLine($"Time: {st.Elapsed}");
            return result;
        }
        
        public static List<Direction> RunAStar(TrooperState startingState) {
            Dictionary<TrooperState, (TrooperState Prev, Direction Dir)> previous = new Dictionary<TrooperState, (TrooperState, Direction)>();
            previous[startingState] = (null, Direction.D);
            SimplePriorityQueue<TrooperState> pq = new SimplePriorityQueue<TrooperState>();
            HashSet<TrooperState> closed = new HashSet<TrooperState>();
            pq.Enqueue(startingState, 0);
            TrooperState finish = null;
            startingState.G = 0;
            while(true) {
                var current = pq.Dequeue();
                closed.Add(current);
                if (WinCondition(current)){
                    finish = current;
                    break;
                }
                foreach(Direction dir in Directions) {
                    var newState = MoveState(current, dir);
                    if (closed.Contains(newState)) continue;
                    int g = current.G + 1;
                    if (!pq.Contains(newState)){
                        newState.G = g;
                        previous[newState] = (current, dir);
                        pq.Enqueue(newState, newState.F);
                    } else if (g < newState.G) {
                        newState.G = g;
                        previous[newState] = (current, dir);
                        pq.UpdatePriority(newState, newState.F);
                    }
                }

            }

            var backtrackTuple = previous[finish];
            List<Direction> result = new List<Direction>();
            while(backtrackTuple.Prev != null) {
                result.Add(backtrackTuple.Dir);
                backtrackTuple = previous[backtrackTuple.Prev];
            }
            result.Reverse();
            //Console.Error.WriteLine($"Time: {st.Elapsed}");
            return result;
        }

        public static Point Move(this Point point, Direction dir) {
            switch(dir) {
                case Direction.U:
                    return new Point(point.X, point.Y - 1);
                case Direction.D:
                    return new Point(point.X, point.Y + 1);
                case Direction.L:
                    return new Point(point.X - 1, point.Y);
                case Direction.R:
                    return new Point(point.X + 1, point.Y);
                default:
                    return point;
            }
        }

        internal static int Heuristic(TrooperState trooperState) => trooperState.Possibilities.Max(p => Heurs[p.X, p.Y]) * INCORRECTNESS_MOD;
        private static int ManhattanDist(Point p, Point x) => Math.Abs(p.X - x.X) + Math.Abs(p.Y - x.Y);
    }

    enum Direction {U, D, L, R}

    class TrooperState {

        private int? _h;
        public int H => (_h.HasValue? _h : _h = TrooperSolver.Heuristic(this)).Value;

        public int G {get; set;} = int.MaxValue;

        public int F => (G == int.MaxValue)? int.MaxValue : G + H;

        public HashSet<Point> Possibilities;

        public override bool Equals(object obj) => (obj is TrooperState s) ? Possibilities.SetEquals(s.Possibilities) : false;

        public override int GetHashCode() => Possibilities.Aggregate(983, (x,y) => x*457 + ((y.Y << 16)^ y.X));
    }

    class PointComparer : IEqualityComparer<Point> {
        
        public static PointComparer Instance = new PointComparer();

        public bool Equals(Point x, Point y) => x.X == y.X && x.Y == y.Y;

        public int GetHashCode(Point obj) => (obj.Y << 16) ^ obj.X;
    }
}
