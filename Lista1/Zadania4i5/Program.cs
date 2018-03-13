using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Zadania4i5
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!Console.IsInputRedirected)Console.Error.WriteLine("READY LUL");

            string line;
            Stopwatch st = Stopwatch.StartNew();
            while((line = Console.ReadLine()) != null) {
                PicrossSolver.ReadLine(line);
            }
            Console.WriteLine($"Total running time: {st.Elapsed}");
        }
    }

    static class PicrossSolver {
        private static Random RNG = new Random();
        private static double FailProb = 0.01;
        private static int OptDist(int current, int patternLength, int length) {
            int SparseBitcount(int n) {
                int count = 0;
                while (n != 0)
                {
                    count++;
                    n &= (n - 1);
                }
                return count;
            }

            if (length == 0) return SparseBitcount(current);
            int required = patternLength;
            int desired = (1<<length) - 1;
            for(int i = 0; i <= patternLength - length; ++i){
                int moves = SparseBitcount(current ^ (desired<<i));
                required = Math.Min(moves, required);
            }

            return required;
        }
        public static int OptDist(List<int> pattern, int length){
            int CodeList(List<int> input) {
                int res = 0;
                foreach(int i in input){
                    res = res<<1 | i;
                }
                return res;
            }

            return OptDist(CodeList(pattern), pattern.Count, length);
        }

        public static void SolvePicture(int[] rows, int[] columns) {
            int[,] picture = new int[columns.Length, rows.Length];
            int[] rowScores = new int[rows.Length];
            int[] columnScores = new int[columns.Length];
            Stopwatch st = Stopwatch.StartNew();

            int CheckColumn(int idx) {
                int hash = 0;
                for(int i = 0; i < rows.Length; ++i) {
                    hash = hash<<1 | picture[idx, i];
                }
                return OptDist(hash, rows.Length, columns[idx]);
            }

            int CheckRow(int idx) {
                int hash = 0;
                for(int i = 0; i < columns.Length; ++i) {
                    hash = hash<<1 | picture[i, idx];
                }
                return OptDist(hash, columns.Length, rows[idx]);
            }

            bool CheckPicture() {
                bool res = true;
                for(int i = 0; i < columns.Length; ++i) {
                    if ((columnScores[i] = CheckColumn(i)) != 0) res = false;
                }
                for(int i = 0; i < rows.Length; ++i) {
                    if ((rowScores[i] = CheckRow(i)) != 0) res = false;
                }
                return res;
            }

            void DrawPicture() {
                for(int y = 0; y < rows.Length; ++y) {
                    for(int x = 0; x < columns.Length; ++x) {
                        Console.Write(picture[x,y] == 1 ? '#' : '.');
                    }
                    Console.WriteLine();
                }
            }

            int turnCounter = 0;
            while(!CheckPicture()) {
                turnCounter++;
                if (turnCounter > 1000000) {
                    turnCounter = 0;
                    picture = new int[columns.Length, rows.Length];
                }
                if (RNG.NextDouble() <= FailProb) {
                    int rx = RNG.Next() % columns.Length;
                    int ry = RNG.Next() % rows.Length;
                    picture[rx, ry] = 1 - picture[rx, ry];
                }
                else {
                    int rx = Enumerable.Range(0, columns.Length).OrderByDescending(x => columnScores[x]).First();

                    int bestDec = int.MinValue;
                    int bestY = -1;

                    for(int ry = 0; ry < rows.Length; ++ry) {
                        int oldScore = columnScores[rx] + rowScores[ry];
                        picture[rx, ry] = 1- picture[rx, ry];
                        int newScore = CheckColumn(rx) + CheckRow(ry);
                        picture[rx, ry] = 1- picture[rx, ry];
                        if (oldScore - newScore > bestDec) {
                            bestDec = oldScore - newScore;
                            bestY = ry;
                        }
                    }
                    picture[rx, bestY] = 1 - picture[rx, bestY];
                }
            }

            DrawPicture();
            Console.WriteLine($"Running time: {st.Elapsed}\n");
        }

        public static string TestExercise4(string line) {
            string[] parts = line.Split(' ');
            return $"Pattern: {parts[0]}:{parts[1]}, required: {OptDist(parts[0].Select(x => x - '0').ToList(), int.Parse(parts[1]))}, expected: {parts[2]}, {(OptDist(parts[0].Select(x => x - '0').ToList(), int.Parse(parts[1])) == int.Parse(parts[2]) ? "PASS" : "FAIL")}";
        }

        public static void ReadLine(string line) {
            string[] splits = line.Split(' ');
            SolvePicture(splits[0].Split(',').Select(x => int.Parse(x)).ToArray(), splits[1].Split(',').Select(x => int.Parse(x)).ToArray());
        }
    }
}
