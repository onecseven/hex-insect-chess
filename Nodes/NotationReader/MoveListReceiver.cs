using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;
using static NotationReader;

public partial class MoveListReceiver : Godot.TextEdit
{
	[Export]
	HFlowContainer container = null;
	Button _nextButton = null;
	[Export]
	Button nextButton
	{
		get => _nextButton; set
		{
			_nextButton = value;
			_nextButton.Pressed += sendNextMove;
		}
	}

	//[Export]
	//Machine machine = null;

	Hive.Hive machine { get {
			return gameWrapper.machine;
		} }

	GameWrapper _gameWrapper = null;

	[Export]
	GameWrapper gameWrapper = null;

	// Called when the node enters the scene tree for the first time.

	public List<List<string>> tokenized = null;
	public List<Move> moves = null;
	public int lastMoveSent = -1;

	public void sendNextMove()
	{
		if (moves == null || machine == null || lastMoveSent == (moves.Count - 1)) return;
		machine.send_move(moves[lastMoveSent + 1]);
		lastMoveSent++;
	}
	public override void _Ready()
	{
		//machine.onSuccessfulMove += eventcheck;
		_on_text_changed();
	}

	public void _on_text_changed()
	{
		if (NotationReader.IsValidMoveList(Text))
		{
			moves = NotationReader.formattedListToMoves(Text);
			gameWrapper.reset();
			var tokenized = NotationReader.Tokenize(Text);
			int i = 1;
			foreach (var child in container.GetChildren())
			{
				//RemoveChild(child);
				child.QueueFree();
			}
			foreach (var token in tokenized)
			{
				var but = new Godot.Button();
				var curried = curry(i);
				but.Pressed += () => curried();
				but.Text = i + ". " + String.Join(" ", token);
				but.Visible = true;
				container.AddChild(but);
				i++;
			}
		}
	}
	public Func<int> curry(int i) => () => {
		var nenMoves = moves.GetRange(0, i);
		lastMoveSent = i-1;
		gameWrapper.restore(nenMoves);
        return i; };

    private void But_Pressed()
    {
		GD.Print("here");
    }

    public void populate(List<Hive.Move> moves)
	{

	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
