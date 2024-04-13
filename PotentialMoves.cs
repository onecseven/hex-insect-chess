using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PotentialMoves : Node2D
{
	// Called when the node enters the scene tree for the first time.

	private Cell hoveringCell = new Cell(0,0);
    Hive.Board board
    {
        get
        {
            return gameWrapper.machine.board;
        }
    }

    [Export]
    GameWrapper gameWrapper = null;

    [Export]
	public TatiHex grid = null;
    [Export]

    public Color ColorBase { get; set; } = Colors.Yellow;

    private List<Path> potentialMoves = new List<Path>();
    public override void _UnhandledInput(InputEvent @event)
    {
        if (grid == null || board == null) return;

        if (@event is InputEventMouse)
        {
            Cell lic = grid.mouseToCell((InputEventMouse)@event);
            if (lic != hoveringCell)
            {
                potentialMoves.Clear();
                hoveringCell = lic;
                if (board.tileIsOccupied(lic) && gameWrapper.machine.oneHiveRuleCheck(lic))
                {
                    Piece piece = board.piecesInPlay[lic].activePiece;
                    potentialMoves = Piece.getLegalMoves(piece, board);
                }
                QueueRedraw();
            }
        }
    }
    public override void _Draw()
    {
        if (potentialMoves.Count == 0) return;
        foreach (Path _path in potentialMoves) {
            if (_path.last != hoveringCell && grid.hexes.TryGetValue(_path.last, out var center))
            {
                var hexSides = grid.GetHexCornersFromCenter(new Vector2(center.X, center.Y));
                for (int i = 0; i < hexSides.Length; i++)
                {
                    int next = i + 1 > hexSides.Length - 1 ? 0 : i + 1;
                    DrawLine(hexSides[i], hexSides[next], ColorBase, 1);
                }
            }
        }
    }
}
