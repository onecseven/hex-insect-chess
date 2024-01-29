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
    Label moveTypeLabel { get => GetChild<Label>(0); }
    Label selectedPieceLabel { get => GetChild<Label>(1); }
    Label destinationLabel { get => GetChild<Label>(2); }
    Label originLabel { get => GetChild<Label>(3); }
    Nullable<Hive.MoveType> _moveType = null;
    Nullable<Hive.MoveType> moveType { get =>_moveType ; set {
            _moveType = value;
            moveTypeLabel.Text = $"move type: {moveType.ToString()}";
        } }
    Nullable<Hive.Pieces> _selectedPiece = null;
    Nullable<Hive.Pieces> selectedPiece
    {
        get => _selectedPiece; set
        {
            _selectedPiece = value;
            selectedPieceLabel.Text = $"piece type: {selectedPiece.ToString()}";
        }
    }
    Nullable<Cell> _destination = null;
    Nullable<Cell> destination
    {
        get => _destination; set
        {
            _destination = value;
            destinationLabel.Text = $"dest: {destination.ToString()}";
        }
    }
    Nullable<Cell> _origin = null;
    Nullable<Cell> origin
    {
        get => _origin; set
        {
            _origin = value;
            originLabel.Text = $"origin: {origin.ToString()}";
        }
    }
    [Signal]
    public delegate void MoveCompletedEventHandler(Hive.Move move);
    void resetMove()
    {
        moveType = null;
        selectedPiece = null;
        destination = null;
        origin = null;
    }
    public override void _Ready()
    {
        gridMoveListener gridList = GetChild<gridMoveListener>(4);
        gridList.BoardPieceClicked += onBoardPieceSelected;
        gridList.TileClicked += onTileClicked;
    }
    public void onInventoryPieceSelected(int _piece) {
        Hive.Pieces piece = (Hive.Pieces)_piece;
        if (midMove && (moveType != Hive.MoveType.PLACE || moveType != Hive.MoveType.INITIAL_PLACE)) resetMove();
        midMove = true;
        if (currentTurn < 2) moveType = Hive.MoveType.INITIAL_PLACE;
        else moveType = Hive.MoveType.PLACE;    
        selectedPiece = piece;
    }
    //signal issues on next method
    public void onBoardPieceSelected(Vector3 _origin, int pieceType) {
        if (midMove && moveType != Hive.MoveType.PLACE) resetMove();
        midMove = true;
        moveType = Hive.MoveType.MOVE_PIECE;
        selectedPiece = (Hive.Pieces)pieceType;
        origin = HiveUtils.vec2cel(_origin);
    }
    public void onTileClicked(Vector3 _destination)
    {
        Cell dest = HiveUtils.vec2cel(_destination);
        if (!midMove || !moveType.HasValue) { resetMove(); return; }
        destination = dest;
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
        //resetMove();
    }
}
