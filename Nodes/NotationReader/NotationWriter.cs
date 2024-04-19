using Godot;
using Hive;
using Sylves;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static NotationReader;

public partial class NotationReader
{
    public static Dictionary<Hive.Pieces, string> reversedPieceDict = pieceDict.ToDictionary(x => x.Value, x => x.Key);
    Hive.Pieces[] multiples = {
        Hive.Pieces.ANT,
        Hive.Pieces.SPIDER,
        Hive.Pieces.BEETLE,
        Hive.Pieces.GRASSHOPPER,
    };

    public static Dictionary<Pieces, int> reference = new Dictionary<Pieces, int>()
    {
        [Pieces.BEE] = 1,
        [Pieces.MOSQUITO] = 1,
        [Pieces.LADYBUG] = 1,
        [Pieces.SPIDER] = 2,
        [Pieces.BEETLE] = 2,
        [Pieces.ANT] = 3,
        [Pieces.GRASSHOPPER] = 3

    };
    public static string toNotationSubject(Pieces piece, Players player, int id = 0) => $"{(player == Players.WHITE ? 'w' : 'b')}{reversedPieceDict[piece]}{(id == 0 ? "" : id)}";

    public static string toNotationObject(Cell location, Tile reference, bool isBeetleMove)
    {
        string prelim = toNotationSubject(reference.activePiece.type, reference.activePiece.owner, reference.activePiece.id);
        if (isBeetleMove) return prelim;
        foreach (FTHexCorner hexCorner in HiveUtils.corners.Keys)
        {
            Cell toCheck = getCellFromDirection(location, hexCorner);
            if (reference.cell == toCheck)
            {
                switch (hexCorner)
                {
                    case FTHexCorner.Right:
                        //it's to the left of the other guy
                        return $"-{prelim}";
                    case FTHexCorner.Left:
                        //fixed
                        return $"{prelim}-";
                    case FTHexCorner.UpRight:
                        //fixed
                        return $"/{prelim}";
                    case FTHexCorner.DownRight:
                        //fixed
                        return $"\\{prelim}";
                    case FTHexCorner.DownLeft:
                        //fixed
                        return $"{prelim}/";
                    case FTHexCorner.UpLeft:
                        //fixed
                        return $"{prelim}\\";
                }
            }
        }
        throw new Exception("weird happening");
    }

