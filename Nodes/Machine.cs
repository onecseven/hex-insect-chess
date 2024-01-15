using Godot;
using Hive;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Machine : Node
{
    //todo
    //advance phase
    //advance turn
    /* Move validators 
		0. The moves can only be emitted for players who match the turn color [x] (check if playerturn)
		1. placement must check that the location in the Move
	    only be adjacent to pieces that are the same color as
	    the player who sent the move [x] (placementLegalityCheck)
			1a. initial placement is the only placement that can start adjacent to the opposite color
		2. move must be legal (as in the output of Piece.getLegalMoves must include the endpoint)
			2a. move must have the correct origin for the piece
			2b. each step in the path must abide by the freedom to move rule
			2c. the move cannot at any point break the one hive rule
		3. if the first three moves of the game for any particular player do not include a placement for the bee, then the only valid fourth move is bee placement
		4. moves must check for wincon status after every move happens
		5. after turn pass but before the game idles to receive moves, it must check that the player who's about to move has any legal moves. if they do not, then it autopasses.
	 */
    Hive.Phases game_status = Hive.Phases.PREPARED;
	Hive.Players turn = Hive.Players.BLACK;
	List<Move> moves = new List<Move>();


	[Signal]
	public delegate void InitialPlacementEventHandler(Hive.INITIAL_PLACE move);

	[Signal]
	public delegate void PieceMovedEventHandler(Hive.MOVE_PIECE move);

	[Signal]
    public delegate void PlacementEventHandler(Hive.PLACE move);


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

	bool checkIfPlayerTurn(Move move) => move.player == turn;
	bool placementLegalityCheck(Hive.PLACE move) {
		List<Sylves.Cell> surroundingPieces = board.getOccupiedNeighbors(move.destination);
		List<Piece> actualPieces = surroundingPieces.Select(cel => board.piecesInPlay[cel]).ToList();
		bool piecesBelongToMover = actualPieces.All(pieces => move.player == pieces.owner);
		if (actualPieces.Count > 0 && piecesBelongToMover) return true;
		return false;
	}
	bool initialPlacementCheck(Hive.PLACE move)
	{
		if (moves.Count == 0) return true;
		else if (moves.Count == 1)
		{
			bool isCorrectUser = moves[0].player != move.player;
			bool isAdjacentToFirstPiece = board.connectingAdjacents
				//(((Hive.PLACE)moves[0]).destination).cp
		}
	}
	public bool wincon_check() {
        throw new NotImplementedException();
    }

}
