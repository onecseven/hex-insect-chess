using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;
using System;
using HexDemo3;

public partial class PiecePainter : Node2D
{

    GameWrapper _parent = null;

    [Export]
    public Font font = ThemeDB.FallbackFont;

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
    private string dots(int i) {
        switch (i)
        {
            case 0:
                return "";
            case 1:
                return "∙";
            case 2:
                return "∶";
            case 3:
                return "∴";
            default:
                return "";
        }
    }
    public override void _Draw()
	{
        if (parent == null)
        {
            GD.PrintErr("BoardNode Doesn't Have Gamewrapper Attached.");
        }
        List<Tile> pieces = parent.machine.board.filteredPiecesInPlay.Select(cell => parent.machine.board.piecesInPlay[cell]).ToList();
        foreach (Tile tile in pieces)
        {
            Color color = tile.activePiece.owner == Players.WHITE ? Color.FromHtml("d9a066") : Color.FromHtml("663931");
            Vector2 center = HiveUtils.Vector3ToVector2(drawnGrid.hexes[tile.cell]) + drawnGrid.Position;
            Vector2 offset = (tile.activePiece.type == Pieces.BEE || tile.activePiece.type == Pieces.LADYBUG || tile.activePiece.type == Pieces.MOSQUITO) ? new Vector2(16,16) : new Vector2(24,16);
            DrawColoredPolygon(HexUtils.HexCornersFromCenter(center, drawnGrid._hexsize / 2, drawnGrid.orientation), color);
            DrawTexture(tile.activePiece.texture, center - offset);
            DrawString(ThemeDB.FallbackFont, center + new Vector2(8,12), $"{dots(tile.activePiece.id)}", HorizontalAlignment.Left, -1, 30, Color.FromHtml("000000"));
        }
    }
}
