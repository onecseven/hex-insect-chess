using Godot;
using System;

public partial class MoveComposer : BaseHiveNode
{
	// Called when the node enters the scene tree for the first time.
	gridMoveListener _listener = null;
	[Export]
	gridMoveListener listener { get { return _listener; } set {
			_listener = value;
            _listener.TileClicked += TileClickedHandler;
		} }

    private void TileClickedHandler(Hive.Tile tile)
    {
		//	TODO handle clicked tiles
		//  declate a variable to track if it's the first click
		//	1. if it's the first click and the tile has a piece that's the subject
		//	flip the flag, save the subject, send the subject up to move maker
		//	2. if it's not the first click check then check if the new tile clicked
		//  is within the pieces.GetLegalMoves output
		//  2a. if it is, then send object to move maker
		//	2b. if it isn't and it's a tile with a piece in it, then
		//	do not flip the flag, replace the subject with the new subject (send up)
		//  2c. if it isn't and it's an empty tile
		//	flip the flag, replace the subject with nothing (send up)
		//	1a if it is the first click but the tile is empty
		//	do nothing
        throw new NotImplementedException();
    }

    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
