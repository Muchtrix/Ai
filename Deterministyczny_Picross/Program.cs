using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;

namespace Deterministyczny_Picross
{
    class Program {
        static void Main (string[] args) {
            using (StreamReader sr = new StreamReader("./zad_input.txt"))
            using (StreamWriter sw = new StreamWriter("./zad_output.txt")) {
                PicrossSolver.SolvePicture(sr, sw);
            }
        }
    }

    static class PicrossSolver {
        private static Dictionary<int, char> DrawingSymbols = new Dictionary<int, char>() {
            {2, '.'},
            {0, ' '},
            {1, '#'}
        };
        private static Dictionary<List<int>, List<int>> CombinationCache = new Dictionary<List<int>, List<int>>();
        private static Dictionary<List<int>, List<long>> LongCombinationCache = new Dictionary<List<int>, List<long>>();
        private static Dictionary<List<int>, List<BigInteger>> BigCombinationCache = new Dictionary<List<int>, List<BigInteger>>();

        private static (int newFilled, int newEmpty) FillStep(int sureFilled, int sureEmpty, List<int> sectors, int patternLength) {
            IEnumerable<(int pattern, int remainingLength)> PossibleSector (int lengthOfSector, int lengthOfSpace) {
                int sector = (1 << lengthOfSector) - 1;
                for (int i = lengthOfSpace - lengthOfSector; i >= 0; --i) {
                    yield return (sector << i, i - 1);
                }
            }

            IEnumerable<int> PossibleAllSectors (List<int> sectoors, int remainingLength, int currentIdx = 0) {
                if (currentIdx >= sectoors.Count) {
                    yield return 0;
                    yield break;
                }

                if (remainingLength <= 0) {
                    yield break;
                }

                foreach (var placement in PossibleSector (sectoors[currentIdx], remainingLength)) {
                    foreach (int placementOfRest in PossibleAllSectors (sectoors, placement.remainingLength, currentIdx + 1)) {
                        yield return placement.pattern | placementOfRest;
                    }
                }
            }
            
            if (!sectors.Any()) return (0, (1 << patternLength) - 1);
            if (!CombinationCache.ContainsKey(sectors)) {
                CombinationCache[sectors] = PossibleAllSectors(sectors, patternLength).ToList();
            }
            int newFilled = (1 << patternLength) - 1, newEmpty = (1 << patternLength) - 1;
            bool isContradicting = true;
            foreach(int combination in CombinationCache[sectors]) {
                if (((combination & sureFilled) == sureFilled) && ((combination | sureFilled) == combination) 
                && ((~combination & sureEmpty) == sureEmpty) && ((~combination | sureEmpty) == ~combination)) {
                    isContradicting = false;
                    newFilled &= combination;
                    newEmpty &= ~combination;
                }
            }
            if (isContradicting) throw new ContradictingStateException();
            return (newFilled, newEmpty);
        }

        private static (long newFilled, long newEmpty) FillStep(long sureFilled, long sureEmpty, List<int> sectors, int patternLength) {
            IEnumerable<(long pattern, int remainingLength)> PossibleSector (int lengthOfSector, int lengthOfSpace) {
                long sector = (1L << lengthOfSector) - 1;
                for (int i = lengthOfSpace - lengthOfSector; i >= 0; --i) {
                    yield return (sector << i, i - 1);
                }
            }

            IEnumerable<long> PossibleAllSectors (List<int> sectoors, int remainingLength, int currentIdx = 0) {
                if (currentIdx >= sectoors.Count) {
                    yield return 0;
                    yield break;
                }

                if (remainingLength <= 0) {
                    yield break;
                }

                foreach (var placement in PossibleSector (sectoors[currentIdx], remainingLength)) {
                    foreach (long placementOfRest in PossibleAllSectors (sectoors, placement.remainingLength, currentIdx + 1)) {
                        yield return placement.pattern | placementOfRest;
                    }
                }
            }
            
            if (!sectors.Any()) return (0, (1L << patternLength) - 1);
            if (!LongCombinationCache.ContainsKey(sectors)) {
                LongCombinationCache[sectors] = PossibleAllSectors(sectors, patternLength).ToList();
            }
            long newFilled = (1L << patternLength) - 1, newEmpty = (1L << patternLength) - 1;
            bool isContradicting = true;
            foreach(long combination in LongCombinationCache[sectors]) {
                if (((combination & sureFilled) == sureFilled) && ((combination | sureFilled) == combination) 
                && ((~combination & sureEmpty) == sureEmpty) && ((~combination | sureEmpty) == ~combination)) {
                    isContradicting = false;
                    newFilled &= combination;
                    newEmpty &= ~combination;
                }
            }
            if (isContradicting) throw new ContradictingStateException();
            return (newFilled, newEmpty);
        }

