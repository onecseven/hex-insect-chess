 using Godot;
using Godot.Collections;
using Sylves;
using System.Linq;
using System.Collections.Generic;
using System;

namespace Hive
{
    public class Piece
    {
        public Pieces type;
        public Players owner;
        public Cell location;
        public static Godot.Collections.Dictionary<Pieces, Texture2D> _textures = new Godot.Collections.Dictionary<Pieces, Texture2D>() {
                [Pieces.ANT] = GD.Load<Texture2D>("res://assets/ant.png"),
                [Pieces.BEE] = GD.Load<Texture2D>("res://assets/bee.png"),
                [Pieces.BEETLE] = GD.Load<Texture2D>("res://assets/beetle.png"),
                [Pieces.GRASSHOPPER] = GD.Load<Texture2D>("res://assets/grasshopper.png"),
                [Pieces.LADYBUG] = GD.Load<Texture2D>("res://assets/ladybug.png"),
                [Pieces.MOSQUITO] = GD.Load<Texture2D>("res://assets/mosquito.png"),
                [Pieces.SPIDER] = GD.Load<Texture2D>("res://assets/spider.png"),

        };
        public  Texture2D texture { get {
                return _textures[this.type];
            } }

        public Piece(Pieces t, Players o, Cell l)
        {
            type = t;
            owner = o;
            location = l;
        }
        public static Piece create(Pieces piece, Players player, Cell destination)
        {
            switch (piece)
            {
                case Pieces.BEE:
                    return new Bee(player, destination);   
                case Pieces.ANT:
                    return new Ant(player, destination);    
                case Pieces.SPIDER:
                    return new Spider(player, destination);
                case Pieces.GRASSHOPPER:
                    return new Grasshopper(player, destination);    
                case Pieces.MOSQUITO:
                    return new Mosquito(player, destination); 
                case Pieces.BEETLE:
                    return new Beetle(player, destination);
                case Pieces.LADYBUG:
                    return new Ladybug(player, destination);
                default:
                    throw new ArgumentException("unrecognized piece type cannot be created");
            }
        }
        public static List<Cell> getLegalMoves(Piece piece, BoardNode board)
        {
            //GD.Print(piece.location);
            switch (piece.type)
            {
                case Pieces.BEE:
                    return Bee._getLegalMoves(board, piece.location);
                case Pieces.SPIDER:
                    return Spider._getLegalMoves(board, piece.location);
                    throw new NotImplementedException();
                case Pieces.BEETLE:
                    return Beetle._getLegalMoves(board, piece.location);
                case Pieces.ANT:
                    throw new NotImplementedException();
                    break;
                case Pieces.GRASSHOPPER:
                    throw new NotImplementedException();
                    break;
                case Pieces.MOSQUITO:
                    throw new NotImplementedException();
                    break;
                case Pieces.LADYBUG:
                    throw new NotImplementedException();
                    break;
                default:
                    throw new NotImplementedException();
                    break;
            }
        }    
    }

    public class Bee : Piece
    {
        public Bee(Players p, Cell l) : base(Pieces.BEE, p, l) { }
        public static List<Cell> _getLegalMoves(BoardNode board, Cell origin) => board.adjacentLegalCells(origin);
    }
    public class Mosquito : Piece
    {
        public Mosquito(Players p, Cell l) : base(Pieces.MOSQUITO, p, l) { }
    }
    public class Ladybug : Piece
    {
        public Ladybug(Players p, Cell l) : base(Pieces.LADYBUG, p, l) { }
    }
    public class Beetle : Piece
    {
        Piece blockedPiece = null;
        public static List<Cell> _getLegalMoves(BoardNode board, Cell origin)
        {
            List<Cell> result = new List<Cell>();
            result.AddRange(board.adjacentLegalCells(origin));
            result.AddRange(board.getOccupiedNeighbors(origin));
            return result;
        }
        public Beetle(Players p, Cell l) : base(Pieces.BEETLE, p, l) { }  
    }
    public class Spider : Piece
    {
        public static List<Cell> _getLegalMoves(BoardNode board, Cell origin)
        {
            List<Cell> pathOrigins = board.adjacentLegalCells(origin);
            HashSet<(Cell origin, Cell secondStep, Cell thirdStep)> paths = new();
            List<Cell> endpoints = new();
            foreach (Cell firstStep in pathOrigins)
            {

                List<Cell> secondStep = board.hypotheticalAdjacentLegalCells(firstStep, origin);
                secondStep.Remove(origin);

                foreach (Cell seStep in secondStep)
                {
                    List<Cell> lastStep = board.hypotheticalAdjacentLegalCells(seStep, origin);
                    lastStep.Remove(firstStep);
                    lastStep.ForEach(last => paths.Add((firstStep, seStep, last)));
                    lastStep.ForEach(endpoints.Add);
                }
            }
            HiveUtils.Unroll("Path", paths);
            return endpoints;
        }
        public Spider(Players p, Cell l) : base(Pieces.SPIDER, p, l) { }
    }
    public class Grasshopper : Piece
    {
        public Grasshopper(Players p, Cell l) : base(Pieces.GRASSHOPPER, p, l) { }
    }
    public class Ant : Piece
    {
        public Ant(Players p, Cell l) : base(Pieces.ANT, p, l) { }
    }
}


