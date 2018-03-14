using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Zadanie1 {
    class Program {
        static void Main (string[] args) {
            using (StreamReader sr = new StreamReader("./zad_input.txt"))
            using (StreamWriter sw = new StreamWriter("./zad_output.txt")) {
                PicrossSolver.SolvePicture(sr, sw);
            }
            // Console.WriteLine ("Hello");
            // while (true) {
            //     PicrossSolver.SolvePicture(Console.In, Console.Out);
            // }
        }
    }

    static class PicrossSolver {
        private static Random RNG = new Random ();
        private const double FailProb = 0.2;
        private const int ResetCounter = 50000;

        private static Dictionary<List<int>, List<int>> CombinationCache = new Dictionary<List<int>, List<int>>();
        static int OptDist (int current, List<int> sectors, int patternLength) {
            int SparseBitcount (int i) {
                i = i - ((i >> 1) & 0x55555555);
                i = (i & 0x33333333) + ((i >> 2) & 0x33333333);
                return (((i + (i >> 4)) & 0x0F0F0F0F) * 0x01010101) >> 24;
            }

            if (!sectors.Any ()) return SparseBitcount (current);
            int required = patternLength;
            if (!CombinationCache.ContainsKey(sectors)) {
                CombinationCache[sectors] = PossibleAllSectors(sectors, patternLength).ToList();
            }
            return CombinationCache[sectors].Min(x => SparseBitcount(x ^ current));
        }

        private static IEnumerable<(int pattern, int remainingLength)> PossibleSector (int lengthOfSector, int lengthOfSpace) {
            int sector = (1 << lengthOfSector) - 1;
            for (int i = lengthOfSpace - lengthOfSector; i >= 0; --i) {
                yield return (sector << i, i - 1);
            }
        }

        private static IEnumerable<int> PossibleAllSectors (List<int> sectors, int remainingLength, int currentIdx = 0) {
            if (currentIdx >= sectors.Count) {
                yield return 0;
                yield break;
            }

            if (remainingLength <= 0) {
                yield break;
            }

            foreach (var placement in PossibleSector (sectors[currentIdx], remainingLength)) {
                foreach (int placementOfRest in PossibleAllSectors (sectors, placement.remainingLength, currentIdx + 1)) {
                    yield return placement.pattern | placementOfRest;
                }
            }
        }

        static void SolvePicture (List<int>[] rows, List<int>[] columns, TextWriter writer) {
            CombinationCache.Clear();
            int[, ] picture = new int[columns.Length, rows.Length];
            int[] rowScores = new int[rows.Length];
            int[] columnScores = new int[columns.Length];

            int CheckColumn (int idx) {
                int hash = 0;
                for (int i = 0; i < rows.Length; ++i) {
                    hash = hash << 1 | picture[idx, i];
                }
                return OptDist (hash, columns[idx], rows.Length);
            }

            int CheckRow (int idx) {
                int hash = 0;
                for (int i = 0; i < columns.Length; ++i) {
                    hash = hash << 1 | picture[i, idx];
                }
                return OptDist (hash, rows[idx], columns.Length);
            }

            void InitDists() {
                for (int i = 0; i < columns.Length; ++i) {
                    columnScores[i] = CheckColumn (i);
                }
                for (int i = 0; i < rows.Length; ++i) {
                    rowScores[i] = CheckRow (i);
                }
            }

            void DrawPicture () {
                for (int y = 0; y < rows.Length; ++y) {
                    for (int x = 0; x < columns.Length; ++x) {
                        writer.Write (picture[x, y] == 1 ? '#' : '.');
                    }
                    writer.WriteLine ();
                }
            }

            int turnCounter = 0;
            //Stopwatch st = Stopwatch.StartNew();
            InitDists();
            while (columnScores.Any(x => x != 0) || rowScores.Any(x => x != 0)) {
                turnCounter++;
                if (turnCounter % ResetCounter == 0) {
                    picture = new int[columns.Length, rows.Length];
                    InitDists();
                }
                if (RNG.NextDouble() <= FailProb) {
                    int rx = RNG.Next() % columns.Length;
                    int ry = RNG.Next() % rows.Length;
                    picture[rx, ry] = 1 - picture[rx, ry];
                    rowScores[ry] = CheckRow(ry);
                    columnScores[rx] = CheckColumn(rx);
                } else {
                    int rx = Enumerable.Range(0, columns.Length).OrderByDescending(x => columnScores[x]).First();

                    int bestDec = int.MinValue;
                    int bestCol = -1;
                    int bestRow = -1;
                    int bestY = -1;

                    for (int ry = 0; ry < rows.Length; ++ry) {
                        int oldScore = columnScores[rx] + rowScores[ry];
                        picture[rx, ry] = 1 - picture[rx, ry];
                        int rrx = CheckColumn(rx);
                        int rry = CheckRow(ry);
                        int newScore = rrx + rry;
                        picture[rx, ry] = 1 - picture[rx, ry];
                        if (oldScore - newScore > bestDec) {
                            bestDec = oldScore - newScore;
                            bestRow = rry;
                            bestCol = rrx;
                            bestY = ry;
                        }
                    }
                    picture[rx, bestY] = 1 - picture[rx, bestY];
                    columnScores[rx] = bestCol;
                    rowScores[bestY] = bestRow; 
                }
            }
            Console.Error.WriteLine($"No of iterations: {turnCounter}");
            //Console.Error.WriteLine($"Time: {st.Elapsed}");
            DrawPicture ();
        }

        public static void SolvePicture (TextReader reader, TextWriter writer) {
            int[] dimensions = reader.ReadLine ().Split (' ').Select (x => int.Parse (x)).ToArray ();
            List<int>[] rows = new List<int>[dimensions[0]];
            List<int>[] columns = new List<int>[dimensions[1]];
            for (int i = 0; i < dimensions[0]; ++i) {
                rows[i] = reader.ReadLine ().Split (' ').Select (x => int.Parse (x)).ToList ();
            }
            for (int i = 0; i < dimensions[1]; ++i) {
                columns[i] = reader.ReadLine ().Split (' ').Select (x => int.Parse (x)).ToList ();
            }

            SolvePicture (rows, columns, writer);
        }
    }
}