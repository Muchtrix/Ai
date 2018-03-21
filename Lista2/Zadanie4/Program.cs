using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Zadanie4
{
    class Program
    {
        static void Main(string[] args)
        {
            using (StreamWriter sr = new StreamWriter("./zad_output.txt"))
                TrooperSolver.Solve(sr);
        }
    }

    static class TrooperSolver {
        public static char[,] Walls;

        static HashSet<Point> Destinations = new HashSet<Point>();

        static Direction[] Directions = new[] {Direction.U, Direction.D, Direction.L, Direction.R};
        static Random RNG = new Random();
        const int RANDOM_COUNT = 40;

        static List<Direction> MovePrefix = new List<Direction>();

        public static void Solve(TextWriter tw) {
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

            int oldCount = startingState.Possibilities.Count;
            for(int i = 0; i < lines[0].Length; ++i){
                startingState = MoveState(startingState, Direction.R);
                MovePrefix.Add(Direction.R);
            }

            for(int i = 0; i < lines.Length; ++i){
                startingState = MoveState(startingState, Direction.U);
                MovePrefix.Add(Direction.U);
            }

            for(int i = 0; i < lines[0].Length; ++i){
                startingState = MoveState(startingState, Direction.L);
                MovePrefix.Add(Direction.L);
            }

            for(int i = 0; i < lines.Length; ++i){
                startingState = MoveState(startingState, Direction.D);
                MovePrefix.Add(Direction.D);
            }

            // int dl = (lines[0].Length + lines.Length);

            // for(int i = 0; i < dl; ++i){
            //     startingState = MoveState(startingState, Direction.R);
            //     startingState = MoveState(startingState, Direction.D);
            //     MovePrefix.Add(Direction.R);
            //     MovePrefix.Add(Direction.D);
            // }

            // while(startingState.Possibilities.Count > 2) {
            //     mv++;
            //     startingState = RandomMove(startingState);
            // }

            //Console.Error.WriteLine($"After beginning {mv} moves: {oldCount}->{startingState.Possibilities.Count}");

            var moves = RunBFS(startingState);
            Console.WriteLine(string.Concat(MovePrefix.Concat(moves)));
            tw.Write(string.Concat(MovePrefix));
            tw.WriteLine(string.Concat(moves));
        }

        //static bool WinCondition(TrooperState st) => st.Possibilities.All(x => Walls[x.X, x.Y] == 'G');
        static bool WinCondition(TrooperState st) => st.Possibilities.SetEquals(Destinations);

        static TrooperState RandomMove(TrooperState state) {
            Direction dir = Directions[RNG.Next(Directions.Length)];
            MovePrefix.Add(dir);
            return MoveState(state, dir);
        }

        static Point MoveOrStay(Point p, Direction Dir) {
            Point n = p.Move(Dir);
            return IsWall(n) ? p : n;
        }

        static TrooperState MoveState(TrooperState state, Direction dir) => new TrooperState {Possibilities = state.Possibilities.Select(x => MoveOrStay(x, dir)).ToHashSet(PointComparer.Instance)};

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

        public static bool IsWall(this Point p) => Walls[p.X, p.Y] == '#';
    }

    enum Direction {U, D, L, R}

    class TrooperState {
        public HashSet<Point> Possibilities;

        public override bool Equals(object obj) {
            if (obj is TrooperState s) {
                return Possibilities.SetEquals(s.Possibilities);
            }
            return false;
        }

        public override int GetHashCode() => Possibilities.Aggregate(983, (x,y) => x*457 + ((y.Y << 16)^ y.X));
    }

    class PointComparer : IEqualityComparer<Point> {
        
        public static PointComparer Instance = new PointComparer();

        public bool Equals(Point x, Point y) {
            return x.X == y.X && x.Y == y.Y;
        }

        public int GetHashCode(Point obj) {
            // Perfect hash for practical bitmaps, their width/height is never >= 65536
            return (obj.Y << 16) ^ obj.X;
        }
    }
}
