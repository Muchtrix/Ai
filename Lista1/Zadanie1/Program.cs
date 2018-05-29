using System;
using System.Collections.Generic;

namespace Reversi
{
    class Program
    {
        static void Main(string[] args)
        {
            bool debug = args.Length > 0 && args[0] == "--debug";
            Console.Error.WriteLine("Program ready!");
            string line;
            while((line = Console.ReadLine()) != null){
                if (debug) {
                    ChessHelper.Debug(line);
                } else {
                    Console.WriteLine(ChessHelper.Checkmate(ChessHelper.ParseLine(line)).Count - 1);
                }
            }
        }
    }

    static class ChessHelper {

        public static Gamestate ParseLine(string line){
            string[] parts = line.Split(' ');
            return new Gamestate{
                BlackMove = parts[0] == "black",
                WhiteKing = (parts[1][0] - 'a', parts[1][1] - '1'),
                WhiteRook = (parts[2][0] - 'a', parts[2][1] - '1'),
                BlackKing = (parts[3][0] - 'a', parts[3][1] - '1')
            };
        }

        private static bool IsCheck(Gamestate state){
            return state.BlackKing.X == state.WhiteRook.X 
                || state.BlackKing.Y == state.WhiteRook.Y 
                || (Math.Abs(state.BlackKing.X - state.WhiteKing.X) <= 1 && Math.Abs(state.BlackKing.Y - state.WhiteKing.Y) <= 1);
        }

        private static bool IsInvalid(Gamestate state){
            return (state.BlackKing.X == state.WhiteKing.X && state.BlackKing.Y == state.WhiteKing.Y)
                || (state.BlackKing.X == state.WhiteRook.X && state.BlackKing.Y == state.WhiteRook.Y)
                || (state.WhiteKing.X == state.WhiteRook.X && state.WhiteKing.Y == state.WhiteRook.Y);
        }

        private static bool IsInvalidAfterRook(Gamestate state) {
            return IsInvalid(state) || (Math.Abs(state.BlackKing.X - state.WhiteRook.X) <= 1 && Math.Abs(state.BlackKing.Y - state.WhiteRook.Y) <= 1);
        }

        private static bool IsInBoard(int pos) => 0 <= pos && pos <= 7;

        private static IEnumerable<int> ValidMoves(Gamestate state){
            if(state.BlackMove){
                state.BlackMove = false;
                for (int dx = -1; dx <= 1; ++dx){
                    for (int dy = -1; dy <= 1; ++dy){
                        if (dx == dy && dx == 0) continue;
                        if (IsInBoard(state.BlackKing.X + dx) && IsInBoard(state.BlackKing.Y + dy)){
                            state.BlackKing.X += dx;
                            state.BlackKing.Y += dy;
                            if (!IsInvalid(state) && !IsCheck(state)) yield return state.Hash();
                            state.BlackKing.X -= dx;
                            state.BlackKing.Y -= dy;
                        }
                    }
                }
            } else {
                state.BlackMove = true;
                
                // White king move
                for (int dx = -1; dx <= 1; ++dx){
                    for (int dy = -1; dy <= 1; ++dy){
                        if (dx == dy && dx == 0) continue;
                        if (IsInBoard(state.WhiteKing.X + dx) && IsInBoard(state.WhiteKing.Y + dy)){
                            state.WhiteKing.X += dx;
                            state.WhiteKing.Y += dy;
                            if (!IsInvalid(state) && !IsCheck(state)) yield return state.Hash();
                            state.WhiteKing.X -= dx;
                            state.WhiteKing.Y -= dy;
                        }
                    }
                }

                // White rook move
                for (int d = 1; d < 8; ++d){
                    state.WhiteRook.X += d;
                    if (!IsInBoard(state.WhiteRook.X) || IsInvalidAfterRook(state)){
                        state.WhiteRook.X -= d;
                        break;
                    }
                    yield return state.Hash();
                    state.WhiteRook.X -= d;
                }
                for (int d = 1; d < 8; ++d){
                    state.WhiteRook.Y += d;
                    if (!IsInBoard(state.WhiteRook.Y) || IsInvalidAfterRook(state)){
                        state.WhiteRook.Y -= d;
                        break;
                    }
                    yield return state.Hash();
                    state.WhiteRook.Y -= d;
                }

                for (int d = 1; d < 8; ++d){
                    state.WhiteRook.X -= d;
                    if (!IsInBoard(state.WhiteRook.X) || IsInvalidAfterRook(state)){
                        state.WhiteRook.X += d;
                        break;
                    }
                    yield return state.Hash();
                    state.WhiteRook.X += d;
                }
                for (int d = 1; d < 8; ++d){
                    state.WhiteRook.Y -= d;
                    if (!IsInBoard(state.WhiteRook.Y) || IsInvalidAfterRook(state)){
                        state.WhiteRook.Y += d;
                        break;
                    }
                    yield return state.Hash();
                    state.WhiteRook.Y += d;
                }
            }
        }

