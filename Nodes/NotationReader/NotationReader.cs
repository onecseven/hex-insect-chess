using Sylves;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using Godot;
using System.Runtime.ExceptionServices;
using Godot.NativeInterop;

public class NotationReader
{
    public class Subject
    {
        public Hive.Players playerMarker;
        public Hive.Pieces pieceMarker;
        public int numMarker = 0;
    }

    public class Objet 
    {
        public Sylves.FTHexCorner positionalMarker;
        public Hive.Players playerMarker;
        public Hive.Pieces pieceMarker;
        public int numMarker = 0;
    }

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
            return moves;
        }
        return new List<string>();
    }

    //public static Objet ParseObjet(string obj)
    //{
    //    foreach (var token in obj)
    //    {

    //    }
    //}
    public static Subject ParseSubject(string subj)
    {
        var subject = new Subject();
        foreach (var token in subj)
        {
            switch (token)
            {
                case (NotationReader.playerMark.Match(token).Success):
                    {

                    }
            }
        }
    }
    public static List<(Objet, Subject)> ParseMove(string move)
    {

    }

    
}