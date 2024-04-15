using Godot;
using System;
using static System.Net.Mime.MediaTypeNames;
using System.ComponentModel;

public partial class History : ScrollContainer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    //public void _on_text_changed()
    //{
    //    if (NotationReader.IsValidMoveList(Text))
    //    {
    //        tokenized = NotationReader.Tokenize(Text);
    //        var parsed = NotationReader.Parser(tokenized);
    //        moves = NotationReader.Translator(parsed);
    //        int i = 1;
    //        foreach (var child in container.GetChildren())
    //        {
    //            RemoveChild(child);
    //            child.QueueFree();
    //        }
    //        foreach (var token in tokenized)
    //        {
    //            var but = new Godot.Button();
    //            but.Text = i + ". " + String.Join(" ", token);
    //            but.Visible = true;
    //            container.AddChild(but);
    //            i++;
    //        }
    //    }

    }
