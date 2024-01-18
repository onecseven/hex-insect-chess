using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Runtime.ExceptionServices;

namespace Hive
{
    #region data enums
    public enum Pieces
    {
        BEE,
        ANT,
        SPIDER,
        GRASSHOPPER,
        MOSQUITO,
        BEETLE,
        LADYBUG,
    }

    public enum Phases
    {
        PREPARED,
        INITIAL,
        WAITING_FOR_MOVE,
        GAME_OVER,
    }

    public enum Players
    {
        BLACK,
        WHITE
    }

    public enum MoveType
    {
        INITIAL_PLACE,
        PLACE,
        MOVE_PIECE
    }

    #endregion

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
    //public class Tile
    //{

    //    public Cell cell;
    //    public List<Piece> pieces = new List<Piece>();
    //    public bool isOccupied => pieces.Count > 0;
    //}
    public class TheHive
    {
        public bool hasPieceAt(Cell cell)
        {
           return true;
        }

        public TheHive()
        {
        }

     #region rule checkers
   

    //freedom to move is on boardnode
        #endregion
    }
    #region moves

    public partial class Move: GodotObject
    {
        public Players player;
        public Pieces piece;
        public MoveType type;
        public Move(Players player, Pieces piece, MoveType type)
        {
            this.player = player;
            this.piece = piece;
            this.type = type;
        }
    }
    public partial class PLACE : Move 
    {
        public Cell destination;
        public PLACE(Players player, Pieces piece, Cell destination) : base(player, piece, MoveType.PLACE) 
        {
            this.destination = destination;
        }
    }
    public partial class INITIAL_PLACE : Move
    {
        public Cell destination;
        public INITIAL_PLACE(Players player, Pieces piece, Cell destination) : base(player, piece, MoveType.INITIAL_PLACE)
        {
            this.destination = destination;
        }
    }

        public partial class MOVE_PIECE : Move
    {
        public Cell origin;
        public Cell destination;
        public MOVE_PIECE(Players player, Pieces piece, Cell origin, Cell destination) : base(player, piece, MoveType.MOVE_PIECE)
        {
            this.destination = destination;
            this.origin = origin;
        }
    }
    #endregion


}
// + player kind