////using Sylves;
////using System.ComponentModel;
////using System.Diagnostics;
////using System.Collections.Generic;

////public static class IEnumerableExtensions
////{
////    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
////       => self.Select((item, index) => (item, index));
////}
//    namespace Hive
//{
//    public enum Pieces
//    {
//        BEE,
//        ANT,
//        SPIDER,
//        GRASSHOPPER,
//        MOSQUITO,
//        BEETLE,
//        LADYBUG,
//    }
//    public abstract class Piece
//    {
//        public Players owner;
//        public Cell location;
//        public Pieces type;
//        //public abstract List<Cell> getLegalMoves(Hive.Board board); 
//        //public static bool isBlocked(Hive.Board board, Piece piece) {
//        //    Tile? tile = board.getTileAt(piece.location);
//        //    if (tile == null) throw new Exception("Tile not found");
//        //    switch (tile.pieces.Count)
//        //    {
//        //        case 0:
//        //            throw new Exception("Piece not found");
//        //        case 1:
//        //            return false;
//        //        case 2:
//        //        {
//        //                int index = tile.pieces.IndexOf(piece);
//        //                if (index == 0) return false;
//        //                else if (index == -1) throw new Exception("Piece not found");
//        //                else if (index > 0) return true;
//        //                break;
//        //        }
//        //        default:
//        //            throw new Exception("We shouldn't be here");

//        //    }
//        //    throw new Exception("We shouldn't be here #2");
//        //}
//    }
//    public class Bee: Hive.Piece
//    {
//        public Bee(Players player) => owner = player;

//        public  List<Cell> getLegalMoves(Hive.Board board) => Bee._getLegalMoves(board, this.location);
//    }
//    public class Spider: Hive.Piece {
//        public Spider(Players player) => owner = player;

//        public static List<(Cell origin, Cell secondStep, Cell thirdStep)> _getLegalMoves(Hive.Board board, Cell origin) {
//            List<Cell> pathOrigins = board.adjacentLegalCells(origin);
//            HashSet<(Cell origin, Cell secondStep, Cell thirdStep)> results = new();
//            foreach (Cell firstStep in pathOrigins)
//            {
//                List<Cell> secondStep = board.adjacentLegalCells(firstStep);
//                secondStep.Remove(origin);
//                foreach (Cell seStep in secondStep)
//                {
//                    List<Cell> lastStep = board.adjacentLegalCells(seStep); 
//                    lastStep.Remove(firstStep);
//                    lastStep.ForEach(last => results.Add((firstStep, seStep, last)));
//                }
//            }
//            return results.ToList();
//        }
//        public List<(Cell origin, Cell secondStep, Cell thirdStep)> getLegalMoves(Hive.Board board, bool typeDecoy = false) => Spider._getLegalMoves(board, this.location);


//    }
//    public class Grasshopper : Hive.Piece
//    {
//        private static List<Cell> getLineUntilEmpty(Hive.Board board, Cell origin, (int x, int y, int z) dir)
//        {
//            List<Cell> line = new List<Cell>();
//            var current = new Cell(origin.x + dir.x, origin.y + dir.y, origin.z + dir.z);
//            bool finished = false;
//            while (!finished)
//            {
//                if (board.hasPieceAt(current))
//                {
//                    line.Add(current);
//                    current = new Cell(current.x + dir.x, current.y + dir.y, current.z + dir.z);
//                }
//                else
//                {
//                    finished = true;    
//                }
//            }
//            return line;
//        }
//        public static List<List<Cell>> _getLegalMoves(Hive.Board board, Cell origin)
//        {
//            List<(int,int,int)> origins = new List<(int, int, int)>();
//            List<List<Cell>> result = new(new List<List<Cell>>());
//            foreach (var (dir, index)  in board.directions.WithIndex())
//            {
//                Cell currentDirection =  new Cell(origin.x + dir.Item1, origin.y + dir.Item2, origin.z + dir.Item3);
//                if (board.hasPieceAt(currentDirection)) origins.Add(dir);
//            }
//            foreach (var dir in origins)
//            {
//                result.Add(getLineUntilEmpty(board, origin, dir));
//            }
//            return result;
//        }
//        public List<List<Cell>> getLegalMoves(Board board) => _getLegalMoves(board, this.location);
//    }
//    public class Mosquito {
//    /*
//        * 1. get all occupied nieghbors.
//        * 2. use their types get legal moves.-
//        */
//    }
//    public class Beetle { 
//    /*
//        * 1. bee movement
//        * 2. add the immediately adjacent tiles with neighbors on it
//        */
//    }
//    public class Ant {
//    /*
//        * 1. i don't even know...
//        */
//    }
//    public class Ladybug { 
//    /*
//        * 1. research
//        */
//    }

//}

