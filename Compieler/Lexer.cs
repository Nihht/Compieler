using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace Compieler
{
    public class Lexer
    {
        public enum Keywords
        {
            NUM, ID, IF, ELSE, WHILE, DO, LBRA, RBRA, LPAR, RPAR, PLUS, MINUS, LESS, EQUAL, SEMICOLON, EOF, NONE
        }

        public Dictionary<char, Keywords> Symbols = new Dictionary<char, Keywords>
        {
            {'{', Keywords.LBRA },
            { '}', Keywords.RBRA },
            { '=', Keywords.EQUAL },
            { ';', Keywords.SEMICOLON },
            { '(', Keywords.LPAR },
            { ')', Keywords.RPAR },
            { '+', Keywords.PLUS },
            { '-', Keywords.MINUS },
            { '<', Keywords.LESS }
        };

        public Dictionary<string, Keywords> Words = new Dictionary<string, Keywords>
        {
            { "if", Keywords.IF },
            { "else", Keywords.ELSE },
            { "while", Keywords.WHILE },
            { "do", Keywords.DO }
        };

        public int Ch = ' ';
        public Keywords Sym;
        public string Value;

        public void Error(string msg)
        {
            throw new Exception($"Lexer error: {msg}");
        }

        private readonly TextReader _reader;

        public void GetChar()
        {
            Ch = _reader.Read();
        }

        public void NextToken()
        {
            Value = null;
            Sym = Keywords.NONE;
            while (Sym == Keywords.NONE)
            {
                if (Ch == -1)
                    Sym = Keywords.EOF;
                else if (Ch == ' ')
                    GetChar();
                else if (Symbols.Keys.Contains((char)Ch))
                {
                    Sym = Symbols[(char) Ch];
                    GetChar();
                }
                else if (char.IsDigit((char) Ch))
                {
                    var intval = 0;
                    while (char.IsDigit((char) Ch))
                    {
                        intval = intval * 10 + (int) Ch;
                        GetChar();
                    }
                    Value = intval.ToString();
                    Sym = Keywords.NUM;
                }
                else if (char.IsLetter((char) Ch))
                {
                    var ident = "";
                    while (char.IsLetter((char) Ch))
                    {
                        ident = ident + char.ToLower((char) Ch);
                        GetChar();
                    }
                    if (Words.Keys.Contains(ident))
                    {
                        Sym = Words[ident];
                    }
                    else if (ident.Length == 1)
                    {
                        Sym = Keywords.ID;
                        Value = ((int) ident[0] - (int) 'a').ToString();
                    }
                    else
                    {
                        Error($"Unknown identifier: {ident}");
                    }
                }
                else
                {
                    Error($"Unexpected symbol: {Ch}");
                }
            }
        }

        public Lexer(TextReader reader)
        {
            _reader = reader;
        }
    }
}
