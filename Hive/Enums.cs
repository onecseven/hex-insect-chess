using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hive
{
    #region data enums
    public enum Pieces
    {
        BEE,
        ANT,
        SPIDER,
        GRASSHOPPER,
        MOSQUITO,
        BEETLE,
        LADYBUG,
    }

    public enum Phases
    {
        PREPARED,
        INITIAL,
        WAITING_FOR_MOVE,
        GAME_OVER,
    }

    public enum Players
    {
        BLACK,
        WHITE
    }

    public enum MoveType
    {
        INITIAL_PLACE,
        PLACE,
        MOVE_PIECE,
        AUTOPASS
    }
    #endregion
}
