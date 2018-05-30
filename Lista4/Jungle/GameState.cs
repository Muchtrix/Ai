using System;
using System.Collections.Generic;

namespace Jungle
{
    enum Piece {
        Rat,
        Cat,
        Dog,
        Wolf,
        Panther,
        Tiger,
        Lion,
        Elephant
    }

    enum Field {
        Grass,
        Water,
        Trap,
        Cave
    }

    enum Direction {
        Up,
        Down,
        Left,
        Right
    }

    enum Player {
        Down,
        Up
    }

    struct Point {
        public int X;
        public int Y;

        public override int GetHashCode() => (X << 16) | Y;

        public override bool Equals(object obj) => obj is Point && ((Point)obj).GetHashCode() == GetHashCode();

        public Point Move(Direction d) {
            switch (d)
            {
                case Direction.Up: return new Point{X = X, Y = Y - 1};
                case Direction.Down: return new Point{X = X, Y = Y + 1};
                case Direction.Left: return new Point{X = X - 1, Y = Y};
                default: return new Point{X = X + 1, Y = Y};
            }
        }

        public int ManhattanDistance(Point d) => Math.Abs(X - d.X) + Math.Abs(Y - d.Y);

        public bool InBounds => 0 <= X && X <= 6 && 0 <= Y && Y <= 8;
    }

    class GameState {
        public static Field[,] Board = new[,] {
            {Field.Grass, Field.Grass, Field.Grass, Field.Grass, Field.Grass, Field.Grass, Field.Grass, Field.Grass, Field.Grass},
            {Field.Grass, Field.Grass, Field.Grass, Field.Water, Field.Water, Field.Water, Field.Grass, Field.Grass, Field.Grass},
            {Field.Trap,  Field.Grass, Field.Grass, Field.Water, Field.Water, Field.Water, Field.Grass, Field.Grass, Field.Trap },
            {Field.Cave,  Field.Trap,  Field.Grass, Field.Grass, Field.Grass, Field.Grass, Field.Grass, Field.Trap,  Field.Cave },
            {Field.Trap,  Field.Grass, Field.Grass, Field.Water, Field.Water, Field.Water, Field.Grass, Field.Grass, Field.Trap },
            {Field.Grass, Field.Grass, Field.Grass, Field.Water, Field.Water, Field.Water, Field.Grass, Field.Grass, Field.Grass},
            {Field.Grass, Field.Grass, Field.Grass, Field.Grass, Field.Grass, Field.Grass, Field.Grass, Field.Grass, Field.Grass}};

        public static Direction[] Directions = new[] {Direction.Up, Direction.Down, Direction.Left, Direction.Right};


        public Dictionary<Point, Piece> UpPlayer;
        public Dictionary<Point, Piece> DownPlayer;

        public int HitCounter;

        public Player CurrentPlayer;

        public List<(Point, Direction)> PossibleMoves() {
            List<(Point, Direction)> res = new List<(Point, Direction)>();
            foreach(Point p in (CurrentPlayer == Player.Up)? UpPlayer.Keys : DownPlayer.Keys) {
                foreach (var d in Directions) {
                    if (IsPossible(p, d)) {
                        res.Add((p, d));
                    }
                }
            }

            return res;
        }

        public GameState ApplyMove((Point position, Direction d) move) {
            var res = new GameState{
                UpPlayer = new Dictionary<Point, Piece>(this.UpPlayer),
                DownPlayer = new Dictionary<Point, Piece>(this.DownPlayer),
                CurrentPlayer = (this.CurrentPlayer == Player.Up) ? Player.Down : Player.Up,
                HitCounter = this.HitCounter + 1
            };
            var our = (this.CurrentPlayer == Player.Up) ? res.UpPlayer : res.DownPlayer;
            var ene = (this.CurrentPlayer == Player.Up) ? res.DownPlayer : res.UpPlayer;
            Point npos = MovePiece(move.position, move.d).Value;
            if (ene.ContainsKey(npos)) {
                ene.Remove(npos);
                res.HitCounter = 0;
            }
            our[npos] = our[move.position];
            our.Remove(move.position);
            return res;
        }

        private Point? MovePiece(Point pos, Direction d)
        {
            var our = (CurrentPlayer == Player.Up) ? UpPlayer : DownPlayer;
            var ene = (CurrentPlayer == Player.Up) ? DownPlayer : UpPlayer;

            Point npos = pos.Move(d);

            if (!npos.InBounds) return null;

            if (npos.X == 3 && npos.Y == 0 && CurrentPlayer == Player.Up) return null;
            if (npos.X == 3 && npos.Y == 8 && CurrentPlayer == Player.Down) return null;

            if (Board[npos.X, npos.Y] == Field.Water) {
                if (our[pos] == Piece.Tiger || our[pos] == Piece.Lion) {
                    do {
                        if (our.ContainsKey(npos) || ene.ContainsKey(npos)) return null;
                        npos = npos.Move(d);
                    } while (Board[npos.X, npos.Y] == Field.Water);
                } else if (our[pos] != Piece.Rat) return null;
            }

            if (our.ContainsKey(npos)) return null;
            if (ene.ContainsKey(npos) && !Defeats(pos, our[pos], npos, ene[npos])) return null;

            return npos;
        }

        private bool Defeats(Point p, Piece pp, Point pb, Piece ppb) {
            if (Board[pb.X, pb.Y]==Field.Trap) return true;
            if (pp == Piece.Rat) return (ppb == Piece.Elephant || ppb == Piece.Rat) && (Board[p.X, p.Y] != Field.Water || Board[pb.X, pb.Y] != Field.Grass);
            return pp <= ppb;
        }

        private bool IsPossible(Point position, Direction d) => MovePiece(position, d).HasValue;

        internal static GameState InitialState()
        {
            GameState res = new GameState{
                UpPlayer = new Dictionary<Point, Piece>(),
                DownPlayer = new Dictionary<Point, Piece>(),
                CurrentPlayer = Player.Up,
                HitCounter = 0
            };
            res.UpPlayer[new Point{X = 0, Y = 0}] = Piece.Lion;
            res.UpPlayer[new Point{X = 6, Y = 0}] = Piece.Tiger;
            res.UpPlayer[new Point{X = 1, Y = 1}] = Piece.Dog;
            res.UpPlayer[new Point{X = 5, Y = 1}] = Piece.Cat;
            res.UpPlayer[new Point{X = 0, Y = 2}] = Piece.Rat;
            res.UpPlayer[new Point{X = 2, Y = 2}] = Piece.Panther;
            res.UpPlayer[new Point{X = 4, Y = 2}] = Piece.Wolf;
            res.UpPlayer[new Point{X = 6, Y = 2}] = Piece.Elephant;

            res.DownPlayer[new Point{X = 6, Y = 8}] = Piece.Lion;
            res.DownPlayer[new Point{X = 0, Y = 8}] = Piece.Tiger;
            res.DownPlayer[new Point{X = 5, Y = 7}] = Piece.Dog;
            res.DownPlayer[new Point{X = 1, Y = 7}] = Piece.Cat;
            res.DownPlayer[new Point{X = 6, Y = 6}] = Piece.Rat;
            res.DownPlayer[new Point{X = 4, Y = 6}] = Piece.Panther;
            res.DownPlayer[new Point{X = 2, Y = 6}] = Piece.Wolf;
            res.DownPlayer[new Point{X = 0, Y = 6}] = Piece.Elephant;

            return res;
        }
    }
}