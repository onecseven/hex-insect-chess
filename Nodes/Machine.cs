using Godot;
using Hive;
using System;
using System.Collections.Generic;

public partial class Machine : Node
{
	//todo
	//advance phase
	//advance turn
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

	public bool wincon_check() {
        throw new NotImplementedException();
    }

}
