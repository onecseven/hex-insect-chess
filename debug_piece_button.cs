using Godot;
using System;

[SceneTree]
public partial class debug_piece_button : HFlowContainer
{
    [OnInstantiate]
    private void Initialise(Hive.Pieces pieceType, int amount)
    {
        _piece = pieceType;
        this.GetChild<TextureButton>(0).TextureNormal = Hive.Piece._textures[pieceType];
        this.GetChild<Label>(1).Text = $"x {amount}";

    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
    }

    [Signal]
    public delegate void ButtonPressedEventHandler(int pieceType);

	Hive.Pieces _piece;
	int amount;

    public void _on_texture_button_pressed()
	{
		EmitSignal(nameof(ButtonPressed), (int)_piece);
		GD.Print("emitted");
	}
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
