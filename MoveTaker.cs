using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;

public partial class MoveTaker : TextEdit
{
    // Called when the node enters the scene tree for the first time.

    public Hive.Hive machine
    {
        get
        {
            return gameWrapper.machine;
        }
    }
    [Export]
    public GameWrapper gameWrapper = null;

    public override void _Ready()
	{
	}
	MoveComposer _composer;	
	[Export]
	MoveComposer composer { get => _composer; set
		{
			_composer = value;
			_composer.SubjectClicked += SubjectHandler;
			_composer.ClearText += ClearTextHandler;
            _composer.ObjectClicked += ObjectHandler;	
		} }
	private bool isBeetleMove = false;
	private (Hive.Tile sub, Hive.Tile obj) pair = new();
	private void SubjectHandler(Hive.Tile tile)
	{
		this.Text = "";
		this.Text += NotationReader.toNotationSubject(tile.activePiece.type, tile.activePiece.owner, tile.activePiece.id);
        this.Text += " ";
        if (tile.activePiece.type == Hive.Pieces.BEETLE) {
			isBeetleMove = true;
		}
		pair.sub = tile;
	}


    private void ObjectHandler(Hive.Tile tile, Hive.Tile neighbor)
    {
        string newstuff = $"{NotationReader.toNotationObject(tile.cell, neighbor, isBeetleMove)}";
		this.Text += newstuff;	
		if (isBeetleMove) isBeetleMove = false;
		pair.obj = tile;
    }

	private void ClearTextHandler()
	{
        this.Text = "";
        this.isBeetleMove = false;
        this.pair = new();
    }

    public override void _UnhandledInput(InputEvent @event)
    {

        if (@event is InputEventKey && ((InputEventKey)@event).KeyLabel == Key.Enter)
        {
            var move = NotationReader.TranslateMove(this.Text.Trim(), machine.board);
            GD.Print("Sending parsed move: ", move);
            ClearTextHandler();
            machine.send_move(move);  
        }
    }
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
	}
}
