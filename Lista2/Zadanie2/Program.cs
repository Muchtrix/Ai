using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Priority_Queue;

namespace Zadanie2
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var sr = new StreamWriter("./zad_output.txt"))
                SokobanSolver.Solve(sr, args[0]);
        }
    }

    static class SokobanSolver {
        public static char[,] Walls;

        static HashSet<Point> Destinations = new HashSet<Point>(PointComparer.Instance);

        static Direction[] Directions = new[] {Direction.Up, Direction.Down, Direction.Left, Direction.Right};

        static int ManhattanDist(Point a, Point b){
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }

        static public int Heuristic(SokobanState state) {
            int sum = 0;
            foreach(var p1 in state.Crates) sum += Destinations.Min(x => ManhattanDist(p1, x));
            return (int)(1.5 * sum);
        }

        public static List<SokobanState> RunAStar(SokobanState startingState) {
            Dictionary<SokobanState, SokobanState> previous = new Dictionary<SokobanState, SokobanState>();
            previous[startingState] = null;
            SimplePriorityQueue<SokobanState> pq = new SimplePriorityQueue<SokobanState>();
            HashSet<SokobanState> closed = new HashSet<SokobanState>();
            pq.Enqueue(startingState, 0);
            SokobanState finish = null;
            startingState.G = 0;
            while(true) {
                var current = pq.Dequeue();
                closed.Add(current);
                if (WinCondition(current)){
                    finish = current;
                    break;
                }
                foreach(Direction dir in Directions) {
                    Point newPos = current.Player.Move(dir);
                    SokobanState newState = null;
                    if (IsWall(newPos)) continue;
                    if (current.Crates.Contains(newPos)) { // przesuwamy skrzynkę
                        Point newCratePos = newPos.Move(dir);
                        if(IsWall(newCratePos) || current.Crates.Contains(newCratePos)) continue;
                        var newSet = new HashSet<Point>(current.Crates, PointComparer.Instance);
                        newSet.Remove(newPos);
                        newSet.Add(newCratePos);
                        newState = new SokobanState { 
                            Player = newPos, 
                            Crates = newSet,
                        };

                    } else { // przesuwamy tylko gracza
                        newState = new SokobanState {
                            Player = newPos,
                            Crates = current.Crates
                        };
                    }
                    if (closed.Contains(newState)) continue;
                    int g = current.G + 1;
                    if (!pq.Contains(newState)){
                        newState.G = g;
                        previous[newState] = current;
                        pq.Enqueue(newState, newState.F);
                    } else if (g < newState.G) {
                        newState.G = g;
                        previous[newState] = current;
                        pq.UpdatePriority(newState, newState.F);
                    }
                }

            }

            List<SokobanState> result = new List<SokobanState> ();
            while(finish != null) {
                result.Add(finish);
                finish = previous[finish];
            }
            result.Reverse();
            return result;
        }

        public static void Solve(TextWriter tw, string version) {
            var lines = File.ReadAllLines("./zad_input.txt");
            SokobanState startingState = new SokobanState();

            Walls = new char[lines[0].Length, lines.Length];
            for(int i = 0; i < lines.Length; ++i) {
                for(int j = 0; j < lines[0].Length; ++j) {
                    switch(lines[i][j]) {
                        case 'W':
                            Walls[j,i] = 'W';
                            break;
                        case 'K':
                            startingState.Player = new Point(j,i);
                            break;
                        case 'B':
                            startingState.Crates.Add(new Point(j,i));
                            break;
                        case 'G':
                            Walls[j,i] = 'G';
                            Destinations.Add(new Point(j,i));
                            break;
                        case '*':
                            startingState.Crates.Add(new Point(j,i));
                            Walls[j,i] = 'G';
                            Destinations.Add(new Point(j,i));
                            break;
                        case '+':
                            startingState.Player = new Point(j,i);
                            Walls[j,i] = 'G';
                            Destinations.Add(new Point(j,i));
                            break;
                    }
                }
            }

            switch(version){
                case "--bfs":
                    tw.WriteLine(EncodeMoves(RunBFS(startingState)));
                    return;
                case "--astar":
                    tw.WriteLine(EncodeMoves(RunAStar(startingState)));
                    return;
                case "--best":
                    tw.WriteLine(string.Concat(RunBestFirst(startingState)));
                    return;
            }
        }

        static bool WinCondition(SokobanState st) => st.Crates.All(x => Walls[x.X, x.Y] == 'G');

        static List<SokobanState> RunBFS(SokobanState startingState) {
            Dictionary<SokobanState, SokobanState> previous = new Dictionary<SokobanState, SokobanState>();
            previous[startingState] = null;
            Queue<SokobanState> openStates = new Queue<SokobanState>();
            openStates.Enqueue(startingState);
            SokobanState finish = null;

            while(true) {
                var current = openStates.Dequeue();
                if(WinCondition(current)) {
                    finish = current;
                    break;
                }
                foreach(Direction dir in Directions) {
                    Point newPos = current.Player.Move(dir);
                    SokobanState newState = null;
                    if (IsWall(newPos)) continue;
                    if (current.Crates.Contains(newPos)) { // przesuwamy skrzynkę
                        Point newCratePos = newPos.Move(dir);
                        if(IsWall(newCratePos) || current.Crates.Contains(newCratePos)) continue;
                        newState = new SokobanState { 
                            Player = newPos, 
                            Crates = current.Crates.Except(new[]{newPos}).Append(newCratePos).ToHashSet(PointComparer.Instance),
                        };

                    } else { // przesuwamy tylko gracza
                        newState = new SokobanState {
                            Player = newPos,
                            Crates = current.Crates
                        };
                    }
                    if(!previous.ContainsKey(newState)) {
                        previous[newState] = current;
                        openStates.Enqueue(newState);
                    }
                }
            }

            List<SokobanState> result = new List<SokobanState> ();
            while(finish != null) {
                result.Add(finish);
                finish = previous[finish];
            }
            result.Reverse();
            return result;
        }

        static HashSet<Point> Reachable(SokobanState st) {
            HashSet<Point> visited = new HashSet<Point>(PointComparer.Instance) {st.Player};
            Queue<Point> open = new Queue<Point>();
            open.Enqueue(st.Player);
            while(open.Any()) {
                var current = open.Dequeue();
                foreach(var dir in Directions) {
                    var newPos = current.Move(dir);
                    if (IsWall(newPos) || st.Crates.Contains(newPos) || visited.Contains(newPos)) continue;
                    visited.Add(newPos);
                    open.Enqueue(newPos);
                }
            }
            return visited;
        }

        static List<(Point PushPos, Point CratePos, Point NewCratePos)> PossiblePushStarts(SokobanState st) {
            var res = new List<(Point PushPos, Point CratePos, Point NewCratePos)>();
            foreach(var crate in st.Crates) {
                var mv = Directions.Select(x => crate.Move(x)).ToArray();
                if (!IsWall(mv[0]) && !IsWall(mv[1]) && !st.Crates.Contains(mv[0]) && !st.Crates.Contains(mv[1])) {
                    res.Add((mv[0], crate, mv[1]));
                    res.Add((mv[1], crate, mv[0]));
                }
                if (!IsWall(mv[2]) && !IsWall(mv[3]) && !st.Crates.Contains(mv[2]) && !st.Crates.Contains(mv[3])) {
                    res.Add((mv[2], crate, mv[3]));
                    res.Add((mv[3], crate, mv[2]));
                }
            }
            return res;
        }

        static List<string> RunBestFirst(SokobanState startingState) {
            Dictionary<SokobanState, (SokobanState, Point)> previous = new Dictionary<SokobanState, (SokobanState, Point)>();
            previous[startingState] = (null, new Point(0,0));
            SimplePriorityQueue<SokobanState> pq = new SimplePriorityQueue<SokobanState>();
            pq.Enqueue(startingState, 0);
            SokobanState finish = null;

            while(true) {
                var current = pq.Dequeue();
                if (WinCondition(current)){
                    finish = current;
                    break;
                }
                var reachable = Reachable(current);
                var moves = PossiblePushStarts(current);

                foreach(var move in moves.Where(x => reachable.Contains(x.PushPos))) {
                    var newSet = new HashSet<Point>(current.Crates, PointComparer.Instance);
                    newSet.Remove(move.CratePos);
                    newSet.Add(move.NewCratePos);
                    var newState = new SokobanState {
                        Player = move.CratePos,
                        Crates = newSet
                    };
                    if (!previous.ContainsKey(newState)) {
                        previous[newState] = (current, move.PushPos);
                        pq.Enqueue(newState, newState.H);
                    }
                }

            }


            List<(SokobanState, Point)> results = new List<(SokobanState, Point)> ();
            var fin = finish;
            while(finish != startingState) {
                results.Add(previous[finish]);
                finish = previous[finish].Item1;
            }

            List<string> res = new List<string>();
            for(int i = results.Count - 1; i >= 0; --i) {
                string inter = FindPath(results[i].Item1, results[i].Item2);
                inter += results[i].Item2.Compare((i == 0 ? fin : results[i - 1].Item1).Player);
                res.Add(inter);
            }
            return res;
        }

        static string FindPath(SokobanState startingState, Point endPosition) {
            Dictionary<Point, Point?> previous = new Dictionary<Point, Point?>(PointComparer.Instance);
            previous[startingState.Player] = null;
            Queue<Point> opened = new Queue<Point>();
            opened.Enqueue(startingState.Player);
            Point finish;
            bool breakout = true;
            while(breakout) {
                var current = opened.Dequeue();
                if (current == endPosition) {
                    finish = current;
                    break;
                }
                foreach(Direction dir in Directions) {
                    Point newPos = current.Move(dir);
                    if (previous.ContainsKey(newPos)) continue;
                    if (IsWall(newPos) || startingState.Crates.Contains(newPos)) continue;
                    previous[newPos] = current;
                    opened.Enqueue(newPos);
                }

            }

            List<Point> result = new List<Point> ();
            while(previous[finish] != null) {
                result.Add(finish);
                finish = previous[finish].Value;
            }
            result.Add(finish);
            result.Reverse();
            return EncodeMoves(result);
        }

        static string EncodeMoves(List<SokobanState> states) {
            char[] result = new char[states.Count - 1];
            for(int i = 0; i < states.Count - 1; ++i) {
                result[i] = states[i].Player.Compare(states[i+1].Player);
            }

            return new string(result);
        }

        static string EncodeMoves(List<Point> states) {
            char[] result = new char[states.Count - 1];
            for(int i = 0; i < states.Count - 1; ++i) {
                result[i] = states[i].Compare(states[i+1]);
            }

            return new string(result);
        }

        public static Point Move(this Point point, Direction dir) {
            switch(dir) {
                case Direction.Up:
                    return new Point(point.X, point.Y - 1);
                case Direction.Down:
                    return new Point(point.X, point.Y + 1);
                case Direction.Left:
                    return new Point(point.X - 1, point.Y);
                case Direction.Right:
                    return new Point(point.X + 1, point.Y);
                default:
                    return point;
            }
        }

        public static char Compare(this Point point, Point next) {
            if (point.X == next.X) {
                if (point.Y < next.Y) return 'D';
                else return 'U';
            }
            if (point.X < next.X) return 'R';
            return 'L';
        }

        public static bool IsWall(this Point p) => Walls[p.X, p.Y] == 'W';
    }

    enum Direction {
        Up,
        Down,
        Left,
        Right
    }

    class SokobanState : FastPriorityQueueNode{
        public Point Player;
        public HashSet<Point> Crates = new HashSet<Point>(PointComparer.Instance);

        private int? _h;
        public int H => (_h.HasValue? _h : _h = SokobanSolver.Heuristic(this)).Value;

        public int G {get; set;} = int.MaxValue;

        public int F => (G == int.MaxValue)? int.MaxValue : G + H;

        public override bool Equals(object obj) {
            if (obj is SokobanState s) {
                return Player == s.Player && Crates.SetEquals(s.Crates);
            }
            return false;
        }

        public override int GetHashCode() => Player.GetHashCode() ^ Crates.Aggregate(983, (x,y) => x*457 + ((y.Y << 16)^ y.X));
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
