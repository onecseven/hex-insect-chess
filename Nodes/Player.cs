using Godot;
using System;
using System.Collections.Generic;

public partial class Player : GodotObject
{
    public Dictionary<Hive.Pieces, int> inventory = new Dictionary<Hive.Pieces, int>()
    {
        [Hive.Pieces.BEE] = 1,
        [Hive.Pieces.ANT] = 3,
        [Hive.Pieces.SPIDER] = 2,
        [Hive.Pieces.BEETLE] = 2,
        [Hive.Pieces.GRASSHOPPER] = 3,
        [Hive.Pieces.MOSQUITO] = 1,
        [Hive.Pieces.LADYBUG] = 1

    };
    public Hive.Players color;

    public Player(Hive.Players _color)
    {
        color = _color;
    }

    public void piecePlaced(Hive.Pieces _piece) {
        if (hasPiece(_piece)) inventory[_piece]--;
        else throw new ArgumentOutOfRangeException("piece placed called when player is out of pieces");
      }
    public bool hasPiece(Hive.Pieces _piece) => inventory.ContainsKey(_piece) && inventory[_piece] > 0;


}
