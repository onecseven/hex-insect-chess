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
        [new Cell(0, 0, 0)] = Piece.create(Pieces.BEE, Players.BLACK, new Cell(0, 0)),
        [new Cell(5, 4, -9)] = Piece.create(Pieces.BEE, Players.BLACK, new Cell(5, 4,-9)),
        [new Cell(5 ,5, -10)] = Piece.create(Pieces.BEE, Players.BLACK, new Cell(5, 5, -10)),
        [new Cell(6, 4, -10)] = Piece.create(Pieces.BEE, Players.BLACK, new Cell(6, 4, -10)),
        [new Cell(4, 6, -10)] = Piece.create(Pieces.BEE, Players.BLACK, new Cell(4, 6, -10)),
        [new Cell(4, 5, -9)] = Piece.create(Pieces.BEE, Players.BLACK, new Cell(4, 5, -9)),
        [new Cell(5, 6, -11)] = Piece.create(Pieces.BEE, Players.BLACK, new Cell(5, 6, -11)),
        [new Cell(6, 5, -11)] = Piece.create(Pieces.SPIDER, Players.BLACK, new Cell(6, 5, -11)),
    };
    public List<Hive.Piece> theHive = new List<Hive.Piece> (); 
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
        else throw new ArgumentException("trying to place piece that has not location");
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
            //get tile location (copy from hove)
            //draw texture 
        }
	}
    public List<Cell> getEmptyNeighbors(Cell cell) => getNeighbors(cell).Where(x => !tileIsOccupied(x)).ToList();
    public List<Cell> getOccupiedNeighbors(Cell cell) => getNeighbors(cell).Where(x => tileIsOccupied(x)).ToList();
    public bool tileIsOccupied(Cell cell) => piecesInPlay.ContainsKey(cell);
    //public List<Cell> getNeighbors(Sylves.Cell origin) => HiveUtils.getNeighbors(origin);
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
        var prelim = empty.Intersect(neighbor_adjacent).ToList();
        return prelim.ToList();
    }
}
