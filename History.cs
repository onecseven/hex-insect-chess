using Godot;
using System;
using System.ComponentModel;
using Hive;
using System.Collections.Generic;
using System.Linq;

public partial class History : ScrollContainer
{
    GameWrapper _gameWrapper = null;
    public Hive.Hive machine
    {
        get
        {
            return _gameWrapper.machine;
        }
    }
    [Export]
    public TatiHex grid = null;
    [Export]
    public GameWrapper gameWrapper {get => _gameWrapper; set { _gameWrapper = value; gameWrapper.onOuterSuccessfulMove += onHistoryUpdate; } }
    [Export]
    HFlowContainer historyButtonContainer = null;
    [Export]
    MoveListReceiver moveListReceiver = null;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
    public void onHistoryUpdate(Move move)
    {
        var moves = NotationReader.moveListToNotation(machine.moves);
        int i = 1;
        foreach (var child in historyButtonContainer.GetChildren())
        {
            child.QueueFree();
        }
        foreach (string token in moves)
        {
            var but = new Godot.Button();
            var curried = curry(i);
            but.Pressed += () => curried();
            but.Text = i + ": " + String.Join(" ", token);
            but.Visible = true;
            historyButtonContainer.AddChild(but);
            i++;
        }
        historyButtonContainer.AddChild(exportButton());
    }
    public Button exportButton()
    {
        Button button = new Godot.Button();
        button.Text = "Export";
        button.Pressed += () =>
        {
            string built = "";
            var reader = NotationReader.moveListToNotation(machine.moves);
            foreach (var (str, i) in reader.Select((value, i) => (value, i)))
            {
                built += $"{i + 1}: {str}\n";
            }
            moveListReceiver.Text = built;
        };
        return button;
    }

    public Func<int> curry(int i) => () => {
        var nenMoves = machine.moves.GetRange(0, i);
        gameWrapper.restore(nenMoves);
        return i;
    };
}
