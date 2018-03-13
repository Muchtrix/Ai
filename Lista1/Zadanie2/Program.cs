using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Zadanie2
{
    class Program
    {
        static void Main(string[] args)
        {
            bool random = args.Length >= 1 && args[1] == "--random";
            Stopwatch st = Stopwatch.StartNew();
            WordSplitter ws = new WordSplitter();
            ws.ReadDict(args[0]);
            string line;
            while((line = Console.ReadLine()) != null) {
                if (random) {
                    Console.WriteLine(ws.SplitLineRandom(line));
                } else {
                    Console.WriteLine(ws.SplitLine(line));
                }
            }
            Console.Error.WriteLine($"Running time: {st.Elapsed}");
        }
    }

    class WordSplitter {
        private HashSet<string> Dict = new HashSet<string>();

        private Random RNG = new Random();

        private string[] TempStrings;

        private int[] TempSums;

        public void ReadDict(string filePath) {
            Dict.Clear();
            using (StreamReader sr = new StreamReader(filePath)) {
                string word = null;
                while((word = sr.ReadLine()) != null) {
                    Dict.Add(word);
                }
            }
        }

        private void SplitLineDynamic(string line) {
            if (Dict.Contains(line)) {
                TempStrings[line.Length] = line;
                TempSums[line.Length] = line.Length * line.Length;
                return;
            }
            int maxSum = 0;
            string maxStr = "";

            for(int i = line.Length - 1; i > 0; --i) {
                if (Dict.Contains(line.Substring(0, i))) {
                    if (TempStrings[line.Length - i] == null) {
                        SplitLineDynamic(line.Substring(i));
                    }
                    if (i * i + TempSums[line.Length - i] > maxSum) {
                        maxSum = i * i + TempSums[line.Length - i];
                        maxStr = line.Substring(0,i) + " " + TempStrings[line.Length - i];
                    }
                }
            }

            TempSums[line.Length] = maxSum;
            TempStrings[line.Length] = maxStr;
        }

        public string SplitLineRandom(string line) {
            string origLine = line;
            List<string> partition = new List<string>();
            int iterationCount = 0;
            while(!string.IsNullOrEmpty(line)) {
                if (Dict.Contains(line)) {
                    partition.Add(line);
                    break;
                }
                ++iterationCount;
                if (iterationCount > 100) {
                    Console.Error.WriteLine(string.Join(' ', partition));
                    line = origLine;
                    partition.Clear();
                    iterationCount = 0;
                }
                int len = RNG.Next(1, line.Length);
                if (Dict.Contains(line.Substring(0, len))) {
                    partition.Add(line.Substring(0, len));
                    line = line.Substring(len);
                }
            }
            return string.Join(' ', partition);
        }

        public string SplitLine(string line) {
            TempStrings = new string[line.Length + 1];
            TempSums = new int[line.Length + 1];
            SplitLineDynamic(line);
            return TempStrings[line.Length];
        }
    }
}
