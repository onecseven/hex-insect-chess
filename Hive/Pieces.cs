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
        public static List<Path> getLegalMoves(Piece piece, BoardNode board)
        {
            //GD.Print(piece.location);
            switch (piece.type)
            {
                case Pieces.BEE:
                    return Bee._getLegalMoves(board, piece.location);
                case Pieces.SPIDER:
                    return Spider._getLegalMoves(board, piece.location);
                case Pieces.BEETLE:
                    return Beetle._getLegalMoves(board, piece.location);
                case Pieces.LADYBUG:
                    return Ladybug._getLegalMoves(board, piece.location);
                case Pieces.GRASSHOPPER:
                    return Grasshopper._getLegalMoves(board, piece.location);
                case Pieces.MOSQUITO:
                    return Mosquito._getLegalMoves(board, piece.location);
                case Pieces.ANT:
                    return Ant._getLegalMoves(board, piece.location);
                default:
                    throw new NotImplementedException();
            }
        }    
    }
    public class Bee : Piece
    {
        public Bee(Players p, Cell l) : base(Pieces.BEE, p, l) { }
        public static List<Path> _getLegalMoves(BoardNode board, Cell origin) => board.adjacentLegalCells(origin).Select(cell => new Path(cell)).ToList();
    }
    public class Mosquito : Piece
    {
        bool beetleMode = false;
        Piece blockedPiece = null;
        public static List<Path> _getLegalMoves(BoardNode board, Cell origin) {
            List<Path> result = new List<Path>();
            Piece originalPiece = board.piecesInPlay[origin];
            List<Cell> occupied_guys = board.getOccupiedNeighbors(origin);
            foreach (Cell occupied in occupied_guys)
            {
                Pieces current_type = board.piecesInPlay[occupied].type;
                if (current_type == Pieces.MOSQUITO) continue;
                List<Path> convertedMoves = Piece.getLegalMoves(new Piece(current_type, originalPiece.owner, origin), board);
                result.AddRange(convertedMoves);
            }
            return result;
        }

        public Mosquito(Players p, Cell l) : base(Pieces.MOSQUITO, p, l) { }
    }
    public class Ladybug : Piece
    {
        public static List<Path> _getLegalMoves(BoardNode board, Cell origin)
        {
            List<Cell> pathOrigins = board.getOccupiedNeighbors(origin);
            List<Path> paths = new();
            foreach (Cell firstStep in pathOrigins)
            {

                List<Cell> secondStep = board.getOccupiedNeighbors(firstStep);
                secondStep.Remove(origin);

                foreach (Cell seStep in secondStep)
                {
                    List<Cell> lastStep = board.getEmptyNeighbors(seStep);
                    lastStep.ForEach(last => paths.Add(new Path(new List<Cell>() { firstStep, seStep, last })));
                }
            }
            return paths;
        }
        public Ladybug(Players p, Cell l) : base(Pieces.LADYBUG, p, l) { }
    }
    public class Beetle : Piece
    {
        Piece blockedPiece = null;
        public static List<Path> _getLegalMoves(BoardNode board, Cell origin)
        {
            List<Cell> result = new List<Cell>();
            result.AddRange(board.adjacentLegalCells(origin));
            result.AddRange(board.getOccupiedNeighbors(origin));
            return result.ConvertAll<Path>(cell => new Path(cell));
        }
        public Beetle(Players p, Cell l) : base(Pieces.BEETLE, p, l) { }  
    }
    public class Spider : Piece
    {
        public static List<Path> _getLegalMoves(BoardNode board, Cell origin)
        {
            List<Cell> pathOrigins = board.adjacentLegalCells(origin);
            List<Path> paths = new();
            List<Cell> endpoints = new();
            foreach (Cell firstStep in pathOrigins)
            {

                List<Cell> secondStep = board.hypotheticalAdjacentLegalCells(firstStep, origin);
                secondStep.Remove(origin);

                foreach (Cell seStep in secondStep)
                {
                    List<Cell> lastStep = board.hypotheticalAdjacentLegalCells(seStep, origin);
                    lastStep.Remove(firstStep);
                    lastStep.ForEach(last => paths.Add(new Path(firstStep, seStep, last)));
                }
            }
            return paths;
        }
        public Spider(Players p, Cell l) : base(Pieces.SPIDER, p, l) { }
    }
    public class Grasshopper : Piece
    {
        private static Path traverseDirection(Cell startPoint, Cell direction, BoardNode board)
        {
            List<Cell> result = new List<Cell>() { startPoint };
            Cell current = startPoint;
            bool toContinue = true;
            while (toContinue == true)
            {
                var newOne = new Cell(current.x + direction.x, current.y + direction.y, current.z + direction.z);
                if (board.tileIsOccupied(newOne))
                {
                    current = newOne;
                    result.Add(newOne);
                    continue;
                }
                else
                {
                    result.Add(newOne);
                    return new Path(result);
                }
            }
            throw new Exception("huh");
        }
        public static List<Path> _getLegalMoves(BoardNode board, Cell origin)
        {
            List<Path> result = new List<Path> ();
            foreach (Cell direction in HiveUtils.directions)
            {
                Cell current = new Cell(origin.x + direction.x, origin.y + direction.y, origin.z + direction.z);
                if (board.tileIsOccupied(current)) result.Add(traverseDirection(current, direction, board));
            }
            return result;
        }
        public Grasshopper(Players p, Cell l) : base(Pieces.GRASSHOPPER, p, l) { }
    }
    public class Ant : Piece
    {
        public static HashSet<Cell> findAll(BoardNode board, Cell origin, HashSet<Cell> excluded)
        {
            excluded.Add(origin);
            var adj = board.hypotheticalAdjacentLegalCells(origin, excluded.ToList()).Where(item => !excluded.Contains(item)); ;
            if (adj.All(item => excluded.Contains(item))) return excluded;
            foreach(var p in adj)
            {
                excluded.UnionWith(findAll(board, p, excluded));
                break;
            }
            return excluded;
        }
        public static List<Path> findViablePath(BoardNode board, Cell origin, Cell dest, List<Cell> excluded)
        {
            List<Path> result = new List<Path>();
            excluded.Add(origin);
            List<Cell> nextSteps = board.hypotheticalAdjacentLegalCells(origin, excluded);
            if (nextSteps.Contains(dest) && board.CanMoveBetween(origin, dest)) 
            {
                excluded.Add(dest);
                result.Add(new Path(excluded));
                return result;
            }
            foreach (Cell step in nextSteps)
            {
                result.AddRange(findViablePath(board, step, dest, excluded));
            }
            return result;
        }

        public static List<Path> _getLegalMoves(BoardNode board, Cell origin)
        {
            return findAll(board, origin, new HashSet<Cell>()).ToList().Select(x => new Path(x)).ToList();
        }
        public Ant(Players p, Cell l) : base(Pieces.ANT, p, l) { }
    }
}
