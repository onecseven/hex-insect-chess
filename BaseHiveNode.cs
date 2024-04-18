using Godot;
using System;

public partial class BaseHiveNode : Node2D
{
    // Called when the node enters the scene tree for the first time.
    // Called when the node enters the scene tree for the first time.
    public Hive.Hive machine
    {
        get
        {
            return gameWrapper.machine;
        }
    }
    [Export]
    public TatiHex grid = null;
    [Export]
    public GameWrapper gameWrapper = null;

    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