    public static Subject parseOuter(string lowercase, bool isSubject)
    {
        Regex template = new Regex(@"([\/\\-])?([wb]{1}){1}([SAQBMLGPsaqbmlgp]{1})([123]{1})?([\/\\-])?|([\.])");
        System.Text.RegularExpressions.Match item = template.Match(lowercase);
        if (item.Success) {
            var values = item.Groups.Values.ToList();
            bool hasDir = values[1].Success || values[5].Success;
            bool isDot = values[6].Success;
            if (isDot) return new NullObjet();
            bool isLeft = values[1].Success;
            string leftMarker = values[1].Value;
            string _player = values[2].Value;
            string _piece = values[3].Value;
            bool hasNum = values[4].Success;
            string _num = values[4].Value;
            string rightMarker = values[5].Value;
            Players player = _player == "w" ? Hive.Players.WHITE : Hive.Players.BLACK;
            Pieces piece = pieceDict[_piece.ToUpper()];
            int num = hasNum ? int.Parse(_num) : 0;
            if (!isSubject && hasDir)
            {
                return new Objet()
                {
                    numMarker = num,
                    playerMarker = player,
                    pieceMarker = piece,
                    positionalMarker = parseDirection(isLeft, isLeft ? leftMarker : rightMarker)
                };
            }
            else if (!isSubject && !hasDir) {
                return new Objet()
                {
                    numMarker = num,
                    playerMarker = player,
                    pieceMarker = piece,
                    onTop = true
                };
            } else if (isSubject)
            {
                return new Subject()
                {
                    numMarker = num,
                    playerMarker = player,
                    pieceMarker = piece,
                };
            }
        }
        return null;
    }
    public static Hive.Move TranslateMove(string move, Hive.Board context)
    {
        bool samePiece(Subject a, Piece b) => a.playerMarker == b.owner && a.pieceMarker == b.type && a.numMarker == b.id;
        var tokenized = TokenizeMoveRaw(move);
        var parsed = (parseOuter(tokenized[0], true), parseOuter(tokenized[1], false));
        Subject subj = parsed.Item1;
        Objet objet = (Objet)parsed.Item2;
        if (objet is NullObjet)
        {
            return new INITIAL_PLACE(subj.playerMarker, subj.pieceMarker, new Sylves.Cell(4, 5, -9));
        }
        List<Tile> pieces = context.filteredPiecesInPlay
            .Select(cell => context.piecesInPlay[cell]).ToList();
        bool alreadyExists = pieces
            .Any(tile => samePiece(subj, tile.activePiece));
        Tile destination = pieces.Find(tile => samePiece(objet, tile.activePiece));
        if (destination != null)
        {
            Cell shifted = getCellFromDirection(destination.cell, objet.positionalMarker);
            if (context.filteredPiecesInPlay.Count == 1)
            {
                return new INITIAL_PLACE(subj.playerMarker, subj.pieceMarker, shifted);
            }
            switch (alreadyExists)
            {
                case true:
                    Tile origin = pieces.Find(tile => samePiece(subj, tile.activePiece));
                    return new Hive.MOVE_PIECE(subj.playerMarker, subj.pieceMarker, origin.cell, objet.onTop ? destination.cell : shifted);
                case false:
                    return new Hive.PLACE(subj.playerMarker, subj.pieceMarker, shifted);
            }
        }
        throw new Exception($"your move was bad: {move}");
    }
    public static string moveListToNotation(List<Move> moves)
    {
        Dictionary<Hive.Pieces, int> wInventory = new Dictionary<Pieces, int>()
        {
            [Hive.Pieces.ANT] = 3,
            [Hive.Pieces.SPIDER] = 2,
            [Hive.Pieces.BEETLE] = 2,
            [Hive.Pieces.GRASSHOPPER] = 3,
        };
        Dictionary<Hive.Pieces, int> bInventory = new Dictionary<Pieces, int>()
        {
            [Hive.Pieces.ANT] = 3,
            [Hive.Pieces.SPIDER] = 2,
            [Hive.Pieces.BEETLE] = 2,
            [Hive.Pieces.GRASSHOPPER] = 3,
        };

        Dictionary<Hive.Players, Dictionary<Hive.Pieces, int>> inventories = new Dictionary<Players, Dictionary<Pieces, int>>()
        {
            [Players.BLACK] = bInventory,
            [Players.WHITE] = wInventory,
        };
        Dictionary<Cell, List<string>> map = new Dictionary<Cell, List<string>>() { };
        List<string> converted = new List<string>();

        string Sujeto(Pieces piece, Players player) => $"{(player == Players.WHITE ? 'w' : 'b')}{reversedPieceDict[piece]}{(inventories[player].ContainsKey(piece) ? reference[piece] - inventories[player][piece] : "")}";
        string placeSubject(Pieces piece, Players player, Cell destination)
        {
            if (piece != Pieces.BEE && piece != Pieces.MOSQUITO && piece != Pieces.LADYBUG)
            {
                inventories[player][piece]--;
            }
            string subject = Sujeto(piece, player);
            map.Add(destination, new List<string>() { subject });
            return subject;
        }
        string Objeto(Cell destination)
        {
            foreach (FTHexCorner hexCorner in HiveUtils.corners.Keys)
            {
                Cell toCheck = getCellFromDirection(destination, hexCorner);
                if (map.ContainsKey(toCheck))
                {
                    string prelim = map[toCheck][0];
                    switch (hexCorner)
                    {
                        case FTHexCorner.Right:
                            //it's to the left of the other guy
                            return $"-{prelim}";
                        case FTHexCorner.Left:
                            //fixed
                            return $"{prelim}-";
                        case FTHexCorner.UpRight:
                            //fixed
                            return $"/{prelim}";
                        case FTHexCorner.DownRight:
                            //fixed
                            return $"\\{prelim}";
                        case FTHexCorner.DownLeft:
                            //fixed
                            return $"{prelim}/";
                        case FTHexCorner.UpLeft:
                            //fixed
                            return $"{prelim}\\";
                    }
                }
            }
            throw new Exception("Couldn't find appropiate reference for the Objet part.");
        }
        string initialToNotation(INITIAL_PLACE move)
        {
            switch (move.player)
            {
                case Players.WHITE:
                    return $"{placeSubject(move.piece, move.player, move.destination)} .";
                case Players.BLACK:
                    return $"{placeSubject(move.piece, move.player, move.destination)} {Objeto(move.destination)}";
                default:
                    throw new Exception("shut up type guy");
            }

        }
        string placeToNotation(PLACE move) => $"{placeSubject(move.piece, move.player, move.destination)} {Objeto(move.destination)}";

        string moveToNotation(MOVE_PIECE move)
        {
            string subj = map[move.origin][0];
            if (map[move.origin].Count > 1)
            {
                map[move.origin].RemoveAt(0);
            }
            else
            {
                map.Remove(move.origin);
            }
            string prelim = $"{subj} {Objeto(move.destination)}";
            if (move.piece == Pieces.BEETLE && map.ContainsKey(move.destination))
            {
                prelim = $"{subj} {map[move.destination].Last()}";
            }
            if (map.ContainsKey(move.destination))
            {
                map[move.destination].Insert(0, subj);
            }
            else
            {
                map.Add(move.destination, new List<string>() { subj });
            }
            return prelim;
        }
        foreach (Move move in moves)
        {
            switch (move.type)
            {
                case MoveType.INITIAL_PLACE:
                    converted.Add(initialToNotation((INITIAL_PLACE)move));
                    break;
                case MoveType.PLACE:
                    converted.Add(placeToNotation((PLACE)move));
                    break;
                case MoveType.MOVE_PIECE:
                    converted.Add(moveToNotation((MOVE_PIECE)move));
                    break;
                case MoveType.AUTOPASS:
                    converted.Add("pass");
                    break;
                default:
                    break;
            }
        }

        GD.Print(converted.Count + ". " + converted.Last());
        return converted.ToString();
    }
}

