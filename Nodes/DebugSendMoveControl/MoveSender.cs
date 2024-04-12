using Godot;
using Hive;
using Sylves;
using System;

public partial class MoveSender : VFlowContainer
{
    // Called when the node enters the scene tree for the first time.
    //[Export]
    //Machine machine = null;
    //private Hive.Players selectedPlayers
    //{
    //    get
    //    {
    //        OptionButton PlayerOption = this.GetNode<OptionButton>("PlayerOption");
    //        return (Hive.Players)PlayerOption.GetSelectedId();
    //    }
    //}

    //private Hive.MoveType selectedMoveType
    //{
    //    get
    //    {
    //        OptionButton MoveTypeOption = this.GetNode<OptionButton>("MoveTypeOption");
    //        return (Hive.MoveType)MoveTypeOption.GetSelectedId();
    //    }
    //}

    //private Hive.Pieces selectedPiece
    //{
    //    get
    //    {
    //        OptionButton PiecesOption = this.GetNode<OptionButton>("PieceOption");
    //        return (Hive.Pieces)PiecesOption.GetSelectedId();
    //    }
    //}

    //private Cell destination
    //{
    //    get
    //    {
    //        SpinBox x = this.GetNode<SpinBox>("destination/x");
    //        SpinBox y = this.GetNode<SpinBox>("destination/y");
    //        return new Cell(Convert.ToInt32(x.Value), Convert.ToInt32(y.Value), Convert.ToInt32(-x.Value - y.Value));
    //    }
    //}

    //private Cell origin
    //{
    //    get
    //    {
    //        SpinBox x = this.GetNode<SpinBox>("origin/x");
    //        SpinBox y = this.GetNode<SpinBox>("origin/y");
    //        return new Cell(Convert.ToInt32(x.Value), Convert.ToInt32(y.Value), Convert.ToInt32(- x.Value - y.Value));
    //    }
    //}

    //public override void _Ready()
    //{
    //    OptionButton MoveTypeOption = this.GetNode<OptionButton>("MoveTypeOption");
    //    OptionButton PlayerOption = this.GetNode<OptionButton>("PlayerOption");
    //    OptionButton PieceOption = this.GetNode<OptionButton>("PieceOption");
    //    Button sendButton = this.GetNode<Button>("send");
    //    sendButton.Pressed += buildMove;
    //    foreach (Hive.Players player in Enum.GetValues(typeof(Hive.Players)))
    //    {
    //        PlayerOption.AddItem(player.ToString());
    //    }
    //    foreach (Hive.Pieces piece in Enum.GetValues(typeof(Hive.Pieces)))
    //    {
    //        PieceOption.AddItem(piece.ToString());
    //    }
    //    foreach (Hive.MoveType move in Enum.GetValues(typeof(Hive.MoveType)))
    //    {
    //        MoveTypeOption.AddItem(move.ToString());
    //    }

    //}

    //public void _on_move_type_option_item_selected(int idx)
    //{
    //    HFlowContainer origin = this.GetNode<HFlowContainer>("origin");

    //    switch ((Hive.MoveType)idx)
    //    {
    //        case Hive.MoveType.INITIAL_PLACE:
    //            break;
    //        case Hive.MoveType.PLACE:
    //            break;
    //        case Hive.MoveType.MOVE_PIECE:
    //            origin.Visible = true;
    //            break;
    //        default:
    //            origin.Visible = false;
    //            break;
    //    }
    //}
    //public void buildMove()
    //{
    //    if (machine == null) return;
    //    //GD.Print(selectedPlayers);
    //    //GD.Print(selectedPiece);
    //    //GD.Print(selectedMoveType);
    //    //GD.Print(destination);
    //    //GD.Print(origin);
    //    switch (selectedMoveType)
    //    {
    //        case Hive.MoveType.INITIAL_PLACE:
    //            machine.send_move(new INITIAL_PLACE(selectedPlayers, selectedPiece, destination));
    //            break;
    //        case Hive.MoveType.PLACE:
    //            machine.send_move(new PLACE(selectedPlayers, selectedPiece, destination));
    //            break;
    //        case Hive.MoveType.MOVE_PIECE:
    //            machine.send_move(new MOVE_PIECE(selectedPlayers, selectedPiece, origin, destination));
    //            break;
    //        default:
    //            throw new Exception("absolutely should not happen");
    //    }
    //}
}
