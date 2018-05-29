using System;
using System.Collections.Generic;
using System.Drawing;

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
        Black,
        White
    }

    struct Point {
        public int X;
        public int Y;

        public override int GetHashCode() => X << 16 | Y;

        public override bool Equals(object obj) {
            if (obj is Point p) {
                return X == p.X && Y == p.Y; 
            }
            return false;
        }
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


        public Dictionary<Point, Piece> WhitePlayer;
        public Dictionary<Point, Piece> BlackPlayer;

        public Player CurrentPlayer;

        public List<(Point, Direction)> PossibleMoves(Player player) {
            List<(Point, Direction)> res = new List<(Point, Direction)>();
            foreach(Point p in (player == Player.White)? WhitePlayer.Keys : BlackPlayer.Keys) {
                foreach (var d in Directions) {
                    if (IsPossible(player, p, d)) {
                        res.Add((p, d));
                    }
                }
            }

            return res;
        }

        public GameState ApplyMove(Point position, Direction d) {
            var res = new GameState{
                WhitePlayer = new Dictionary<Point, Piece>(this.WhitePlayer),
                BlackPlayer = new Dictionary<Point, Piece>(this.BlackPlayer),
                CurrentPlayer = (this.CurrentPlayer == Player.White) ? Player.Black : Player.White
            };
            var our = (this.CurrentPlayer == Player.White) ? res.WhitePlayer : res.BlackPlayer;
            var ene = (this.CurrentPlayer == Player.White) ? res.BlackPlayer : res.WhitePlayer;
            Point npos = MovePiece(position, d);
            if (ene.ContainsKey(npos)) {
                ene.Remove(npos);
            }
            our[npos] = our[position];
            our.Remove(position);
            return res;
        }

        private Point MovePiece(Point pos, Direction d)
        {
            throw new NotImplementedException();
        }

        private bool IsPossible(Player player, Point position, Direction d)
        {
            throw new NotImplementedException();
        }
    }
}