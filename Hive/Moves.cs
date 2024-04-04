using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive
{
    #region moves
    public partial class Move : GodotObject
    {
        public Players player;
        public Pieces piece;
        public MoveType type;
        public Move(Players player, Pieces piece, MoveType type)
        {
            this.player = player;
            this.piece = piece;
            this.type = type;
        }
    }
    public partial class PLACE : Move
    {
        public Cell destination;
        public PLACE(Players player, Pieces piece, Cell destination) : base(player, piece, MoveType.PLACE)
        {
            this.destination = destination;
        }
        public override string ToString() => $"{player} PLACES {piece} AT {destination}";

    }
    public partial class INITIAL_PLACE : Move
    {
        public Cell destination;
        public INITIAL_PLACE(Players player, Pieces piece, Cell destination) : base(player, piece, MoveType.INITIAL_PLACE)
        {
            this.destination = destination;
        }
        public override string ToString() => $"{player} PLACES {piece} AT {destination}";

    }
    public partial class MOVE_PIECE : Move
    {
        public Cell origin;
        public Cell destination;
        public MOVE_PIECE(Players player, Pieces piece, Cell origin, Cell destination) : base(player, piece, MoveType.MOVE_PIECE)
        {
            this.destination = destination;
            this.origin = origin;
        }
        public override string ToString() => $"{player} MOVES {piece} AT {origin} TO {destination}";
    }
    public partial class AUTOPASS : Move
    {
        public AUTOPASS(Players player) : base(player, Pieces.MOSQUITO, MoveType.AUTOPASS)
        {

        }
    }
    #endregion

}
