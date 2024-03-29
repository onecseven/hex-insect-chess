using Sylves;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Runtime.ExceptionServices;
using Godot.NativeInterop;
using System.Diagnostics.SymbolStore;
using System.Reflection.Metadata.Ecma335;

public class NotationReader
{
    public class Subject
    {
        public Hive.Players playerMarker;
        public Hive.Pieces pieceMarker;
        public int numMarker = 0;
        override public string ToString() => $"playerMarker: {playerMarker}\npieceMarker: {pieceMarker}\nnumMarker: {numMarker}";
    }

    public class NullObjet : Objet
    {
        override public string ToString() => $"null";

    }

    public class Objet
    {
        public Sylves.FTHexCorner positionalMarker;
        public Hive.Players playerMarker;
        public Hive.Pieces pieceMarker;
        public int numMarker = 0;
        override public string ToString() => $"positionalMarker: {positionalMarker}\nplayerMarker: {playerMarker}\npieceMarker: {pieceMarker}\nnumMarker: {numMarker}";
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

    public static Regex moveListPattern = new Regex(@"(([\/\\-])?([wb]{1}){1}([SAQBMLGP]{1})([123]{1})?([\/\\-])?|[\.])");
    public static Regex positionalParticle= new Regex(@"([\/\\-])");
    public static Regex playerMark = new Regex(@"([wb]{1}){1}");
    public static Regex pieceParticle = new Regex(@"([SAQBMLGP]{1})");
    public static Regex numberSpecifierParticle = new Regex(@"(?:[SAQBMLGP]{1})([123]{1})");

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
    public static bool IsValidMoveListToken(string line)
    {
        var a = line.Split(":").ToList();
        if (a.Count < 1) return false;
        var designator = a[0];
        if (designator == "URL") return true;
        else if (int.TryParse(designator, out int _))
        {
            var moves = a[1].Split(" ").ToList<string>().Where(str => str.Count() > 0).ToList();
            if (moves.Count !=2) return false;
            if (!isValidMove(moves)) return false;
        }
        return true;
    }
    public static bool isValidMove(List<string> moves)
    {
        if (moves.Count != 2) return false; //redundant but you never know
        string subject = moves[0];
        string objet = moves[1];
        if (moveListPattern.Match(subject).Success && (moveListPattern.Match(objet).Success)) return true;
        GD.Print("Invalid move");
        return false;
    }

    public static List<List<string>> Tokenize(string moveList) {
        List<string> lines = moveList.Split("\n").ToList<string>().Where(str => str.Count() > 0).ToList();
        List<List<string>> tokenized = new List<List<string>>();
        foreach (string line in lines) {
            List<string> tokenizedMove = TokenizeMove(line);
            if (tokenizedMove.Count > 0) tokenized.Add(tokenizedMove);  
        }
        return tokenized;
    }

    public static List<string> TokenizeMove(string move)
    {
        var basicSplit = move.Split(":").ToList();
        if (basicSplit.Count < 1) throw new Exception("weird move crashed tokenizemove");
        var designator = basicSplit[0];
        if (designator == "URL") return new List<string>();
        else if (int.TryParse(designator, out int id))
        {
            var moves = basicSplit[1].Split(" ").ToList<string>().Where(str => str.Count() > 0).ToList();
            GD.Print(moves[0], moves[1]);
            return moves;
        }
        return new List<string>();
    }

    public static FTHexCorner parseDirection(bool isLeft, string line)
    {
        switch (line)
        {
            case "-":
                if (isLeft) return FTHexCorner.Left;
                else return FTHexCorner.Right;
            case "/":
                if (isLeft) return FTHexCorner.UpLeft;
                else return FTHexCorner.UpRight;
            case "\\":
                if (isLeft) return FTHexCorner.DownLeft;
                else return FTHexCorner.DownRight;
            default:
                throw new Exception("POSITION FUCKED UP");
        }
    }

    public static Objet ParseObjet(string obj)
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
        }
        return objet;
    }
    public static Subject ParseSubject(string subj)
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
        return subject;
    }
    public static (Subject, Objet) ParseMove(List<string> tokenized)
    {
        return (ParseSubject(tokenized[0]), ParseObjet(tokenized[1]));
    }

    public static List<(Subject, Objet)> Parser (List<List<string>> moves) => moves.Select(ParseMove).ToList();
}