using Godot;
using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive
{
    public partial class Hive
    {
        private bool moveIsValid(Move move)
        {
            if (move.player != turn || (mustPlayBee(move.player) && (move.piece != Pieces.BEE)))
            {
                GD.PrintErr("Must Play Bee or wrong player.");
                return false;
            }
            switch (move.type)
            {
                case MoveType.INITIAL_PLACE:
                    INITIAL_PLACE initialPlaceCasted = (INITIAL_PLACE)move;
                    GD.Print("\n" + move.player + "INITIAL PLACING " + initialPlaceCasted.piece + " ON " + initialPlaceCasted.destination);
                    return initialPlacementCheck(initialPlaceCasted);
                case MoveType.PLACE:
                    PLACE placeCasted = (PLACE)move;
                    return placementLegalityCheck(placeCasted.destination, placeCasted.player, placeCasted.piece);
                case MoveType.MOVE_PIECE:
                    MOVE_PIECE moveCasted = (MOVE_PIECE)move;
                    if (moveIsLegal(moveCasted) == false || oneHiveRuleCheck(moveCasted.origin) == false) return false;
                    break;
                default:
                    break;
            }
            return true;
        }

        #region MOVE_PIECE check
        bool moveIsLegal(MOVE_PIECE move)
        {
            // check that the piece is in play
            if (!board.piecesInPlay[move.origin].isOccupied || board.piecesInPlay[move.origin].activePiece.owner != move.player) return false;
            Piece piece = board.piecesInPlay[move.origin].activePiece;
            List<Path> paths = Piece.getLegalMoves(piece, board);
            Path playerPath = paths[paths.FindIndex(path => path.last == move.destination)];
            List<Sylves.Cell> endpoints = paths.Select(path => path.last).ToList();
            if (endpoints.Contains(move.destination) && pathIsLegal(playerPath, piece.type)) return true;
            else return false;
        }
        bool pathIsLegal(Path path, Pieces pieceType)
        {
            //TODO we can probably just check for normal moving piece types and then check that origin and end are not surrounded by door formations
            //we just check that the origin/end aren't surrounded by >= 5 pieces
            //switch (pieceType)
            //{
            //    case Pieces.SPIDER:
            //    case Pieces.ANT:
            //    case Pieces.BEE:
            //    case Pieces.BEETLE:
            //    foreach ((Cell first, Cell last) in path.pairs)
            //    {
            //        if (!board.CanMoveBetween(first, last)) return false;
            //    }
            //    break;
            //    case Pieces.LADYBUG:
            //        break;
            //    case Pieces.GRASSHOPPER:
            //        break;
            //    case Pieces.MOSQUITO:
            //        break;
            //}

            //giving up on checking if paths are legal
            //if there's a bug with grasshoppers it's probably this
            //if (pieceType == Pieces.GRASSHOPPER) return true;
            return true;
        }
        #endregion
        bool placementLegalityCheck(Cell destination, Players player, Pieces piece)
        {
            bool checkIfPlayerHasPieceInInventory(Players player, Pieces piece) => players[player].hasPiece(piece);
            //TODO send getOccupied neighbors to hexutils or maybe even a dedicated board class
            List<Sylves.Cell> surroundingPieces = board.getOccupiedNeighbors(destination);

            //pieces in play -> board[Cell]

            List<Piece> actualPieces = surroundingPieces.Select(cel => board.piecesInPlay[cel].activePiece).ToList();
            bool piecesBelongToMover = actualPieces.All(pieces => player == pieces.owner);
            bool playerHasPieceInHand = checkIfPlayerHasPieceInInventory(player, piece);
            //GD.Print("Player has pieces to connect: ", actualPieces.Count > 0);
            //GD.Print("Pieces belong to mover: ", piecesBelongToMover);
            //GD.Print("Player has piece " + piece + " in inventory: ", playerHasPieceInHand);
            if (actualPieces.Count > 0 && piecesBelongToMover && playerHasPieceInHand) return true;
            return false;
        }
        bool initialPlacementCheck(INITIAL_PLACE move)
        {
            if (moves.Count == 0) return true;
            else if (moves.Count == 1)
            {
                bool isCorrectUser = moves[0].player != move.player;
                bool isAdjacentToFirstPiece = board.AreCellsAdjacent(((INITIAL_PLACE)moves[0]).destination, move.destination);
                if (isCorrectUser && isAdjacentToFirstPiece) return true;
            }
            return false;
        }
        bool mustPlayBee(Players player)
        {
            bool hasPlayedBee = moves.Where(move => move.player == player).Where(move => move.type == MoveType.INITIAL_PLACE || move.type == MoveType.PLACE).Any(move => move.piece == Pieces.BEE);
            bool hasPlayedThreeMoves = (moves.Where(move => move.player == player).ToList().Count) == 3;
            bool belowLimit = (moves.Where(move => move.player == player).ToList().Count) < 3;
            //GD.Print("hasPlayedBee ", hasPlayedBee);
            //GD.Print("hasPlayedThreeMoves " + hasPlayedThreeMoves + " " + moves.Where(move => move.player == player).ToList().Count);
            if (hasPlayedThreeMoves && !hasPlayedBee) return true;
            else if (!belowLimit && !hasPlayedThreeMoves && !hasPlayedBee)
            {
                throw new ArgumentException("illegal game state");
            }
            return false;
        }

        #region autopass checks

        public void autopassCheck()
        {

            if ((playerHasPiecesInPlay(turn) && hasLegalPlacementTarget(turn)) || playerHasPossibleMoves(turn)) return;
            else
            {
                if (moves.Last().type == MoveType.AUTOPASS)
                {
                    throw new Exception("AUTOPASS LOOP FUCK");
                }
                GD.Print("Autopass!");

                send_move(new AUTOPASS(turn));
            };

        }
        bool hasLegalPlacementTarget(Players player)
        {
            if (moves.Count <= 2) return true;
            //br
            List<Piece> playerPieces = board.piecesInPlay.Where(kvp => kvp.Value.isOccupied && kvp.Value.activePiece.owner == player).Select(kvp => kvp.Value.activePiece).ToList();
            foreach (Piece item in playerPieces)
            {
                //br
                List<Cell> neighbors = board.getEmptyNeighbors(item.location);
                if (neighbors.Any(neigh => placementLegalityCheck(neigh, player, item.type))) return true;
            }
            return false;
        }
        //br
        bool playerHasPiecesInPlay(Players player) => board.piecesInPlay.Where(kvp => kvp.Value.isOccupied && kvp.Value.activePiece.owner == player).ToList().Count < 13;
        bool playerHasPossibleMoves(Players player)
        {
            //br
            List<Piece> playerPieces = board.piecesInPlay.Where(kvp => kvp.Value.isOccupied && kvp.Value.activePiece.owner == player).Select(kvp => kvp.Value.activePiece).ToList();
            List<Path> possibleMoves = new List<Path>();
            foreach (Piece piece in playerPieces)
            {
                //piece rewrite
                possibleMoves.AddRange(Piece.getLegalMoves(piece, board));
            }
            if (possibleMoves.Count == 0 || possibleMoves.All(path => path.isNullPath)) return false;
            else return true;
        }
        #endregion

        #region onehiverule check

        private HashSet<Cell> recursiveGetNeighbors(Cell cell, HashSet<Cell> memo, Cell? excludedCell)
        {
            //br
            List<Cell> neighbors = board.getOccupiedNeighbors(cell);
            if (excludedCell.HasValue) neighbors.Remove(excludedCell.Value);
            if (neighbors.Count == 0 || neighbors.All(item => memo.Contains(item))) return memo;
            foreach (Cell neighbor in neighbors)
            {
                memo.Add(neighbor);
            }
            foreach (Cell neighbor in neighbors)
            {
                memo = recursiveGetNeighbors(neighbor, memo, excludedCell);
            }
            return memo;
        }
        public bool oneHiveRuleCheck(Cell movingPiece)
        {
            HashSet<Cell> hypoHive = new HashSet<Cell>();
            //br
            List<Cell> prelim = board.filteredPiecesInPlay;
            prelim.Remove(movingPiece);
            Cell first = prelim.First();
            int target = prelim.Count;
            HashSet<Cell> computed = recursiveGetNeighbors(first, hypoHive, movingPiece);
            if (computed.Count != target) return false;
            return true;
        }
        #endregion
        public bool wincon_check()
        {
            List<Piece> bees = board.piecesInPlay.Where(kvp => kvp.Value.isOccupied && kvp.Value.pieces.Any(pie => pie.type == Pieces.BEE)).Select(kvp => kvp.Value.activePiece).ToList();
            if (bees.Any(piece => board.getOccupiedNeighbors(piece.location).Count == 6)) return true;
            return false;
        }
    }
}

