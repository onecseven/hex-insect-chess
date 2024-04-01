using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;
using static NotationReader;

public partial class TextEdit : Godot.TextEdit
{
	// TODO
	// Add the index value to the buttons.
	// Flush buttons when new movelists are validated.
	// Buttons on pressed should be connectedd to a "travelto" function on this class
	// We should export a machine that we can use to send moves to and from
    [Export]
    HFlowContainer container = null;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	}

	public void _on_text_changed()
	{
		if (NotationReader.IsValidMoveList(Text))
		{
			var tokenized = NotationReader.Tokenize(Text);
			var parsed = NotationReader.Parser(tokenized);
			var translated = NotationReader.Translator(parsed);
			int i = 1;
            foreach (var token in tokenized)
            {
                var but = new Godot.Button();
				but.Text = i + ". " + String.Join(" ", token);
				but.Visible = true;
                container.AddChild(but);
				i++;
            }
        }
	}

	public void populate(List<Hive.Move> moves)
	{

	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
