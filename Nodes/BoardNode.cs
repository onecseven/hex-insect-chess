using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class BoardNode : Node2D
{
    [Export]
    public TatiHex grid = null;
    private Player whitePlayer = new Player(Players.WHITE);
    private Player blackPlayer = new Player(Players.BLACK);
    public Dictionary<Cell, Hive.Piece> piecesInPlay = new Dictionary<Cell, Hive.Piece>() { [new Cell(0, 0, 0)] = Piece.create(Pieces.BEE, Players.BLACK, new Cell(0, 0)) };
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
    public List<Cell> getEmptyNeighbors(Cell cell) => getNeighbors(cell).Where(x => tileIsOccupied(x)).ToList();
    public List<Cell> getOccupiedNeighbors(Cell cell) => getNeighbors(cell).Where(x => !tileIsOccupied(x)).ToList();
    public bool tileIsOccupied(Cell cell) => piecesInPlay.ContainsKey(cell);
    public List<Cell> getNeighbors(Sylves.Cell origin) => HiveUtils.getNeighbors(origin);
    public List<Cell> adjacentLegalCells(Cell cell)
    {
        List<Cell> empty = getEmptyNeighbors(cell);
        List<Cell> neighbors = getOccupiedNeighbors(cell);
        HashSet<Cell> neighbor_adjacent = new HashSet<Cell>();
        foreach (Cell neighbor in neighbors)
        {
            List<Cell> temp = getEmptyNeighbors(neighbor);
            temp.ForEach(x => neighbor_adjacent.Add(neighbor));
        }
        return empty.Intersect(neighbor_adjacent).ToList();
    }
}
