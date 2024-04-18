using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class gridMoveListener : BaseHiveNode
{
    // Called when the node enters the scene tree for the first time.
 
    public delegate void TileClickedEventHandler(Tile tile);
    public event TileClickedEventHandler TileClicked;
    public override void _UnhandledInput(InputEvent @event)
    {

        if (@event is InputEventMouseButton && ((InputEventMouseButton)@event).Pressed)
        {
            Sylves.Cell currentPosition = grid.mouseToCell((InputEventMouse)@event);
            Tile clickedTile = machine.board.piecesInPlay[currentPosition];
            TileClicked?.Invoke(clickedTile);
        }
    }
}
