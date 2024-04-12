using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class gridMoveListener : Node2D
{
	// Called when the node enters the scene tree for the first time.

	//private Cell hoveringCell = new Cell(0,0);
	//[Export]
	//public TatiHex grid = null;
 //   [Export]
	//public BoardNode board = null;
 //   //[Export]
 //   //public Color ColorBase { get; set; } = Colors.Yellow;
 //   //[Export]
 //   //public Machine machine = null;
 //   [Signal]
 //   public delegate void TileClickedEventHandler(Vector3 cell);
 //   [Signal]
 //   public delegate void BoardPieceClickedEventHandler(Vector3 origin, int pieceType);
 //   public override void _UnhandledInput(InputEvent @event)
 //   {

 //       if (@event is InputEventMouseButton && ((InputEventMouseButton)@event).Pressed)
 //       {
 //           Vector2 currentPosition = ((InputEventMouse)@event).Position - grid.Position;
 //           if (grid.grid.FindCell(new Vector3(currentPosition.X, currentPosition.Y, 0), out Cell lic))
 //           {
 //             if (!board.tileIsOccupied(lic)) {
 //               EmitSignal(nameof(TileClicked), new Vector3(lic.x, lic.y, lic.z));
 //             } else {
 //               var piece = board.piecesInPlay[lic];
 //               EmitSignal(nameof(BoardPieceClicked), new Vector3(piece.location.x, piece.location.y, piece.location.z), (int)piece.type);
 //             }
 //           }
 //       }
 //    }
}
