using Godot;
using Hive;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class MoveComposer : BaseHiveNode
{
	// Called when the node enters the scene tree for the first time.
	gridMoveListener _listener = null;
	[Export]
	gridMoveListener listener { get { return _listener; } set {
			_listener = value;
            _listener.TileClicked += TileClickedHandler;
		} }
	private bool isFirstClick = true;
	private Hive.Tile savedPiece = null;

	public delegate void ClearTextEventHandler();
    public delegate void SubjectEventHandler(Tile tile);
    public delegate void ObjectEventHandler(Tile tile, Tile neighbor);
	public event SubjectEventHandler SubjectClicked;
	public event ObjectEventHandler ObjectClicked;
	public event ClearTextEventHandler ClearText;
	private void TileClickedHandler(Hive.Tile tile)
	{
		if (isFirstClick && tile.isOccupied && tile.activePiece.owner == machine.turn)
		{
			isFirstClick = false;
			savedPiece = tile;
			SubjectClicked?.Invoke(tile);
			return;
		}
		 else if (isFirstClick == false && savedPiece == tile)
		{
			isFirstClick = true;
			savedPiece = null;
			ClearText?.Invoke();
			return;
		} else if (isFirstClick == false && savedPiece != null)
        {
            ObjectClicked?.Invoke(tile, machine.board.getOccupiedNeighbors(tile.cell).Select(cell => machine.board.piecesInPlay[cell]).ToList()[0]);
            isFirstClick = true;
            savedPiece = null;
			return;
        }
    }

    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
