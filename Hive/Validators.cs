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
                GD.Print("Must Play Bee?");
                return false;
            }
            switch (move.type)
            {
                case MoveType.INITIAL_PLACE:
                    INITIAL_PLACE initialPlaceCasted = (INITIAL_PLACE)move;
                    GD.Print("\n" + move.player + "INITIAL PLACING " + initialPlaceCasted.piece + " ON " + initialPlaceCasted.destination);
                    bool iPlaceCheck = initialPlacementCheck(initialPlaceCasted);
                    GD.Print("Is it a bee placement? ", move.type != MoveType.AUTOPASS && move.type != MoveType.MOVE_PIECE && move.piece == Pieces.BEE);
                    GD.Print("Initial Placement Check: ", iPlaceCheck);
                    if (!iPlaceCheck) return false;
                    break;
                case MoveType.PLACE:
                    PLACE placeCasted = (PLACE)move;
                    GD.Print("\n" + move.player + " PLACING " + placeCasted.piece + " ON " + placeCasted.destination);
                    bool placeCheck = placementLegalityCheck(placeCasted.destination, placeCasted.player, placeCasted.piece);
                    GD.Print("Placement Check: ", placeCheck);
                    if (!placeCheck) return false;
                    break;
                case MoveType.MOVE_PIECE:
                    MOVE_PIECE moveCasted = (MOVE_PIECE)move;
                    GD.Print("\n" + move.player + " MOVING " + moveCasted.piece + " FROM " + moveCasted.origin + " TO " + moveCasted.destination);
                    bool moveCheck = moveIsLegal(moveCasted);
                    bool hiveRuleCheck = oneHiveRuleCheck(moveCasted.origin);
                    GD.Print("Move Is Legal Check: ", moveCheck);
                    GD.Print("One Hive Rule Check: ", hiveRuleCheck);
                    if (!moveIsLegal(moveCasted)) return false;
                    if (!oneHiveRuleCheck(moveCasted.origin)) return false;
                    break;
                default:
                    break;
            }
            return true;
        }
        bool checkIfPlayerHasPieceInInventory(Players player, Pieces piece)
        {
            Player pieceOwner = player == Players.BLACK ? _board.blackPlayer : _board.whitePlayer;
            return pieceOwner.hasPiece(piece);
        }
        bool checkIfPlayerTurn(Move move) => move.player == turn;
        bool moveIsLegal(MOVE_PIECE move)
        {
            if (!board.piecesInPlay.ContainsKey(move.origin)) return false;
            Piece piece = board.piecesInPlay[move.origin];
            List<Path> paths = Piece.getLegalMoves(piece, board);
            Path playerPath = paths[paths.FindIndex(path => path.last == move.destination)];
            List<Sylves.Cell> endpoints = paths.Select(path => path.last).ToList();
            bool correctPlayer = piece.owner == turn;
            GD.Print("Did the correct player submit the move? ", correctPlayer);
            GD.Print("Legal paths contain destination: ", endpoints.Contains(move.destination));
            GD.Print("Player path is legal: ", pathIsLegal(playerPath, piece.type));
            if (correctPlayer && endpoints.Contains(move.destination) && pathIsLegal(playerPath, piece.type)) return true;
            else return false;
        }
        bool pathIsLegal(Path path, Pieces pieceType)
        {
            //giving up on checking if paths are legal
            //if there's a bug with grasshoppers it's probably this
            //if (pieceType == Pieces.GRASSHOPPER) return true;
            //foreach ((Cell first, Cell last)  in path.pairs)
            //{
            //    if (!board.CanMoveBetween(first, last)) return false; 
            //}
            return true;
        }
        bool placementLegalityCheck(Cell destination, Players player, Pieces piece)
        {
            List<Sylves.Cell> surroundingPieces = board.getOccupiedNeighbors(destination);
            List<Piece> actualPieces = surroundingPieces.Select(cel => board.piecesInPlay[cel]).ToList();
            bool piecesBelongToMover = actualPieces.All(pieces => player == pieces.owner);
            bool playerHasPieceInHand = checkIfPlayerHasPieceInInventory(player, piece);
            GD.Print("Player has pieces to connect: ", actualPieces.Count > 0);
            GD.Print("Pieces belong to mover: ", piecesBelongToMover);
            GD.Print("Player has piece " + piece + " in inventory: ", playerHasPieceInHand);
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
                GD.Print("Debug: ", ((INITIAL_PLACE)moves[0]).destination, move.destination);
                GD.Print("Correct player: ", isCorrectUser);
                GD.Print("Initial placement adjacent to first piece:", isAdjacentToFirstPiece);
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
        bool hasPlayerWon(Players player)
        {
            List<Piece> playerPieces = board.piecesInPlay.Where(kvp => kvp.Value.owner != player).Select(kvp => kvp.Value).ToList();
            Piece bee = playerPieces.Where(pce => pce.type == Pieces.BEE).FirstOrDefault();
            if (bee == null) return false;
            if (board.getOccupiedNeighbors(bee.location).Count == 6) return true;
            else return false;
        }
        bool hasLegalPlacementTarget(Players player)
        {
            if (moves.Count <= 2) return true;
            List<Piece> playerPieces = board.piecesInPlay.Where(kvp => kvp.Value.owner == player).Select(kvp => kvp.Value).ToList();
            foreach (Piece item in playerPieces)
            {
                List<Cell> neighbors = board.getEmptyNeighbors(item.location);
                if (neighbors.Any(neigh => placementLegalityCheck(neigh, player, item.type))) return true;
            }
            return false;
        }
        bool playerHasPiecesInPlay(Players player) => board.piecesInPlay.Where(kvp => kvp.Value.owner == player).ToList().Count < 13;
        bool playerHasPossibleMoves(Players player)
        {
            List<Piece> playerPieces = board.piecesInPlay.Where(kvp => kvp.Value.owner == player).Select(kvp => kvp.Value).ToList();
            List<Path> possibleMoves = new List<Path>();
            foreach (Piece piece in playerPieces)
            {
                possibleMoves.AddRange(Piece.getLegalMoves(piece, board));
            }
            if (possibleMoves.Count == 0 || possibleMoves.All(path => path.isNullPath)) return false;
            else return true;
        }
        private HashSet<Cell> recursiveGetNeighbors(Cell cell, HashSet<Cell> memo, Cell? excludedCell)
        {
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
            List<Cell> prelim = board.piecesInPlay.Keys.ToList();
            prelim.Remove(movingPiece);
            Cell first = prelim.First();
            int target = prelim.Count;
            HashSet<Cell> computed = recursiveGetNeighbors(first, hypoHive, movingPiece);
            if (computed.Count != target) return false;
            return true;
        }
        public bool wincon_check()
        {
            List<Piece> bees = board.piecesInPlay.Where(kvp => kvp.Value.type == Pieces.BEE).Select(kvp => kvp.Value).ToList();
            if (bees.Any(piece => board.getOccupiedNeighbors(piece.location).Count == 6)) return true;
            return false;
        }
    }
}

