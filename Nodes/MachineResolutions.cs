using Godot;
using Hive;
using System;
using System.Collections.Generic;

public partial class Machine : Node
{ 
    BoardNode _board = null;
    [Export]
    BoardNode board { get => _board; set {
            _board = value;
            InitialPlacement += _board.initialPlace;
            Placement += _board.place;
            PieceMoved += _board.move;
        }}

    private void Machine_Placement(PLACE move)
    {
        throw new NotImplementedException();
    }


    [Export]
    TatiHex world;

    //private void ResolveMove(Move move)
    //{

    //}

    //private void ResolveInitialPlace(INITIAL_PLACE move)
    //{
    //    if (board == null) return;
    //}
    //private void ResolvePlace(PLACE move)
    //{
    //    if (board == null) return;
    //    board.place(new Piece(move.piece, move.player, move.destination));
    //}

}
