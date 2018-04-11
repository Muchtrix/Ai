using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Deterministyczny_Picross
{
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
        private const int ResetCounter = 20000;
        
        static private Dictionary<int, char> DrawingSymbols = new Dictionary<int, char>() {
            {-1, '.'},
            {0, ' '},
            {1, '#'}
        };

        private static Dictionary<List<int>, List<int>> CombinationCache = new Dictionary<List<int>, List<int>>();

        private static (int newFilled, int newEmpty) FillStep(int sureFilled, int sureEmpty, List<int> sectors, int patternLength) {
            if (!sectors.Any()) return (0, (1 << patternLength) - 1);
            if (!CombinationCache.ContainsKey(sectors)) {
                CombinationCache[sectors] = PossibleAllSectors(sectors, patternLength).ToList();
            }
            int newFilled = (1 << patternLength) - 1, newEmpty = (1 << patternLength) - 1;
            foreach(int combination in CombinationCache[sectors]) {
                if (((combination & sureFilled) == sureFilled) && ((~combination & sureEmpty) == sureEmpty)) {
                    newFilled &= combination;
                    newEmpty &= ~combination;
                }
                //if ((combination & sureFilled) == sureFilled) newFilled &= combination;
                //if ((~combination & sureEmpty) == sureEmpty) newEmpty &= ~combination;
            }
            return (newFilled, newEmpty);
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
            Stopwatch st = Stopwatch.StartNew();
            int[, ] picture = new int[columns.Length, rows.Length];
            bool[] full = new bool[columns.Length + rows.Length];

            Console.WriteLine("---");

            void DrawPicture (TextWriter wr) {
                for (int y = 0; y < rows.Length; ++y) {
                    for (int x = 0; x < columns.Length; ++x) {
                        wr.Write (DrawingSymbols[picture[x, y]]);
                    }
                    wr.WriteLine ();
                }
            }

            // int turnCounter = 0;
            while (!full.All(x => x)) {
                for (int idx = 0; idx < columns.Length; ++idx) {
                    int fullHash = 0, emptyHash = 0;
                    for (int i = 0; i < rows.Length; ++i) {
                        fullHash = (fullHash << 1) | ((picture[idx, i] == 1)? 1 : 0);
                        emptyHash = (emptyHash << 1) | ((picture[idx, i] == -1)? 1 : 0);
                    }
                    (fullHash, emptyHash) = FillStep(fullHash, emptyHash, columns[idx], rows.Length);
                    full[idx] = (fullHash | emptyHash) == ((1 << rows.Length) - 1);
                    for (int i = rows.Length - 1; i >= 0;  --i) {
                        picture[idx, i] = (fullHash % 2 == 1)? 1 : (emptyHash % 2 == 1)? -1 : 0;
                        fullHash >>= 1;
                        emptyHash >>= 1;
                    }
                }

                for (int idx = 0; idx < rows.Length; ++idx) {
                    int fullHash = 0, emptyHash = 0;
                    for (int i = 0; i < columns.Length; ++i) {
                        fullHash = (fullHash << 1) | ((picture[i, idx] == 1)? 1 : 0);
                        emptyHash = (emptyHash << 1) | ((picture[i, idx] == -1)? 1 : 0);
                    }
                    (fullHash, emptyHash) = FillStep(fullHash, emptyHash, rows[idx], columns.Length);
                    full[columns.Length + idx] = (fullHash | emptyHash) == ((1 << columns.Length) - 1);
                    for (int i = columns.Length - 1; i >= 0;  --i) {
                        picture[i, idx] = (fullHash % 2 == 1)? 1 : (emptyHash % 2 == 1)? -1 : 0;
                        fullHash >>= 1;
                        emptyHash >>= 1;
                    }
                }
                // ++turnCounter;
                // DrawPicture(Console.Out);
                // Console.WriteLine();
            }
            // Console.WriteLine($"No of iterations: {turnCounter}, time: {st.Elapsed}");
            DrawPicture(writer);
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