        public static List<int> Checkmate(Gamestate state){
            int initialHash = state.Hash();
            Queue<(int move, int depth)> nextMoves = new Queue<(int, int)>();
            Dictionary<int, int> previousMove = new Dictionary<int, int>();
            previousMove[initialHash] = initialHash;
            nextMoves.Enqueue((initialHash, 0));

            int checkmateState;

            while(true){
                (int currentHash, int depth) = nextMoves.Dequeue();
                //Console.WriteLine($"Current depth: {depth}");
                Gamestate current = Gamestate.UnHash(currentHash);
                bool canMove = false;
                foreach(int next in ValidMoves(current)){
                    canMove = true;
                    if(!previousMove.ContainsKey(next)){
                        nextMoves.Enqueue((next, depth + 1));
                        previousMove[next] = currentHash;
                    }
                }
                if (current.BlackMove && IsCheck(current) && !canMove) {
                    checkmateState = currentHash;
                    break;
                }
            }

            List<int> result = new List<int>();
            result.Add(checkmateState);
            while(checkmateState != initialHash){
                checkmateState = previousMove[checkmateState];
                result.Add(checkmateState);
            }
            result.Reverse();
            return result;
        }

        public static void Debug(string line){
            List<int> states = Checkmate(ParseLine(line));
            foreach(var state in states){
                Gamestate.UnHash(state).PrettyPrint();
                //Console.WriteLine(Gamestate.UnHash(state).ToString());
            }
        }
    }

    struct Gamestate {
        public (int X, int Y) WhiteKing;
        public (int X, int Y) WhiteRook;
        public (int X, int Y) BlackKing;
        public bool BlackMove;

        public int Hash(){
            int res = WhiteKing.X;
            res = res<<3 | WhiteKing.Y;
            res = res<<3 | WhiteRook.X;
            res = res<<3 | WhiteRook.Y;
            res = res<<3 | BlackKing.X;
            res = res<<3 | BlackKing.Y;
            res = res<<1 | (BlackMove? 1 : 0);
            return res;
        }

        public override string ToString() => $"{(BlackMove ? "black" : "white")} {(char)('a' + WhiteKing.X)}{(char)('1' + WhiteKing.Y)} {(char)('a' + WhiteRook.X)}{(char)('1' + WhiteRook.Y)} {(char)('a' + BlackKing.X)}{(char)('1' + BlackKing.Y)}";

        public static Gamestate UnHash(int hash){
            Gamestate res = new Gamestate();
            res.BlackMove = hash % 2 == 1;
            hash >>= 1;
            res.BlackKing.Y = hash % 8;
            hash >>= 3;
            res.BlackKing.X = hash % 8;
            hash >>= 3;
            res.WhiteRook.Y = hash % 8;
            hash >>= 3;
            res.WhiteRook.X = hash % 8;
            hash >>= 3;
            res.WhiteKing.Y = hash % 8;
            hash >>= 3;
            res.WhiteKing.X = hash % 8;
            return res;
        }

        public void PrettyPrint() {
            char[,] board = new char[8,8];
            for(int i = 0; i < 8; ++i)
                for(int j = 0; j < 8; ++j)
                    board[i,j] = ' ';
            
            board[BlackKing.X, BlackKing.Y] = '\x265A';
            board[WhiteKing.X, WhiteKing.Y] = '\x2654';
            board[WhiteRook.X, WhiteRook.Y] = '\x2656';

            Console.WriteLine("  +-+-+-+-+-+-+-+-+");
            for(int y = 7; y >=0; --y) {
                Console.Write($"{1 + y} |");
                for(int x = 0; x <= 7; ++x) {
                    Console.Write($"{board[x,y]}|");
                }
                Console.WriteLine();
                Console.WriteLine("  +-+-+-+-+-+-+-+-+");
            }
            Console.WriteLine("   a b c d e f g h");
            Console.WriteLine();
        }
    }
}
