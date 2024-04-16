using Sylves;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using Godot;
using static NotationReader;
using System.Collections.Immutable;
using Hive;
using System.Security.Cryptography.X509Certificates;
using HexDemo3;
using static System.Net.Mime.MediaTypeNames;

public class NotationReader
{
    public class Subject
    {
        public Hive.Players playerMarker;
        public Hive.Pieces pieceMarker;
        public int numMarker = 0;
        override public string ToString() => $"playerMarker: {playerMarker}\npieceMarker: {pieceMarker}\nnumMarker: {numMarker}";
        public string ToNotation()
        {
            string result = "";
            if (playerMarker == Hive.Players.BLACK) result += "b";
            else result += "w";
            result += pieceDict.Where<KeyValuePair<string, Hive.Pieces>>(kvp => kvp.Value == pieceMarker).First().Key;
            if (numMarker != 0) result += numMarker.ToString();
            return result;
        }
    }

    public class Objet : Subject
    {
        public Sylves.FTHexCorner positionalMarker;
        public bool onTop = false;
        override public string ToString() => $"positionalMarker: {positionalMarker}\nplayerMarker: {playerMarker}\npieceMarker: {pieceMarker}\nnumMarker: {numMarker}";
        public new string ToNotation()
        {
            Subject basse = new Subject()
            {
                playerMarker = this.playerMarker,
                pieceMarker = this.pieceMarker,
                numMarker = this.numMarker
            };
            string basseStr = basse.ToNotation();
            switch (positionalMarker)
            {
                case FTHexCorner.Right:
                    return basseStr + "-";
                case FTHexCorner.UpRight:
                    return basseStr + "/";
                case FTHexCorner.UpLeft:
                    return "/" + basseStr;
                case FTHexCorner.Left:
                    return "-" + basseStr;
                case FTHexCorner.DownLeft:
                    return "\\" + basseStr;
                case FTHexCorner.DownRight:
                    return basseStr + "\\";
            }
            throw new Exception("wtf kind of notation request was that");
        }


    }

    public class NullObjet : Objet
    {
        override public string ToString() => $"null";
        public new string ToNotation() => ".";
    }


    public static Dictionary<string, Hive.Pieces> pieceDict = new Dictionary<string, Hive.Pieces>() {
        ["S"] = Hive.Pieces.SPIDER,
        ["A"] = Hive.Pieces.ANT,
        ["Q"] = Hive.Pieces.BEE,
        ["B"] = Hive.Pieces.BEETLE,
        ["M"] = Hive.Pieces.MOSQUITO,
        ["L"] = Hive.Pieces.LADYBUG,
        ["G"] = Hive.Pieces.GRASSHOPPER,
    };

    #region regex
    public static Regex moveListPattern = new Regex(@"(([\/\\-])?([wb]{1}){1}([SAQBMLGP]{1})([123]{1})?([\/\\-])?|[\.])");
    public static Regex positionalParticle = new Regex(@"([\/\\-])");
    public static Regex playerMark = new Regex(@"([wb]{1}){1}");
    public static Regex pieceParticle = new Regex(@"([SAQBMLGP]{1})");
    public static Regex numberSpecifierParticle = new Regex(@"(?:[SAQBMLGP]{1})([123]{1})");
    #endregion
    public static bool IsValidMoveList(string moveList)
    {
        List<string> lines = moveList.Split("\n").ToList<string>().Where(str => str.Count() > 0).ToList();
        foreach (string line in lines)
        {
            if (!IsValidMoveListToken(line)) return false;
        }
        GD.Print("Move list validated.");
        return true;
    }
    #region validator helpers
    private static bool IsValidMoveListToken(string line)
    {
        var a = line.Split(":").ToList();
        if (a.Count < 1) return false;
        var designator = a[0];
        if (designator == "URL") return true;
        else if (int.TryParse(designator, out int _))
        {
            var moves = a[1].Split(" ").ToList<string>().Where(str => str.Count() > 0).ToList();
            if (moves.Count != 2) return false;
            if (!isValidMove(moves)) return false;
        }
        return true;
    }
    private static bool isValidMove(List<string> moves)
    {
        if (moves.Count != 2) return false; //redundant but you never know
        string subject = moves[0];
        string objet = moves[1];
        if (moveListPattern.Match(subject).Success && (moveListPattern.Match(objet).Success)) return true;
        GD.Print("Invalid move");
        return false;
    }
    #endregion

