using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

public partial class Machine : Node
{
    #region declarations
    Hive.Phases game_status = Hive.Phases.PREPARED;
    Hive.Players turn = Hive.Players.BLACK;
    List<Move> moves = new List<Move>();


    [Signal]
    public delegate void InitialPlacementEventHandler(Hive.INITIAL_PLACE move);

    [Signal]
    public delegate void PieceMovedEventHandler(Hive.MOVE_PIECE move);

    [Signal]
    public delegate void PlacementEventHandler(Hive.PLACE move);
    #endregion

    public void send_move(Move move)
    {
        switch (move.type)
        {
            case MoveType.INITIAL_PLACE:
                EmitSignal(nameof(InitialPlacement), move);
                break;
            case MoveType.MOVE_PIECE:
                EmitSignal(nameof(PieceMoved), move);
                break;
            case MoveType.PLACE:
                EmitSignal(nameof(Placement), move);
                break;
        }
    }

    #region validators
    /* Move validators 
     * finished
		0. The moves can only be emitted for players who match the turn color [x] (check if playerturn)
		1. placement must check that the location in the Move
	    only be adjacent to pieces that are the same color as
	    the player who sent the move [x] (placementLegalityCheck)
			1a. initial placement is the only placement that can start adjacent to the opposite color [x] (initialplacementlegality check)
		3. if the first three moves of the game for any particular player do not include a placement for the bee, then the only valid fourth move is bee placement [x] hasPlayerPlayedBee and mustPlayBee
		4. moves must check for wincon status after every move happens [x]
    * TODO
		2. move must be legal (as in the output of Piece.getLegalMoves must include the endpoint) [move is legal]
			2a. move must have the correct origin for the piece [move is legal]
			2b. each step in the path must abide by the freedom to move rule [path is legal]
			2c. the move cannot at any point break the one hive rule [uh gotta implement oneHiveRuleCheck somewhere] (first move check)
		5. after turn pass but before the game idles to receive moves, [before move gets submitted, eg: on phase change]
            it must check that the player who's about to move has any legal moves. if they do not, then it autopasses. [autopass check region]
     */
    bool checkIfPlayerTurn(Move move) => move.player == turn;

    bool moveIsLegal(Hive.MOVE_PIECE move)
    {
        if (!board.piecesInPlay.ContainsKey(move.origin)) return false; 
        Piece piece = board.piecesInPlay[move.origin];
        List<Path> paths = Piece.getLegalMoves(piece, board);
        Path playerPath = paths[paths.FindIndex(path => path.last == move.destination)];
        List<Sylves.Cell> endpoints = paths.Select(path => path.last).ToList();
        if (endpoints.Contains(move.destination) && pathIsLegal(playerPath)) return true;
        else return false;
    }

    bool pathIsLegal(Path path)
    {
        foreach ((Cell first, Cell last)  in path.pairs)
        {
            if (!board.CanMoveBetween(first, last)) return false; 
        }
        return true;
    }
    bool placementLegalityCheck(Cell destination, Hive.Players player)
    {
        List<Sylves.Cell> surroundingPieces = board.getOccupiedNeighbors(destination);
        List<Piece> actualPieces = surroundingPieces.Select(cel => board.piecesInPlay[cel]).ToList();
        bool piecesBelongToMover = actualPieces.All(pieces => player == pieces.owner);
        if (actualPieces.Count > 0 && piecesBelongToMover) return true;
        return false;
    }
    bool initialPlacementCheck(Hive.PLACE move)
    {
        if (moves.Count == 0) return true;
        else if (moves.Count == 1)
        {
            bool isCorrectUser = moves[0].player != move.player;
            bool isAdjacentToFirstPiece = board.AreCellsAdjacent(((Hive.INITIAL_PLACE)moves[0]).destination, move.destination);
            if (isCorrectUser && isAdjacentToFirstPiece) return true;
        }
        return false;
    }
    bool hasPlayerPlayedBee(Hive.Players player)
    {
        var playerMoves = moves.Where(move => move.player == player).Where(move => move.type == MoveType.INITIAL_PLACE || move.type == MoveType.PLACE);
        if (moves.Any(move => move.piece == Pieces.BEE)) return true;
        return false;
    }
    bool mustPlayBee(Hive.Players player)
    {
        bool hasPlayedBee = hasPlayerPlayedBee(player);
        bool hasPlayedThreeMoves = (moves.Where(move => move.player == player).ToList().Count) == 3;
        bool belowLimit = (moves.Where(move => move.player == player).ToList().Count) > 3;
        if (belowLimit && hasPlayedThreeMoves && !hasPlayedBee) return true;
        else if (!belowLimit && !hasPlayedThreeMoves && !hasPlayedBee) throw new ArgumentException("illegal game state");
        return false;
    }
    bool hasPlayerWon(Hive.Players player)
    {
        List<Piece> playerPieces = board.piecesInPlay.Where(kvp => kvp.Value.owner != player).Select(kvp => kvp.Value).ToList();
        Piece bee = playerPieces.Where(pce => pce.type == Pieces.BEE).FirstOrDefault();
        if (bee == null) return false;
        if (board.getOccupiedNeighbors(bee.location).Count == 6) return true;
        else return false;
    }

    //autopass check
    #region autopass checks
    //only look into this if piece has no moves available
    bool hasLegalPlacementTarget (Hive.Players player)
    {
        List<Piece> playerPieces = board.piecesInPlay.Where(kvp => kvp.Value.owner == player).Select(kvp => kvp.Value).ToList();
        foreach (Piece item in playerPieces)
        {
            List<Cell> neighbors = board.getEmptyNeighbors(item.location);
            if (neighbors.Any(neigh => placementLegalityCheck(neigh, player))) return true;
        }
        return false;
    }
    bool playerHasPieces(Hive.Players player) => board.piecesInPlay.Where(kvp => kvp.Value.owner == player).ToList().Count < 13;
    bool playerHasPossibleMoves(Hive.Players player)
    {
        List<Piece> playerPieces = board.piecesInPlay.Where(kvp => kvp.Value.owner == player).Select(kvp => kvp.Value).ToList();
        List<Path> possibleMoves = new List<Path>();
        foreach (Piece piece in playerPieces) {
            possibleMoves.AddRange(Piece.getLegalMoves(piece, board));
        }
        if (possibleMoves.Count == 0 || possibleMoves.All(path => path.isNullPath)) return false;
        else return true;
    }
    #endregion

    #endregion
    public bool wincon_check()
    {
        throw new NotImplementedException();
    }

}
