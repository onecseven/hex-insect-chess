using Godot;
using HexDemo3;
using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive
{
    public class Board
    {
        public Dictionary<Cell, Tile> piecesInPlay = new Dictionary<Cell, Tile>();
        public List<Cell> filteredPiecesInPlay { get { return piecesInPlay.Where(kvp => kvp.Value.isOccupied).ToList().Select(KeyValuePair => KeyValuePair.Key).ToList(); } }
        public Sylves.HexGrid grid = new Sylves.HexGrid(25);
        public Board()
        {
            foreach (Cell cell in HexUtils.HexGen(36, 24, HexOrientation.PointyTopped))
            {
                piecesInPlay.Add(cell, new Tile(cell));
            }
        }

        private readonly Cell nullCell = new Cell(-12, -12, -12);

        public void movePiece(Cell origin, Piece piece)
        {
            piecesInPlay[origin].removePiece();
            piecesInPlay[piece.location].addPiece(piece);

        }
        public void placePiece(Piece pieceToPlace) => piecesInPlay[pieceToPlace.location].addPiece(pieceToPlace);
        public List<Cell> getEmptyNeighbors(Cell cell) => getNeighbors(cell).Where(x => !tileIsOccupied(x)).ToList();
        public List<Cell> getOccupiedNeighbors(Cell cell) => getNeighbors(cell).Where(x => tileIsOccupied(x)).ToList();
        public bool tileIsOccupied(Cell cell) => piecesInPlay.ContainsKey(cell) && piecesInPlay[cell].isOccupied;
        public List<Cell> getNeighbors(Cell origin) => HiveUtils.getNeighbors(origin).ToList();
        //this one works mbut maybe our version will too
        //public List<Cell> getNeighbors(Sylves.Cell origin) => grid.GetNeighbours(origin).ToList();
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
        //this is for the freedom to move rule right
        /*
         *
         *returns xy

           /  \
          |  y |
         / \  /  \
        | a ||  b |
         \ /  \  /
           | x |     
         `  \ /
         */
        public List<Cell> connectingAdjacents(Cell a, Cell b)
        {
            List<Cell> aNeighbors = HiveUtils.getNeighbors(a);
            List<Cell> bNeighbors = HiveUtils.getNeighbors(b);
            List<Cell> union = aNeighbors.Intersect(bNeighbors).ToList();
            if (union.Count > 0 && union.Count == 2) return union;
            else throw new Exception("connectingAdjacents fucked up somewhere!");
        }

        public bool CanMoveBetween(Cell a, Cell b) => !connectingAdjacents(a, b).All(cell => tileIsOccupied(cell));

    }
}
