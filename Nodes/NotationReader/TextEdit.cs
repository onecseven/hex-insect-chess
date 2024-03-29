using Godot;
using System;
using static NotationReader;

public partial class TextEdit : Godot.TextEdit
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void _on_text_changed()
	{
		if (NotationReader.IsValidMoveList(Text))
		{
			var tokenized = NotationReader.Tokenize(Text);
        }
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
