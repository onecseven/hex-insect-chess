using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Xml.Serialization;

public partial class Machine : Node
{
    #region declarations

    Hive.Phases _game_status = Hive.Phases.PREPARED;
    Hive.Phases game_status { get => _game_status; set
        {
            _game_status = value;
            EmitSignal(nameof(gameStatusChanged), _game_status.ToString());
        } }
    Hive.Players _turn = Hive.Players.BLACK;
    Hive.Players turn
    {
        get => _turn; set
        {
            _turn = value;
            EmitSignal(nameof(turnChanged), (int)_turn);
        }
    }
    List<Move> moves = new List<Move>();
    [Signal]
    public delegate void InitialPlacementEventHandler(Hive.INITIAL_PLACE move);
    [Signal]
    public delegate void PieceMovedEventHandler(Hive.MOVE_PIECE move);
    [Signal]
    public delegate void PlacementEventHandler(Hive.PLACE move);
    [Signal]
    public delegate void gameStatusChangedEventHandler(string game_status);
    [Signal]
    public delegate void turnChangedEventHandler(int player);

    public override void _Ready()
    {
        EmitSignal(nameof(gameStatusChanged), _game_status.ToString());
        EmitSignal(nameof(turnChanged), (int)_turn);
    }

    public void send_move(Move move)
    #endregion
    {
        if (!moveIsValid(move) || game_status == Phases.GAME_OVER)
        {
            GD.Print("Invalid move");
            return;
        }
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
            case MoveType.AUTOPASS:
                break;
        }
        moves.Add(move);
        if (wincon_check()) game_over();
        else advanceTurn();
    }

    public void advanceTurn()
    {

        turn = ((Hive.Players)((int)turn ^ 1));
        autopassCheck();
    }

    public void autopassCheck()
    {
        GD.Print("PlayerHasPieces: ", playerHasPieces(turn));
        GD.Print("HasLegalPlacementTarget: ", hasLegalPlacementTarget(turn));
        GD.Print("PlayerHasPossibleMoves: ", playerHasPossibleMoves(turn));

        if ((playerHasPieces(turn) && hasLegalPlacementTarget(turn)) || playerHasPossibleMoves(turn)) return;
        else
        {
            if (moves.Last().type == MoveType.AUTOPASS)
            {
                throw new Exception("AUTOPASS LOOP FUCK");
            }
            send_move(new AUTOPASS(turn));
        };

    }

    public void game_over()
    {
        game_status = Phases.GAME_OVER;
    }

    private bool moveIsValid(Move move)
    {
        GD.Print("turn matches player?: ", move.player == turn);
        if (move.player != turn) return false;
        switch (move.type)
        {
            case MoveType.INITIAL_PLACE:
                Hive.INITIAL_PLACE initialPlaceCasted = (Hive.INITIAL_PLACE)move;
                GD.Print("Initial Placement Check: ", initialPlacementCheck(initialPlaceCasted));
                if (!initialPlacementCheck(initialPlaceCasted)) return false;
                break;
            case MoveType.PLACE:
                Hive.PLACE placeCasted = (Hive.PLACE)move;
                GD.Print("Placement Check: ", placementLegalityCheck(placeCasted.destination, placeCasted.player));
                GD.Print("Must User Play Bee?: ", mustPlayBee(placeCasted.player));
                if (mustPlayBee(placeCasted.player) && move.piece != Pieces.BEE) return false;
                if (!placementLegalityCheck(placeCasted.destination, placeCasted.player)) return false;
                break;
            case MoveType.MOVE_PIECE:
                Hive.MOVE_PIECE moveCasted = (Hive.MOVE_PIECE)move;
                GD.Print("Move Is Legal Check: ", moveIsLegal(moveCasted));
                if (!moveIsLegal(moveCasted)) return false;
                GD.Print("OneHiveRuleCheck: ", oneHiveRuleCheck(moveCasted.origin));
                if (!oneHiveRuleCheck(moveCasted.origin))
                {
                    return false;
                }

                break;
            default:
                break;
        }
        return true;
    }

    #region validators

    /** TODO
     * try to play a full game
     * 1. turn label component.
     * 2. move history?
     */
    bool checkIfPlayerTurn(Move move) => move.player == turn;
    bool moveIsLegal(Hive.MOVE_PIECE move)
    {
        if (!board.piecesInPlay.ContainsKey(move.origin)) return false; 
        Piece piece = board.piecesInPlay[move.origin];
        List<Path> paths = Piece.getLegalMoves(piece, board);
        Path playerPath = paths[paths.FindIndex(path => path.last == move.destination)];
        List<Sylves.Cell> endpoints = paths.Select(path => path.last).ToList();
        GD.Print("Legal paths contain destination: ", endpoints.Contains(move.destination));
        GD.Print("Player path is legal: ", pathIsLegal(playerPath));
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
        GD.Print("Player has pieces?: ", actualPieces.Count > 0);
        GD.Print("Pieces belong to mover?: ", piecesBelongToMover);
        if (actualPieces.Count > 0 && piecesBelongToMover) return true;
        return false;
    }
    bool initialPlacementCheck(Hive.INITIAL_PLACE move)
    {
        if (moves.Count == 0) return true;
        else if (moves.Count == 1)
        {
            bool isCorrectUser = moves[0].player != move.player;
            bool isAdjacentToFirstPiece = board.AreCellsAdjacent(((Hive.INITIAL_PLACE)moves[0]).destination, move.destination);
            GD.Print("Correct player?: ", isCorrectUser);
            GD.Print("is initial placement adjacent to first piece?", isAdjacentToFirstPiece);
            if (isCorrectUser && isAdjacentToFirstPiece) return true;
        }
        return false;
    }
    bool mustPlayBee(Hive.Players player)
    {
        bool hasPlayerPlayedBee(Hive.Players player)
        {
            var playerMoves = moves.Where(move => move.player == player).Where(move => move.type == MoveType.INITIAL_PLACE || move.type == MoveType.PLACE);
            if (moves.Any(move => move.piece == Pieces.BEE)) return true;
            return false;
        }

        bool hasPlayedBee = hasPlayerPlayedBee(player);
        bool hasPlayedThreeMoves = (moves.Where(move => move.player == player).ToList().Count) == 3;
        bool belowLimit = (moves.Where(move => move.player == player).ToList().Count) < 3;
        if (belowLimit && hasPlayedThreeMoves && !hasPlayedBee) return true;
        else if (!belowLimit && !hasPlayedThreeMoves && !hasPlayedBee)
        {
            throw new ArgumentException("illegal game state");
        }
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
        if (moves.Count < 2) return true;
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
    #endregion

    #endregion
    public bool wincon_check()
    {
        List<Piece> bees = board.piecesInPlay.Where(kvp => kvp.Value.type == Pieces.BEE).Select(kvp => kvp.Value).ToList();
        if (bees.Any(piece => board.getOccupiedNeighbors(piece.location).Count == 6)) return true;
        return false;
    }

}
