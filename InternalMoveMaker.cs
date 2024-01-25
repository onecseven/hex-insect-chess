using Godot;
using Hive;
using Sylves;
using System;

public partial class InternalMoveMaker : Node
{
    //find the current player
    bool midMove = false;
    int currentTurn = -1;
    Hive.Players currentPlayer = Players.BLACK;
    Nullable<Hive.MoveType> moveType = null;
    Nullable<Hive.Pieces> selectedPiece = null;
    Nullable<Cell> destination = null;
    Nullable<Cell> origin = null;
    [Signal]
    public delegate void MoveCompletedEventHandler(Hive.Move move);
    void resetMove()
    {
        moveType = null;
        selectedPiece = null;
        destination = null;
        origin = null;
    }
    public void onInventoryPieceSelected(int _piece) {
        GD.Print("INV SEL ", _piece);
        Hive.Pieces piece = (Hive.Pieces)_piece;
        if (midMove && (moveType != Hive.MoveType.PLACE || moveType != Hive.MoveType.INITIAL_PLACE)) resetMove();
        if (currentTurn < 2) moveType = Hive.MoveType.INITIAL_PLACE;
        else moveType = Hive.MoveType.PLACE;    
        selectedPiece = piece;
    }
    //signal issues on next method
    void onBoardPieceSelected(Piece piece) {
        if (midMove && moveType != Hive.MoveType.PLACE) resetMove();
        moveType = Hive.MoveType.MOVE_PIECE;
        selectedPiece = piece.type;
        origin = piece.location;
    }
    void onTileClicked(Cell _destination)
    {
        if (!midMove || !moveType.HasValue) return;
        destination = _destination;
        moveCompleted();
    }
    public void onTurnChanged(int player)
    {
        currentPlayer = (Hive.Players)player;
        currentTurn++;
    }
    void moveCompleted()
    {
        switch (moveType.Value)
        {
            case MoveType.INITIAL_PLACE:
                if (!selectedPiece.HasValue || !destination.HasValue) throw new Exception("bad initial place");
                EmitSignal(nameof(MoveCompleted), new INITIAL_PLACE(currentPlayer, selectedPiece.Value, destination.Value));
                break;
            case MoveType.PLACE:
                if (!selectedPiece.HasValue || !destination.HasValue) throw new Exception("bad place move");
                EmitSignal(nameof(MoveCompleted), new PLACE(currentPlayer, selectedPiece.Value, destination.Value));
                break;
            case MoveType.MOVE_PIECE:
                if (!selectedPiece.HasValue || !destination.HasValue || !origin.HasValue) throw new Exception("bad movepiece move");
                EmitSignal(nameof(MoveCompleted), new MOVE_PIECE(currentPlayer, selectedPiece.Value, origin.Value, destination.Value));
                break;
            case MoveType.AUTOPASS:
                break;
            default:
                break;
        }
        resetMove();
    }
}
