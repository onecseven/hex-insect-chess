using Godot;
using System;

public partial class playerLable : Label
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public void onTurnChanged(int player)
    {
        GD.Print(player);
        this.Text = $"Players: {(Hive.Players)player}";
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