        private static (BigInteger newFilled, BigInteger newEmpty) FillStep(BigInteger sureFilled, BigInteger sureEmpty, List<int> sectors, int patternLength) {
            IEnumerable<(BigInteger pattern, int remainingLength)> PossibleSector (int lengthOfSector, int lengthOfSpace) {
                BigInteger sector = (BigInteger.One << lengthOfSector) - 1;
                for (int i = lengthOfSpace - lengthOfSector; i >= 0; --i) {
                    yield return (sector << i, i - 1);
                }
            }

            IEnumerable<BigInteger> PossibleAllSectors (List<int> sectoors, int remainingLength, int currentIdx = 0) {
                if (currentIdx >= sectoors.Count) {
                    yield return 0;
                    yield break;
                }

                if (remainingLength <= 0) {
                    yield break;
                }

                foreach (var placement in PossibleSector (sectoors[currentIdx], remainingLength)) {
                    foreach (BigInteger placementOfRest in PossibleAllSectors (sectoors, placement.remainingLength, currentIdx + 1)) {
                        yield return placement.pattern | placementOfRest;
                    }
                }
            }
            
            if (!sectors.Any()) return (0, (1L << patternLength) - 1);
            if (!BigCombinationCache.ContainsKey(sectors)) {
                BigCombinationCache[sectors] = PossibleAllSectors(sectors, patternLength).ToList();
            }
            BigInteger newFilled = (1L << patternLength) - 1, newEmpty = (1L << patternLength) - 1;
            bool isContradicting = true;
            foreach(long combination in BigCombinationCache[sectors]) {
                if (((combination & sureFilled) == sureFilled) && ((combination | sureFilled) == combination) 
                && ((~combination & sureEmpty) == sureEmpty) && ((~combination | sureEmpty) == ~combination)) {
                    isContradicting = false;
                    newFilled &= combination;
                    newEmpty &= ~combination;
                }
            }
            if (isContradicting) throw new ContradictingStateException();
            return (newFilled, newEmpty);
        }

