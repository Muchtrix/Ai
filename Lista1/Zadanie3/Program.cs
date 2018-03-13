using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace Zadanie3
{
    class Program
    {
        static void Main(string[] args)
        {
            //PokerHelper.TestHands();

            PokerHelper.SimulateGamesDeterministic(PokerHelper.LowDeck(), PokerHelper.HighDeck());

            // var highDeck = PokerHelper.HighDeck();

            // Console.WriteLine("Low vs High(100k hands)");
            // PokerHelper.SimulateGames(PokerHelper.LowDeck(), highDeck, 100000);
            // Console.WriteLine("\nHigh vs High (100k hands)");
            // PokerHelper.SimulateGames(highDeck, highDeck, 100000);
            // Console.WriteLine("\nSingle color vs High (100k hands)");
            // PokerHelper.SimulateGames(PokerHelper.LowSingleColorDeck(), highDeck, 100000);
            // Console.WriteLine("\nTwo colors vs High (100k hands)");
            // PokerHelper.SimulateGames(PokerHelper.LowDoubleColorDeck(), highDeck, 100000);
        }
    }

    enum CardColor{
        Spades,
        Clubs,
        Diamonds,
        Hearts
    }

    class Card {
        public int Value;
        public CardColor Color;

        public override string ToString() => $"{Value} of {Color}";
    }

    static class PokerHelper {

        static Random Rng = new Random();

        static bool IsFlush(IEnumerable<Card> hand){
            return hand.GroupBy(x => x.Color).Max(x => x.Count()) == 5;
        }

        static bool IsStraight(IEnumerable<Card> hand){
            var sorted = hand.OrderBy(x => x.Value);
            bool normalRising = sorted.Select((x, i) => x.Value - i).GroupBy(x => x).Max(x => x.Count()) == 5;
            // bool aceRising = sorted.First().Value == 1 
            //     && sorted.Skip(1).Append(new Card{Value=14, Color=CardColor.Clubs})
            //        .Select((x, i) => x.Value - i)
            //        .GroupBy(x => x).First().Count() == 5;
            return normalRising;// || aceRising;
        }

        static bool IsFourOfAKind(IEnumerable<Card> hand){
            return hand.GroupBy(x => x.Value).Max(x => x.Count()) == 4;
        }

        static bool IsFullHouse(IEnumerable<Card> hand){
            var grouped = hand.GroupBy(x => x.Value).OrderByDescending(x => x.Count()).ToArray();
            return grouped[0].Count() == 3 && grouped[1].Count() == 2;
        }

        static bool IsThreeOfAKind(IEnumerable<Card> hand){
            return hand.GroupBy(x => x.Value).Max(x => x.Count()) == 3;
        }

        static bool IsTwoPair(IEnumerable<Card> hand){
            var grouped = hand.GroupBy(x => x.Value).OrderByDescending(x => x.Count()).ToArray();
            return grouped[0].Count() == 2 && grouped[1].Count() == 2;
        }

        static bool IsPair(IEnumerable<Card> hand){
            return hand.GroupBy(x => x.Value).Max(x => x.Count()) == 2;
        }

        public static int HandScore(IEnumerable<Card> hand){
            if (hand.Count() != 5) return 0;
            bool straight = IsStraight(hand);
            bool flush = IsFlush(hand);
            if (straight && flush) return 9;
            if (IsFourOfAKind(hand)) return 8;
            if (IsFullHouse(hand)) return 7;
            if (flush) return 6;
            if (straight) return 5;
            if (IsThreeOfAKind(hand)) return 4;
            if (IsTwoPair(hand)) return 3;
            if (IsPair(hand)) return 2;
            return 1; // High card
        }

        public static Card[] HighDeck() {
            Card[] res = new Card[16];
            int counter = 0;
            for(int i = 11; i <= 14; ++i){
                foreach(CardColor color in Enum.GetValues(typeof(CardColor))){
                    res[counter++] = new Card{Value=i, Color=color};
                }
            }
            // foreach(CardColor color in Enum.GetValues(typeof(CardColor))){
            //     res[counter++] = new Card{Value=1, Color=color};
            // }
            return res;
        }

        public static Card[] LowDeck() {
            Card[] res = new Card[36];
            int counter = 0;
            for(int i = 2; i <= 10; ++i){
                foreach(CardColor color in Enum.GetValues(typeof(CardColor))){
                    res[counter++] = new Card{Value=i, Color=color};
                }
            }
            return res;
        }

        public static Card[] LowSingleColorDeck() {
            Card[] res = new Card[9];
            for(int i = 2; i <= 10; ++i){
                res[i-2] = new Card{Value=i, Color=CardColor.Clubs};
            }
            return res;
        }

        public static Card[] LowDoubleColorDeck() {
            Card[] res = new Card[18];
            int counter = 0;
            for(int i = 2; i <= 10; ++i){
                res[counter++] = new Card{Value=i, Color=CardColor.Clubs};
                res[counter++] = new Card{Value=i, Color=CardColor.Hearts};
            }
            return res;
        }

        public static IEnumerable<Card> RandomHand(IEnumerable<Card> deck) => deck.OrderBy(x => Rng.Next()).Take(5).ToArray();

        // public static IEnumerable<Card> RandomHand(Card[] deck) {
        //     double cardsLeft = 5.0;
        //     for(int i = 0; i < deck.Length; ++i) {
        //         if (Rng.NextDouble() <= cardsLeft/(deck.Length - i)) {
        //             yield return deck[i];
        //             cardsLeft--;
        //         }
        //         if (cardsLeft == 0.0) break;
        //     }
        // }

        public static void SimulateGames(Card[] playerDeck, Card[] opponentDeck, int noOfHands) {
            Stopwatch st = Stopwatch.StartNew();
            double winCount = 0;
            double loseCount = 0;
            for(int i = 1; i <= noOfHands; ++i){
                Console.Write($"\rPlaying: {(int)(i*100.0 / noOfHands)}%");
                if (IsHighPlayerBetter(RandomHand(opponentDeck), RandomHand(playerDeck))){
                    ++loseCount;
                } else {
                    ++winCount;
                }
            }
            Console.WriteLine($"\rWin ratio: {winCount / (loseCount + winCount) * 100}%");
            //Console.WriteLine($"\r\t\tWins: {winCount}");
            Console.WriteLine($"Running time: {st.Elapsed}");
        }
        static bool IsHighPlayerBetter(IEnumerable<Card> highHand, IEnumerable<Card> lowHand) => HandScore(highHand) >= HandScore(lowHand);

        // static IEnumerable<IEnumerable<Card>> GenerateAllHands(Card[] deck) {
        //     bool PopEq5(long n) {
        //         int c = 0;
        //         while(n > 0) {
        //             ++c;
        //             n &= n-1;
        //         }
        //         return c == 5;
        //     }
        //     long maxVal = (1L << (deck.Length)) - 1;
        //     for(long i = 0; i < maxVal; ++i) {
        //         if (PopEq5(i)){
        //             List<Card> res = new List<Card>();
        //             for(int k = 0; k< deck.Length; ++k) {
        //                 if ((i & (1 << k)) != 0) res.Add(deck[k]);
        //             }
        //             yield return res;
        //         }
        //     }
        // }

        static IEnumerable<IEnumerable<Card>> GenerateAllHands(Card[] deck) => GetPermutations(deck, 5);

        static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items, int count)
        {
            int i = 0;
            foreach (var item in items)
            {
                if (count == 1)
                    yield return new T[] { item };
                else
                {
                    foreach (var result in GetPermutations(items.Skip(i + 1), count - 1))
                        yield return new T[] { item }.Concat(result);
                }

                ++i;
            }
        }

        public static void SimulateGamesDeterministic(Card[] playerDeck, Card[] opponentDeck) {
            Stopwatch st = Stopwatch.StartNew();
            int[] lowHandsScores = new int[10];
            int[] highHandScores = new int[10];
            foreach(var scoreGroup in GenerateAllHands(playerDeck).Select(x => HandScore(x)).GroupBy(x => x)) {
                lowHandsScores[scoreGroup.Key] = scoreGroup.Count();
            }
            foreach(var scoreGroup in GenerateAllHands(opponentDeck).Select(x => HandScore(x)).GroupBy(x => x)) {
                highHandScores[scoreGroup.Key] = scoreGroup.Count();
            }
            long lowWins = 0;
            long highWins = 0;

            for(int i = 1; i <= 9; ++i) {
                for(int j = 1; j <= 9; ++j) {
                    if (i < j) { // przegrana figuranta
                        lowWins += lowHandsScores[j] * highHandScores[i];
                    } else {
                        highWins += lowHandsScores[j] * highHandScores[i];
                    }
                }
            }

            // for(int i = 1; i <= noOfHands; ++i){
            //     Console.Write($"\rPlaying: {(int)(i*100.0 / noOfHands)}%");
            //     if (!IsHighPlayerBetter(RandomHand(opponentDeck), RandomHand(playerDeck))){
            //         ++winCount;
            //     }
            // }
            Console.WriteLine($"\rWin ratio: {((double)lowWins / (lowWins + highWins)):0.0%}");
            //Console.WriteLine($"\r\t\tWins: {winCount}");
            Console.WriteLine($"Running time: {st.Elapsed}");
        }

        public static void TestHands() {
            Dictionary<string, int> values = new Dictionary<string, int>{
                {"2", 2},
                {"3", 3},
                {"4", 4},
                {"5", 5},
                {"6", 6},
                {"7", 7},
                {"8", 8},
                {"9", 9},
                {"10", 10},
                {"J", 11},
                {"Q", 12},
                {"K", 13},
                {"A", 14}
            };
            Dictionary<char,CardColor> colors = new Dictionary<char, CardColor>{
                {'♣', CardColor.Clubs},
                {'♦', CardColor.Diamonds},
                {'♥', CardColor.Hearts},
                {'♠', CardColor.Spades}
            };
            string[] handNames = new string[] {
                "BŁAD",
                "Wysoka karta",
                "Para",
                "Dwie pary",
                "Trójka",
                "Strit",
                "Kolor",
                "Full",
                "Kareta",
                "Poker"
            };
            int[,] hands = new int[2,10];
            int blotWins = 0;
            int highWins = 0;
            string line;
            while((line = Console.ReadLine()) != null) {
                var cards = line.Split(';').Take(10).Select(x=> x.Trim()).Select(x => new Card{Value = values[x.Substring(0, x.Length-1)], Color = colors[x[x.Length - 1]]});
                var highHand = cards.Take(5);
                var blotHand = cards.Skip(5);
                ++hands[0, HandScore(highHand)];
                ++hands[1, HandScore(blotHand)];
                if (IsHighPlayerBetter(highHand, blotHand)){
                    ++highWins;
                } else {
                    ++blotWins;
                }
            }

            Console.WriteLine($"Blotkarz: {blotWins} {blotWins*100.0 / (blotWins + highWins)}%");
            Console.WriteLine($"Figurant: {highWins} {highWins*100.0 / (blotWins + highWins)}%");
            Console.WriteLine();

            for(int i = 0; i < 2; ++i) {
                for (int j = 1; j < 10; ++j) {
                    Console.WriteLine($"{handNames[j]} - {hands[i,j]}");
                }
                Console.WriteLine();
            }
        }

        public static int Next(this RNGCryptoServiceProvider rng) {
            byte[] bytes = new byte[4];
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);
        }
    }
}
