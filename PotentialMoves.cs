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
	[Export]
	public TatiHex grid = null;
    [Export]
	public BoardNode board = null;
    [Export]
    public Color ColorBase { get; set; } = Colors.Yellow;

    private List<Cell> potentialMoves = new List<Cell>();
    public override void _UnhandledInput(InputEvent @event)
    {
        //if (grid == null || board == null) return;

        //if (@event is InputEventMouse)
        //{
        //    Vector2 currentPosition = ((InputEventMouse)@event).Position - grid.Position;
        //    if (grid.grid.FindCell(new Vector3(currentPosition.X, currentPosition.Y, 0), out Cell lic))
        //    {
        //        if (lic == hoveringCell) return;
        //        else
        //        {
        //            potentialMoves.Clear();
        //            hoveringCell = lic;
        //            if (board.tileIsOccupied(lic) && TheHive.oneHiveRuleCheck(board, lic))
        //            {
        //                Piece piece = board.piecesInPlay[lic];
        //                potentialMoves = Piece.getLegalMoves(piece, board);
        //            }
        //            QueueRedraw();
        //        }
        //    }
        //}
    }
    public override void _Draw()
    {
        foreach (Cell cell in potentialMoves) {
            if (grid.hexes.TryGetValue(cell, out var center))
            {
                //GD.Print("potentialmoves draw ", center, cell);
                var super_shapes = grid.GetHexCornersFromCenter(new Vector2(center.X, center.Y));
                for (int i = 0; i < super_shapes.Length; i++)
                {
                    int next = i + 1 > super_shapes.Length - 1 ? 0 : i + 1;
                    DrawLine(super_shapes[i] + grid.Position, super_shapes[next] + grid.Position, ColorBase, 1);
                }
            }
        }
    }
}