        static void SolvePicture (List<int>[] rows, List<int>[] columns, TextWriter writer) {
            Stopwatch st = Stopwatch.StartNew();
            int[, ] picture = new int[columns.Length, rows.Length];
            Console.CursorVisible = false;

            void DrawPicture (TextWriter wr) {
                for (int y = 0; y < rows.Length; ++y) {
                    for (int x = 0; x < columns.Length; ++x) {
                        wr.Write (DrawingSymbols[picture[x, y]]);
                    }
                    wr.WriteLine ();
                }
            }

            Stack<PicrossState> stateStack = new Stack<PicrossState>();
            bool isDetermined = true;
            int fullHash, emptyHash;
            do {
                if (st.ElapsedMilliseconds >= 1000) {
                    Console.Clear();
                    DrawPicture(Console.Error);
                    st.Restart();
                }
                try
                {
                    bool anythingChanged = false;
                    for (int idx = 0; idx < columns.Length; ++idx) {
                        fullHash = 0;
                        emptyHash = 0;
                        for (int i = 0; i < rows.Length; ++i) {
                            fullHash = (fullHash << 1) | ((picture[idx, i] == 1)? 1 : 0);
                            emptyHash = (emptyHash << 1) | ((picture[idx, i] == 2)? 1 : 0);
                        }
                        var (newFullHash, newEmptyHash) = FillStep(fullHash, emptyHash, columns[idx], rows.Length);
                        if (fullHash == newFullHash && emptyHash == newEmptyHash) {
                            continue;
                        } else {
                            anythingChanged = true;
                        }
                        for (int i = rows.Length - 1; i >= 0;  --i) {
                            picture[idx, i] = (newFullHash % 2 == 1)? 1 : (newEmptyHash % 2 == 1)? 2 : 0;
                            newFullHash >>= 1;
                            newEmptyHash >>= 1;
                        }
                    }

                    for (int idx = 0; idx < rows.Length; ++idx) {
                        fullHash = 0;
                        emptyHash = 0;
                        for (int i = 0; i < columns.Length; ++i) {
                            fullHash = (fullHash << 1) | ((picture[i, idx] == 1)? 1 : 0);
                            emptyHash = (emptyHash << 1) | ((picture[i, idx] == 2)? 1 : 0);
                        }
                        var (newFullHash, newEmptyHash) = FillStep(fullHash, emptyHash, rows[idx], columns.Length);
                        if (fullHash == newFullHash && emptyHash == newEmptyHash) {
                            continue;
                        } else {
                            anythingChanged = true;
                        }
                        for (int i = columns.Length - 1; i >= 0;  --i) {
                            picture[i, idx] = (newFullHash % 2 == 1)? 1 : (newEmptyHash % 2 == 1)? 2 : 0;
                            newFullHash >>= 1;
                            newEmptyHash >>= 1;
                        }
                    }

                    isDetermined = true;
                    foreach (var px in picture) {
                        isDetermined = isDetermined && (px != 0);
                    }

                    if (! anythingChanged) {
                        //TODO?: better candidate selection
                        for (int x = 0; x < columns.Length; ++x) {
                            for (int y = 0; y < rows.Length; ++y) {
                                if (picture[x,y] == 0) {
                                    var nv = new PicrossState {
                                        Picture = new int[columns.Length, rows.Length],
                                        ChangedPixel = (x, y)
                                    };
                                    Array.Copy(picture, nv.Picture, picture.Length);
                                    stateStack.Push(nv);
                                    picture[x,y] = 1;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (ContradictingStateException)
                {
                    PicrossState lastState = stateStack.Pop();
                    picture = lastState.Picture;
                    picture[lastState.ChangedPixel.X, lastState.ChangedPixel.Y] = 2;
                    // Console.Clear();
                    // DrawPicture(Console.Error);
                }
            } while (!isDetermined);
            // Console.Clear();
            DrawPicture(Console.Error);
            Console.CursorVisible = true;
            DrawPicture(writer);
        }

        static void SolveLongPicture (List<int>[] rows, List<int>[] columns, TextWriter writer) {
            int[, ] picture = new int[columns.Length, rows.Length];
            Console.CursorVisible = false;

            void DrawPicture (TextWriter wr) {
                for (int y = 0; y < rows.Length; ++y) {
                    for (int x = 0; x < columns.Length; ++x) {
                        wr.Write (DrawingSymbols[picture[x, y]]);
                    }
                    wr.WriteLine ();
                }
            }

            Stack<PicrossState> stateStack = new Stack<PicrossState>();
            bool isDetermined = true;
            long fullHash, emptyHash;
            do {
                try
                {
                    bool anythingChanged = false;
                    for (int idx = 0; idx < columns.Length; ++idx) {
                        fullHash = 0L;
                        emptyHash = 0L;
                        for (int i = 0; i < rows.Length; ++i) {
                            fullHash = (fullHash << 1) | ((picture[idx, i] == 1)? 1L : 0L);
                            emptyHash = (emptyHash << 1) | ((picture[idx, i] == 2)? 1L : 0L);
                        }
                        var (newFullHash, newEmptyHash) = FillStep(fullHash, emptyHash, columns[idx], rows.Length);
                        if (fullHash == newFullHash && emptyHash == newEmptyHash) {
                            continue;
                        } else {
                            anythingChanged = true;
                        }
                        for (int i = rows.Length - 1; i >= 0;  --i) {
                            picture[idx, i] = (newFullHash % 2 == 1)? 1 : (newEmptyHash % 2 == 1)? 2 : 0;
                            newFullHash >>= 1;
                            newEmptyHash >>= 1;
                        }
                    }

                    for (int idx = 0; idx < rows.Length; ++idx) {
                        fullHash = 0L;
                        emptyHash = 0L;
                        for (int i = 0; i < columns.Length; ++i) {
                            fullHash = (fullHash << 1) | ((picture[i, idx] == 1)? 1L : 0L);
                            emptyHash = (emptyHash << 1) | ((picture[i, idx] == 2)? 1L : 0L);
                        }
                        var (newFullHash, newEmptyHash) = FillStep(fullHash, emptyHash, rows[idx], columns.Length);
                        if (fullHash == newFullHash && emptyHash == newEmptyHash) {
                            continue;
                        } else {
                            anythingChanged = true;
                        }
                        for (int i = columns.Length - 1; i >= 0;  --i) {
                            picture[i, idx] = (newFullHash % 2 == 1)? 1 : (newEmptyHash % 2 == 1)? 2 : 0;
                            newFullHash >>= 1;
                            newEmptyHash >>= 1;
                        }
                    }

                    isDetermined = true;
                    foreach (var px in picture) {
                        isDetermined = isDetermined && (px != 0);
                    }

                    if (! anythingChanged) {
                        //TODO?: better candidate selection
                        for (int x = 0; x < columns.Length; ++x) {
                            for (int y = 0; y < rows.Length; ++y) {
                                if (picture[x,y] == 0) {
                                    var nv = new PicrossState {
                                        Picture = new int[columns.Length, rows.Length],
                                        ChangedPixel = (x, y)
                                    };
                                    Array.Copy(picture, nv.Picture, picture.Length);
                                    stateStack.Push(nv);
                                    picture[x,y] = 1;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (ContradictingStateException)
                {
                    PicrossState lastState = stateStack.Pop();
                    picture = lastState.Picture;
                    picture[lastState.ChangedPixel.X, lastState.ChangedPixel.Y] = 2;
                    // Console.Clear();
                    // DrawPicture(Console.Error);
                }
            } while (!isDetermined);
            // Console.Clear();
            DrawPicture(Console.Error);
            Console.CursorVisible = true;
            DrawPicture(writer);
        }

        static void SolveBigPicture (List<int>[] rows, List<int>[] columns, TextWriter writer) {
            int[, ] picture = new int[columns.Length, rows.Length];
            Console.CursorVisible = false;

            void DrawPicture (TextWriter wr) {
                for (int y = 0; y < rows.Length; ++y) {
                    for (int x = 0; x < columns.Length; ++x) {
                        wr.Write (DrawingSymbols[picture[x, y]]);
                    }
                    wr.WriteLine ();
                }
            }

            Stack<PicrossState> stateStack = new Stack<PicrossState>();
            bool isDetermined = true;
            BigInteger fullHash, emptyHash;
            do {
                try
                {
                    bool anythingChanged = false;
                    for (int idx = 0; idx < columns.Length; ++idx) {
                        fullHash = BigInteger.Zero;
                        emptyHash = BigInteger.Zero;
                        for (int i = 0; i < rows.Length; ++i) {
                            fullHash = (fullHash << 1) | ((picture[idx, i] == 1)? BigInteger.One : BigInteger.Zero);
                            emptyHash = (emptyHash << 1) | ((picture[idx, i] == 2)? BigInteger.One : BigInteger.Zero);
                        }
                        var (newFullHash, newEmptyHash) = FillStep(fullHash, emptyHash, columns[idx], rows.Length);
                        if (fullHash == newFullHash && emptyHash == newEmptyHash) {
                            continue;
                        } else {
                            anythingChanged = true;
                        }
                        for (int i = rows.Length - 1; i >= 0;  --i) {
                            picture[idx, i] = (newFullHash % 2 == 1)? 1 : (newEmptyHash % 2 == 1)? 2 : 0;
                            newFullHash >>= 1;
                            newEmptyHash >>= 1;
                        }
                    }

                    for (int idx = 0; idx < rows.Length; ++idx) {
                        fullHash = BigInteger.Zero;
                        emptyHash = BigInteger.Zero;
                        for (int i = 0; i < columns.Length; ++i) {
                            fullHash = (fullHash << 1) | ((picture[i, idx] == 1)? BigInteger.One : BigInteger.Zero);
                            emptyHash = (emptyHash << 1) | ((picture[i, idx] == 2)? BigInteger.One : BigInteger.Zero);
                        }
                        var (newFullHash, newEmptyHash) = FillStep(fullHash, emptyHash, rows[idx], columns.Length);
                        if (fullHash == newFullHash && emptyHash == newEmptyHash) {
                            continue;
                        } else {
                            anythingChanged = true;
                        }
                        for (int i = columns.Length - 1; i >= 0;  --i) {
                            picture[i, idx] = (newFullHash % 2 == 1)? 1 : (newEmptyHash % 2 == 1)? 2 : 0;
                            newFullHash >>= 1;
                            newEmptyHash >>= 1;
                        }
                    }

                    isDetermined = true;
                    foreach (var px in picture) {
                        isDetermined = isDetermined && (px != 0);
                    }

                    if (! anythingChanged) {
                        //TODO?: better candidate selection
                        for (int x = 0; x < columns.Length; ++x) {
                            for (int y = 0; y < rows.Length; ++y) {
                                if (picture[x,y] == 0) {
                                    var nv = new PicrossState {
                                        Picture = new int[columns.Length, rows.Length],
                                        ChangedPixel = (x, y)
                                    };
                                    Array.Copy(picture, nv.Picture, picture.Length);
                                    stateStack.Push(nv);
                                    picture[x,y] = 1;
                                    break;
                                }
                            }
                        }
                    }
                }
                catch (ContradictingStateException)
                {
                    PicrossState lastState = stateStack.Pop();
                    picture = lastState.Picture;
                    picture[lastState.ChangedPixel.X, lastState.ChangedPixel.Y] = 2;
                    // Console.Clear();
                    // DrawPicture(Console.Error);
                }
            } while (!isDetermined);
            // Console.Clear();
            DrawPicture(Console.Error);
            Console.CursorVisible = true;
            DrawPicture(writer);
        }

        public static void SolvePicture (TextReader reader, TextWriter writer) {
            int[] dimensions = reader.ReadLine().Split(' ').Select(x => int.Parse(x)).ToArray();
            List<int>[] rows = new List<int>[dimensions[0]];
            List<int>[] columns = new List<int>[dimensions[1]];

            for (int i = 0; i < dimensions[0]; ++i) {
                rows[i] = reader.ReadLine().Split(' ').Select(x => int.Parse(x)).ToList();
                if (rows[i].Count == 1 && rows[i][0] == 0) rows[i].Clear();
            }

            for (int i = 0; i < dimensions[1]; ++i) {
                columns[i] = reader.ReadLine().Split(' ').Select(x => int.Parse(x)).ToList();
                if (columns[i].Count == 1 && columns[i][0] == 0) columns[i].Clear();
            }

            if (rows.Length <= 32 && columns.Length <= 32)
                SolvePicture(rows, columns, writer);
            else if (rows.Length <= 64 && columns.Length <= 64)
                SolveLongPicture(rows, columns, writer);
            else
                SolveBigPicture(rows, columns, writer);
        }
    }

    class PicrossState {
        public int[,] Picture;
        public (int X, int Y) ChangedPixel;
    }

    class ContradictingStateException : Exception { }

    class IntegerSet {
        //TODO: implement custom integer set class
        public static int SET_SIZE = 10;

        private uint[] Values = new uint[SET_SIZE];

        public IntegerSet() {}

        public IntegerSet(int upperBound) {
            int currentIdx = 0;
            while(upperBound >= 32) {
                Values[currentIdx++] = uint.MaxValue;
            }
            Values[currentIdx] = (1U << upperBound) - 1;
        }

        public IntegerSet(int length, int shift) {
            //TODO: implement constructor alagous to (1111b << 5)
        }

        public static IntegerSet operator ^(IntegerSet a, IntegerSet b) {
            var res = new IntegerSet();
            for(int i = 0; i < SET_SIZE; ++i) {
                res.Values[i] = a.Values[i] ^ b.Values[i];
            }
            return res;
        }

        public static IntegerSet operator |(IntegerSet a, IntegerSet b) {
            var res = new IntegerSet();
            for(int i = 0; i < SET_SIZE; ++i) {
                res.Values[i] = a.Values[i] | b.Values[i];
            }
            return res;
        }

        public static IntegerSet operator &(IntegerSet a, IntegerSet b) {
            var res = new IntegerSet();
            for(int i = 0; i < SET_SIZE; ++i) {
                res.Values[i] = a.Values[i] & b.Values[i];
            }
            return res;
        }

        public bool this[int i] {
            get {
                int currentIdx = 0;
                while(i >= 32) {
                    i -= 32;
                    ++currentIdx;
                }
                return (Values[currentIdx] & ((1U << i) - 1)) != 0;
            }
        }
    }
}
