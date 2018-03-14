using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Zadanie2
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var sr = new StreamWriter("./zad_output.txt"))
                SokobanSolver.Solve(sr);
        }
    }

    static class SokobanSolver {
        public static char[,] Walls;

        static HashSet<Point> Destinations = new HashSet<Point>();

        static Direction[] Directions = new[] {Direction.Up, Direction.Down, Direction.Left, Direction.Right};

        public static void Solve(TextWriter tw) {
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

            var states = RunBFS(startingState);
            tw.WriteLine(EncodeMoves(states));
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
                            Crates = current.Crates.Except(new[]{newPos}).Append(newCratePos).ToHashSet(),
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

        static string EncodeMoves(List<SokobanState> states) {
            char[] result = new char[states.Count - 1];
            for(int i = 0; i < states.Count - 1; ++i) {
                result[i] = states[i].Player.Compare(states[i+1].Player);
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

    class SokobanState {
        public Point Player;
        public HashSet<Point> Crates = new HashSet<Point>();

        public override bool Equals(object obj) {
            if (obj is SokobanState s) {
                return Player == s.Player && Crates.SetEquals(s.Crates);
            }
            return false;
        }

        public override int GetHashCode() => Player.GetHashCode() ^ Crates.Select(x => x.GetHashCode()).Aggregate((x,y) => x ^ y);
    }
}
