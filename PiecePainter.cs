using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;
using System;

public partial class PiecePainter : Node2D
{

    GameWrapper _parent = null;

    [Export]
    public GameWrapper parent {
        get => _parent; set
        {
            _parent = value;
            _parent.machine.onSuccessfulMove += onSuccessfulMoveHandler;
        }
    }

    [Export]
    public TatiHex drawnGrid = null;

    public void onSuccessfulMoveHandler(Move move) => QueueRedraw();
    public override void _Draw()
	{
        if (parent == null)
        {
            GD.PrintErr("BoardNode Doesn't Have Gamewrapper Attached.");
        }
        List<Tile> pieces = parent.machine.board.filteredPiecesInPlay.Select(cell => parent.machine.board.piecesInPlay[cell]).ToList();
        foreach (Tile tile in pieces)
        {
                Vector3 center = drawnGrid.hexes[tile.cell];
                DrawTexture(tile.activePiece.texture, (HiveUtils.Vector3ToVector2(center) + drawnGrid.Position) - new Vector2(16,16));
        }
	}
}
