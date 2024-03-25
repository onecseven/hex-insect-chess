using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class statusLable : Label
{

    public void onGameStatusChanged(string phase)
    {
        //GD.Print(phase);
        this.Text = $"Phase: {phase}";
    }
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