    #region tokenizer
    public static List<List<string>> Tokenize(string moveList) {
        List<string> lines = moveList.Split("\n").ToList<string>().Where(str => str.Count() > 0).ToList();
        List<List<string>> tokenized = new List<List<string>>();
        foreach (string line in lines) {
            List<string> tokenizedMove = TokenizeMove(line);
            if (tokenizedMove.Count > 0) tokenized.Add(tokenizedMove);
        }
        return tokenized;
    }
    #endregion
    private static List<string> TokenizeMove(string move)
    {
        var basicSplit = move.Split(":").ToList();
        if (basicSplit.Count < 1) throw new Exception("weird move crashed tokenizemove");
        var designator = basicSplit[0];
        if (designator == "URL") return new List<string>();
        else if (int.TryParse(designator, out int id))
        {
            var moves = basicSplit[1].Split(" ").ToList<string>().Where(str => str.Count() > 0).ToList();
            //GD.Print(moves[0], moves[1]);
            return moves;
        }
        return new List<string>();
    }
    #region parsing helpers
    private static FTHexCorner parseDirection(bool isLeft, string line)
    {
        switch (line)
        {
            case "-":
                if (isLeft) return FTHexCorner.Left;
                else return FTHexCorner.Right;
            case "\\":
                if (isLeft) return FTHexCorner.UpLeft;
                else return FTHexCorner.DownRight;
            case "/":
                if (isLeft) return FTHexCorner.DownLeft;
                else return FTHexCorner.UpRight;
            default:
                throw new Exception("POSITION FUCKED UP");
        }
    }
    private static Objet ParseObjet(string obj)
    {
        if (obj == ".") return new NullObjet();
        Subject basse = ParseSubject(obj);
        Objet objet = new Objet() {
            numMarker = basse.numMarker,
            pieceMarker = basse.pieceMarker,
            playerMarker = basse.playerMarker,
        };
        string first = obj.First().ToString();
        string last = obj.Last().ToString();
        FTHexCorner position;
        if (positionalParticle.IsMatch(first)) {
            objet.positionalMarker = parseDirection(true, first);
        } else if (positionalParticle.IsMatch(last))
        {
            objet.positionalMarker = parseDirection(false, last);
        } else
        {
            objet.onTop = true;
        }
        return objet;
    }
    private static Subject ParseSubject(string subj)
    {
        var subject = new Subject() {
            numMarker = 0,
        };
        foreach (var token in subj)
        {
            if (playerMark.IsMatch(token.ToString())) { subject.playerMarker = token == 'w' ? Hive.Players.WHITE : Hive.Players.BLACK; }
            if (pieceParticle.IsMatch(token.ToString())) { subject.pieceMarker = pieceDict[token.ToString()]; }
        }
        if (numberSpecifierParticle.IsMatch(subj))
        {
            subject.numMarker = int.Parse(numberSpecifierParticle.Match(subj).Value[1].ToString());
        };
        //GD.Print("Original: ", subj, "\nParsed: ", subject.ToNotation(), "\n");
        return subject;
    }
    private static (Subject, Objet) ParseMove(List<string> tokenized)
    {
        return (ParseSubject(tokenized[0]), ParseObjet(tokenized[1]));
    }
    #endregion
    public static List<(Subject, Objet)> Parser(List<List<string>> moves) => moves.Select(ParseMove).ToList();

