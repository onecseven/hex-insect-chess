using Godot;
using System;
using System.Collections.Generic;

public partial class GameWrapper : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public Hive.Hive _machine = new Hive.Hive();
	public Hive.Hive machine { get => _machine; set {
			_machine = value;
            _machine.onSuccessfulMove += outerOnSuccessfulMove;
			_machine.onGameOver += () => GD.Print("game over!");
            _machine.onFailedMove += _machine_onFailedMove;
		} }

    private void _machine_onFailedMove(string error)
    {
		GD.PrintErr(error);
    }

    public event Hive.Hive.onSuccessfulMoveEventHandler onOuterSuccessfulMove;
    private void outerOnSuccessfulMove(Hive.Move move)
    {
		onOuterSuccessfulMove(move);
    }

	public override void _Ready()
	{
		machine.onSuccessfulMove += outerOnSuccessfulMove;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void reset ( )
	{
		machine = new Hive.Hive();
	}
    public void restore(List<Hive.Move>  moves)
    {
        machine = new Hive.Hive();
        foreach (var move in moves)
        {
			machine.send_move(move);
        }
	}
}
