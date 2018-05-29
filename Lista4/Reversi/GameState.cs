using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Reversi {
    enum Piece {
        Empty = 0,
        White = 1,
        Black = 2,
    }

    class GameState {
        public Piece[,] Board;
        
        public Piece CurrentPlayer;

        public static Size[] Directions = new Size[8] {
            new Size(0, -1),
            new Size(1, -1),
            new Size(1, 0),
            new Size(1, 1),
            new Size(0, 1),
            new Size(-1, 1),
            new Size(-1, 0),
            new Size(-1, -1)
        };

        public int WhiteScore {get; private set;}

        public int BlackScore {get; private set;}

        public static bool IsInBoard(Point position) => position.X < 8 && position.X >= 0 && position.Y < 8 && position.Y >= 0;

        public static bool IsValid(GameState state, Point position) {
            if (state.Board[position.X, position.Y] != Piece.Empty) return false;
            Piece opponentColor = 3 - state.CurrentPlayer;
            foreach(Size dir in Directions){
                Point np = position + dir;
                if (!IsInBoard(np) || state.Board[np.X, np.Y] != opponentColor) continue;
                np += dir;
                while (IsInBoard(np)) {
                    if (state.Board[np.X, np.Y] == Piece.Empty) {
                        break;
                    } else if (state.Board[np.X, np.Y] == state.CurrentPlayer) {
                        return true;
                    }
                    np += dir;
                }
            }
            return false;
        }

        internal void PrintBoard(List<Point> hilighted = null)
        {
            if (hilighted == null) hilighted = new List<Point>();
            Dictionary<Piece, string> symbols = new Dictionary<Piece, string> {
                {Piece.Empty, " ."},
                {Piece.White, " O"},
                {Piece.Black, " X"}
            };

            Console.Error.WriteLine("  a b c d e f g h");
            for(int y = 0; y < 8; y++) {
                Console.Error.Write(y+1);
                for (int x = 0; x < 8; x++) {
                    if (hilighted.Any(p => p.X == x && p.Y == y)) {
                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                        Console.Error.Write(" +"); 
                    } else if (Board[x,y] == Piece.Empty) {
                        Console.ForegroundColor = ((x + y) % 2 == 0) ? ConsoleColor.DarkGreen : ConsoleColor.DarkBlue;
                        Console.Error.Write(" .");
                    } else if (Board[x,y] == Piece.White) {
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Error.Write(" \x25cf");
                    } else if (Board[x,y] == Piece.Black) {
                        Console.ForegroundColor = ConsoleColor.DarkRed;
                        Console.Error.Write(" \x25cf");
                    }
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
                Console.Error.WriteLine(" " + (y + 1));
            }
            Console.Error.WriteLine("  a b c d e f g h");
        }

        public List<Point> PossibleMoves() {
            List<Point> res = new List<Point>();
            for(int x = 0; x < 8; x++) {
                for(int y = 0; y < 8; y++) {
                    Point candidate = new Point(x, y);
                    if (IsValid(this, candidate)) res.Add(candidate);
                }
            }
            return res;
        }

        public GameState AddPiece(Point position) {
            GameState res = new GameState();
            res.Board = new Piece[8, 8];
            Array.Copy(Board, res.Board, Board.Length);
            res.CurrentPlayer = 3 - CurrentPlayer;
            List<Point> changed = new List<Point>();

            foreach(Size dir in Directions){
                Point np = position + dir;
                changed.Clear();
                while (IsInBoard(np)) {
                    if (Board[np.X, np.Y] == Piece.Empty) {
                        break;
                    } else if (Board[np.X, np.Y] == CurrentPlayer) {
                        foreach(var p in changed) {
                            res.Board[p.X, p.Y] = CurrentPlayer;
                        }
                        break;
                    }
                    changed.Add(np);
                    np += dir;
                }
            }
            res.Board[position.X, position.Y] = CurrentPlayer;
            for (int x = 0; x < 8; ++x) {
                for (int y = 0; y < 8; ++y) {
                    if (res.Board[x,y] == Piece.White) ++res.WhiteScore;
                    else if (res.Board[x,y] == Piece.Black) ++res.BlackScore;
                }
            }
            return res;
        }

        public static GameState StartState() {
            GameState res = new GameState {
                WhiteScore = 2,
                BlackScore = 2
            };
            res.CurrentPlayer = Piece.Black;
            res.Board = new Piece[8, 8];

            res.Board[3, 3] = Piece.White;
            res.Board[4, 3] = Piece.Black;
            res.Board[3, 4] = Piece.Black;
            res.Board[4, 4] = Piece.White;

            return res;
        }
    }
}