    private static Cell getCellFromDirection(Cell origin, FTHexCorner dir)
    {
        return new Cell(origin.x + HiveUtils.corners[dir].x, origin.y + HiveUtils.corners[dir].y, origin.z + HiveUtils.corners[dir].z);
    }
    public static List<Hive.Move> Translator(List<(Subject, Objet)> moves)
    {

        //HashSet<string> seen = new HashSet<string>();
        //seen.Add(first.subject.ToNotation());
        //seen.Add(second.subject.ToNotation());

        List<Hive.Move> processed = new List<Hive.Move>();
        Cell center = new Sylves.Cell(4, 5, -9);

        Dictionary<string, Cell> pieceTracker = new Dictionary<string, Cell>();
        List<(Subject, Objet)> iterateMoves = moves.Skip(2).ToList();

        //initial moves

        (Subject subject, Objet objet) first = moves[0];
        (Subject subject, Objet objet) second = moves[1];
        Cell secondPieceLoc = getCellFromDirection(center, second.objet.positionalMarker);
        pieceTracker.Add(first.subject.ToNotation(), center);
        pieceTracker.Add(second.subject.ToNotation(), secondPieceLoc);
        processed.Add(new Hive.INITIAL_PLACE(first.subject.playerMarker, first.subject.pieceMarker, center));
        processed.Add(new Hive.INITIAL_PLACE(second.subject.playerMarker, second.subject.pieceMarker, secondPieceLoc));

        //rest of moves
        foreach ((Subject, Objet) move in iterateMoves)
        {
            Subject subj = move.Item1;
            Objet objet = move.Item2;
            bool alreadyExists = pieceTracker.ContainsKey(subj.ToNotation());
            // Gotta use ((Subject)objet).ToNotation() because if we use the Objet version of ToNotation() we also add the positional marker
            // objet.onTop marks that the notation has no positional marker and is therefore on top of the specified piece
            // if it's true, we skip the call to getCellFromDirection
            Cell dest = objet.onTop ? pieceTracker[((Subject)objet).ToNotation()] : getCellFromDirection(pieceTracker[((Subject)objet).ToNotation()], objet.positionalMarker);
            switch (alreadyExists)
            {
                case true:
                    processed.Add(new Hive.MOVE_PIECE(subj.playerMarker, subj.pieceMarker, pieceTracker[subj.ToNotation()], dest));
                    pieceTracker[subj.ToNotation()] = dest;
                    break;
                case false:
                    processed.Add(new Hive.PLACE(subj.playerMarker, subj.pieceMarker, dest));
                    pieceTracker.Add(subj.ToNotation(), dest);
                    break;
            }
            //GD.Print($"Original: {subj.ToNotation()} {objet.ToNotation()}");
            //GD.Print($"Translated: {processed.Last().ToString()}");
        }
        return processed;
    }

    public static List<Hive.Move> formattedListToMoves(string moveList)
    {
        if (IsValidMoveList(moveList)) return Translator(Parser(Tokenize(moveList)));
        throw new Exception("Invalid movelist.");
    }
    public static string moveListToNotation(List<Move> moves)
    {
        Dictionary<Hive.Pieces, string> reversedPieceDict = pieceDict.ToDictionary(x => x.Value, x => x.Key);
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
        Dictionary<Pieces, int> reference = new Dictionary<Pieces, int>()
        {
            [Pieces.BEE] = 1,
            [Pieces.MOSQUITO] = 1,
            [Pieces.LADYBUG] = 1,
            [Pieces.SPIDER] = 2,
            [Pieces.BEETLE] = 2,
            [Pieces.ANT] = 3,
            [Pieces.GRASSHOPPER] = 3

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
        string Objeto(Cell destination) {
            foreach (FTHexCorner hexCorner in HiveUtils.corners.Keys){
                Cell toCheck = getCellFromDirection(destination, hexCorner);
                if (map.ContainsKey(toCheck))
                {
                    string prelim = map[toCheck][0];
                    switch (hexCorner)
                    {
                        case FTHexCorner.Right:
                            //it's to the left of the other guy
                            return $"-{prelim}";
                        case FTHexCorner.UpRight:
                            //fixed
                            return $"/{prelim}";
                        case FTHexCorner.DownRight:
                            //fixed
                            return $"\\{prelim}";
                        case FTHexCorner.Left:
                            //fixed
                            return $"{prelim}-";
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
            if (map.ContainsKey(move.destination))
            {
                map[move.destination].Insert(0, subj); 
            } else {
                map.Add(move.destination, new List<string>() { subj });
            }
            return prelim;
        }
        foreach (Move move in moves) {
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