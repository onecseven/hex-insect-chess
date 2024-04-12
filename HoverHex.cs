using Godot;
using Sylves;
using System;

public partial class HoverHex : Node2D
{


    private Sylves.Cell hoveringCell = new Cell(0, 0);


    [Export]
     TatiHex _hexgrid = null;

    public TatiHex hexgrid
    {
        get => _hexgrid; set
        {
            _hexgrid = value;
            this.Position = _hexgrid.Position;
        }
    }

    [Export]
    public Color ColorBase { get; set; } = Colors.Yellow;


    public override void _Ready()
    {
    }
    public override void _UnhandledInput(InputEvent @event)
    {
    	if (hexgrid == null)
        {
            GD.PrintErr("No grid in HoverHex");
            return;
        }

        if (@event is InputEventMouse)
        {
            Vector2 currentPosition = ((InputEventMouse)@event).GlobalPosition - hexgrid.Position;
            var lic = hexgrid.FindCell(new Vector2(currentPosition.X, currentPosition.Y));
            if (lic != null)
            {
                if (lic == hoveringCell) return;
                else
                {
                    hoveringCell = (Cell)lic;
                    QueueRedraw();
                }
            }
        }
    }
    public override void _Draw()
    {
        if (hexgrid.hexes.ContainsKey(hoveringCell))
        {
            Vector3 center = hexgrid.hexes[hoveringCell];
            var super_shapes = hexgrid.GetHexCornersFromCenter(new Vector2(center.X, center.Y));
            for (int i = 0; i < super_shapes.Length; i++)
            {
                int next = i + 1 > super_shapes.Length - 1 ? 0 : i + 1;
                DrawLine(super_shapes[i], super_shapes[next], ColorBase, 1);
            }
        }
        return;
    }
}
