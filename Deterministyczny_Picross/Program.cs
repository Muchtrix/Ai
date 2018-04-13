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
            {2, '.'},
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
            var enumeration = CombinationCache[sectors];
            bool isContradicting = true;
            foreach(int combination in enumeration) {
                if (((combination & sureFilled) == sureFilled) && ((~combination & sureEmpty) == sureEmpty)) {
                    isContradicting = false;
                    newFilled &= combination;
                    newEmpty &= ~combination;
                }
            }
            if (isContradicting) throw new ContradictingStateException();
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
            byte[, ] picture = new byte[columns.Length, rows.Length];

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
            bool isDetermined = true;
            do {
                try
                {
                    bool anythingChanged = false;
                    for (int idx = 0; idx < columns.Length; ++idx) {
                        int fullHash = 0, emptyHash = 0;
                        for (int i = 0; i < rows.Length; ++i) {
                            fullHash = (fullHash << 1) | ((picture[idx, i] == 1)? 1 : 0);
                            emptyHash = (emptyHash << 1) | ((picture[idx, i] == 2)? 1 : 0);
                        }
                        var (newFullHash, newEmptyHash) = FillStep(fullHash, emptyHash, columns[idx], rows.Length);
                        if ((fullHash ^ newFullHash) == 0 && (emptyHash ^ newEmptyHash) == 0) {
                            continue;
                        } else {
                            anythingChanged = true;
                        }
                        for (int i = rows.Length - 1; i >= 0;  --i) {
                            picture[idx, i] = (newFullHash % 2 == 1)? (byte)1 : (newEmptyHash % 2 == 1)? (byte)2 : (byte)0;
                            newFullHash >>= 1;
                            newEmptyHash >>= 1;
                        }
                    }

                    for (int idx = 0; idx < rows.Length; ++idx) {
                        int fullHash = 0, emptyHash = 0;
                        for (int i = 0; i < columns.Length; ++i) {
                            fullHash = (fullHash << 1) | ((picture[i, idx] == 1)? 1 : 0);
                            emptyHash = (emptyHash << 1) | ((picture[i, idx] == 2)? 1 : 0);
                        }
                        var (newFullHash, newEmptyHash) = FillStep(fullHash, emptyHash, rows[idx], columns.Length);
                        if ((fullHash ^ newFullHash) == 0 && (emptyHash ^ newEmptyHash) == 0) {
                            continue;
                        } else {
                            anythingChanged = true;
                        }
                        for (int i = columns.Length - 1; i >= 0;  --i) {
                            picture[i, idx] = (newFullHash % 2 == 1)? (byte)1 : (newEmptyHash % 2 == 1)? (byte)2 : (byte)0;
                            newFullHash >>= 1;
                            newEmptyHash >>= 1;
                        }
                    }
                    Console.WriteLine(anythingChanged);
                    if (! anythingChanged) {
                        //TODO: guess new pixel and push state on stack
                    }
                }
                catch (ContradictingStateException)
                {
                    Console.Error.WriteLine("CONTRADICTION!!");
                    //TODO: pop a state from stack for backtracking
                }

                isDetermined = true;
                foreach (var px in picture) {
                    isDetermined = isDetermined && (px != 0);
                }
            } while (!isDetermined);
            DrawPicture(writer);
        }

        public static void SolvePicture (TextReader reader, TextWriter writer) {
            int[] dimensions = reader.ReadLine ().Split (' ').Select (x => int.Parse (x)).ToArray ();
            List<int>[] rows = new List<int>[dimensions[0]];
            List<int>[] columns = new List<int>[dimensions[1]];
            for (int i = 0; i < dimensions[0]; ++i) {
                rows[i] = reader.ReadLine ().Split (' ').Select (x => int.Parse (x)).ToList ();
                if (rows[i].Count == 1 && rows[i][0] == 0) rows[i].Clear();
            }
            for (int i = 0; i < dimensions[1]; ++i) {
                columns[i] = reader.ReadLine ().Split (' ').Select (x => int.Parse (x)).ToList ();
                if (columns[i].Count == 1 && columns[i][0] == 0) columns[i].Clear();
            }

            SolvePicture (rows, columns, writer);
        }
    }

    class PicrossState {
        public byte[,] Picture;
        public (int X, int Y, byte Value) ChangedPixel;
    }

    class ContradictingStateException : Exception { }
}
