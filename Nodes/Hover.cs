using Godot;
using HexDemo3;
using Sylves;
using System;
using static Godot.TextServer;

public partial class Hover : Node2D
{
	// Called when the node enters the scene tree for the first time.

	private Cell hoveringCell = new Cell(0,0);
    [Export]
    public TatiHex grid = null;
    [Export]
    public Color ColorBase { get; set; } = Colors.Yellow;


    public override void _Ready()
	{
		this.Position = grid.Position;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _UnhandledInput(InputEvent @event)
	{
		if (grid == null) return;
        if (@event is InputEventMouse)
		{
			Vector2 currentPosition = ((InputEventMouse)@event).Position - grid.Position;
			if (grid.grid.FindCell(new Vector3(currentPosition.X, currentPosition.Y, 0), out Cell lic)) {

                if (lic == hoveringCell) return;
				else
				{
                    hoveringCell = lic;
					QueueRedraw();
				}
		}
		}
	}

    public override void _Draw()
    {
		if (grid.hexes.ContainsKey(hoveringCell))
		{
            Vector3 center = grid.hexes[hoveringCell];
			var super_shapes = grid.GetHexCornersFromCenter(new Vector2(center.X, center.Y));
			for (int i = 0; i < super_shapes.Length; i++)
			{
				int next = i + 1 > super_shapes.Length - 1 ? 0 : i + 1;
				DrawLine(super_shapes[i], super_shapes[next], ColorBase, 1);
			}
		}
		return;
    }
}
