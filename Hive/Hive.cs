using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Runtime.ExceptionServices;
using HexDemo3;

namespace Hive
{

    public class Player
    {
        public Dictionary<Pieces, int> inventory = new Dictionary<Pieces, int>()
        {
            [Pieces.BEE] = 1,
            [Pieces.ANT] = 3,
            [Pieces.SPIDER] = 2,
            [Pieces.BEETLE] = 2,
            [Pieces.GRASSHOPPER] = 3,
            [Pieces.MOSQUITO] = 1,
            [Pieces.LADYBUG] = 1

        };
        public Players color;

        public Player(Players _color)
        {
            color = _color;
        }

        public void piecePlaced(Pieces _piece)
        {
            if (hasPiece(_piece)) inventory[_piece]--;
            else throw new ArgumentOutOfRangeException("piece placed called when player is out of pieces");
        }
        public bool hasPiece(Pieces _piece) => inventory.ContainsKey(_piece) && inventory[_piece] > 0;

    }
    public class Path
    {
        public List<Cell> steps;
        public Cell last
        {
            get
            {
                if (steps.Count < 1) throw new ArgumentOutOfRangeException("path class");
                return steps[steps.Count - 1];
            }
        }
        public Cell penult
        {
            get
            {
                if (steps.Count < 2) throw new ArgumentOutOfRangeException("path class");
                return steps[steps.Count - 2];
            }
        }
        public bool isSingleStep { get => steps.Count == 1; }
        public List<(Cell first, Cell last)> pairs { get
            {
                if (isSingleStep) { return new List<(Cell first, Cell last)>(); }
                List<(Cell first, Cell last)> result = new List<(Cell first, Cell last)>();
                for (int i = 0; i < steps.Count - 2; i++)
                {
                    //int next = i + 1 > steps.Count - 1 ? 0 : i + 1;
                    result.Add((steps[i], steps[i + 1]));
                }
                return result;
            }
        }
        public Path(List<Cell> steps)
        {
            this.steps = steps;
        }

        public bool isNullPath { get => steps.Count == 0;}
        public Path(Cell step)
        {
            this.steps = new List<Cell>() { step };
        }
        public Path(params Cell[] step)
        {
            this.steps = step.ToList();
        }
    }
    public class Tile
    {

        public Cell cell;
        public List<Piece> pieces = new List<Piece>();
        public Piece activePiece { get {
                if (pieces.Count > 0) return pieces[0];
                else return null;
            } }
        public bool isOccupied => pieces.Count > 0;
        public Tile(Cell cell)
        {
            this.cell = cell;
        }

        public void addPiece(Piece piece) { pieces.Prepend(piece); }
        public Piece removePiece(Piece piece) {
            Piece returnable = pieces[0];
            pieces.RemoveAt(0);
            return returnable;
        }
    }
    public partial class Hive
    {
        public Phases game_status = Phases.PREPARED;
        public Players turn = Players.WHITE;
        List<Move> moves = new List<Move>();
        public Sylves.HexGrid grid = new Sylves.HexGrid(25);
        public Dictionary<Cell,Tile> board = new Dictionary<Cell,Tile>();
        public Dictionary<Players, Player> players = new Dictionary<Players, Player> {
            [Players.WHITE] = new Player(Players.WHITE),
            [Players.BLACK] = new Player(Players.BLACK),
        };
        public Hive()
        {
            foreach (Cell cell in HexUtils.HexGen(36,24,HexOrientation.PointyTopped))
            {
                board.Add(cell, new Tile(cell));
            }
        }
     #region rule checkers
    //freedom to move is on boardnode
     #endregion
    }
}
// + player kind