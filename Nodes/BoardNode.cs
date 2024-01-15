using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BoardNode : Node2D
{
    private readonly Cell nullCell = new Cell(-12, -12, -12);
    [Export]
    public TatiHex grid = null;
    private Player whitePlayer = new Player(Players.WHITE);
    private Player blackPlayer = new Player(Players.BLACK);
    public Dictionary<Cell, Hive.Piece> piecesInPlay = new Dictionary<Cell, Hive.Piece>() {
        [new Cell(5, 4, -9)] = Piece.create(Pieces.BEE, Players.BLACK, new Cell(5, 4,-9)),
        [new Cell(5 ,5, -10)] = Piece.create(Pieces.BEE, Players.BLACK, new Cell(5, 5, -10)),
        [new Cell(6, 4, -10)] = Piece.create(Pieces.BEE, Players.BLACK, new Cell(6, 4, -10)),
        [new Cell(4, 6, -10)] = Piece.create(Pieces.BEE, Players.BLACK, new Cell(4, 6, -10)),
        [new Cell(4, 5, -9)] = Piece.create(Pieces.BEE, Players.BLACK, new Cell(4, 5, -9)),
        [new Cell(5, 6, -11)] = Piece.create(Pieces.BEE, Players.BLACK, new Cell(5, 6, -11)),
        [new Cell(6, 5, -11)] = Piece.create(Pieces.SPIDER, Players.BLACK, new Cell(6, 5, -11)),
        [new Cell(7, 3, -10)] = Piece.create(Pieces.LADYBUG, Players.BLACK, new Cell(7, 3, -10)),
        [new Cell(6, 6, -12)] = Piece.create(Pieces.GRASSHOPPER, Players.BLACK, new Cell(6, 6, -12)),
    };
    public void place(PLACE move)
    {
        Player pieceOwner = move.player == Hive.Players.BLACK ? blackPlayer : whitePlayer;
        Piece newPiece = Piece.create(move.piece, move.player, (move.destination));
        if (pieceOwner.hasPiece(newPiece.type))
        {
            pieceOwner.piecePlaced(newPiece.type);
            piecesInPlay.Add(newPiece.location, newPiece);
            QueueRedraw();
        }
        else throw new ArgumentException("trying to place piece that has no location");
    }
    public void move(MOVE_PIECE move)
    {
        if (piecesInPlay.ContainsKey(move.origin) && !piecesInPlay.ContainsKey(move.destination))
        {
            piecesInPlay[move.destination] = Piece.create(piecesInPlay[move.origin].type, piecesInPlay[move.origin].owner, move.destination);
            piecesInPlay.Remove(move.origin);
            QueueRedraw();
        }
    }
    public void initialPlace(INITIAL_PLACE move) => place(new Hive.PLACE(move.player, move.piece, move.destination));
    public override void _Draw()
	{
        foreach ((Cell key, Hive.Piece piece) in piecesInPlay)
        {
            if (grid.hexes.ContainsKey(piece.location))
            {
                Vector3 center = grid.hexes[piece.location];
                DrawTexture(piece.texture, (HiveUtils.Vector3ToVector2(center) + grid.Position) - new Vector2(16,16));
            }
        }
	}
    public List<Cell> getEmptyNeighbors(Cell cell) => getNeighbors(cell).Where(x => !tileIsOccupied(x)).ToList();
    public List<Cell> getOccupiedNeighbors(Cell cell) => getNeighbors(cell).Where(x => tileIsOccupied(x)).ToList();
    public bool tileIsOccupied(Cell cell) => piecesInPlay.ContainsKey(cell);
    public List<Cell> getNeighbors(Sylves.Cell origin) => grid.grid.GetNeighbours(origin).ToList();
    public bool AreCellsAdjacent(Cell a, Cell B) => HiveUtils.getNeighbors(a).Contains(B);
    public List<Cell> adjacentLegalCells(Cell cell)
    {
        List<Cell> empty = getEmptyNeighbors(cell);
        List<Cell> neighbors = getOccupiedNeighbors(cell);
        HashSet<Cell> neighbor_adjacent = new HashSet<Cell>();
        foreach (Cell neighbor in neighbors)
        {
            List<Cell> neighborAdjacentEmpties = getEmptyNeighbors(neighbor);
            neighborAdjacentEmpties.ForEach(temp_tile => neighbor_adjacent.Add(temp_tile));
        }
        var prelim = empty.Intersect(neighbor_adjacent).ToList();
        return prelim.ToList();
    }
    public List<Cell> hypotheticalAdjacentLegalCells(Cell cell, Cell exclude)
    {
        List<Cell> empty = getEmptyNeighbors(cell);
        List<Cell> neighbors = getOccupiedNeighbors(cell);
        neighbors.Remove(exclude);
        HashSet<Cell> neighbor_adjacent = new HashSet<Cell>();
        foreach (Cell neighbor in neighbors)
        {
            List<Cell> neighborAdjacentEmpties = getEmptyNeighbors(neighbor);
            neighborAdjacentEmpties.ForEach(temp_tile => neighbor_adjacent.Add(temp_tile));
        }
        var prelim = empty.Intersect(neighbor_adjacent).Where(next => CanMoveBetween(cell, next)).ToList();
        return prelim.ToList();
    }
    public List<Cell> hypotheticalAdjacentLegalCells(Cell cell, List<Cell> exclude)
    {
        List<Cell> empty = getEmptyNeighbors(cell);
        List<Cell> neighbors = getOccupiedNeighbors(cell);
        foreach (Cell toExclude in exclude)
        {
            neighbors.Remove(toExclude);   
        }
        HashSet<Cell> neighbor_adjacent = new HashSet<Cell>();
        foreach (Cell neighbor in neighbors)
        {
            List<Cell> neighborAdjacentEmpties = getEmptyNeighbors(neighbor);
            neighborAdjacentEmpties.ForEach(temp_tile => neighbor_adjacent.Add(temp_tile));
        }
        var prelim = empty.Intersect(neighbor_adjacent).Where(next => CanMoveBetween(cell, next)).ToList();
        return prelim.ToList();
    }
    //this is for the freedom to move rule right?
    public List<Cell> connectingAdjacents(Cell a, Cell b)
    {
        List<Cell> aNeighbors = HiveUtils.getNeighbors(a);
        List<Cell> bNeighbors = HiveUtils.getNeighbors(b);
        List<Cell> union = aNeighbors.Intersect(bNeighbors).ToList();
        if (union.Count > 0 && union.Count == 2) return union;
        else throw new Exception("connectingAdjacents fucked up somewhere!");
    }

    public bool CanMoveBetween(Cell a, Cell b) => !connectingAdjacents(a,b).All(cell => tileIsOccupied(cell));
   
}
