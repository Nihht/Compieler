using System;
using System.Linq.Expressions;

namespace Compieler
{
    public class Node
    {
        public Node(Parser.Kind kind, string value, Node op1, Node op2, Node op3)
        {
            Kind = kind;
            Value = value;
            Op1 = op1;
            Op2 = op2;
            Op3 = op3;
        }

        public Parser.Kind Kind;
        public string Value;
        public Node Op1;
        public Node Op2;
        public Node Op3;
    }

    public class Parser
    {
        public enum Kind
        {
            Var,
            Const,
            Add,
            Sub,
            Lt,
            Set,
            If1,
            If2,
            While,
            Do,
            Empty,
            Seq,
            Expr,
            Prog
        }

        public Lexer Lex;

        public Parser(Lexer lexer)
        {
            Lex = lexer;
        }

        public void Error(string msg)
        {
            throw new Exception($"Lexer error: {msg}");
        }

        public Node Term()
        {
            if (Lex.Sym == Lexer.Keywords.ID)
            {
                var n = new Node(Kind.Var, Lex.Value, null, null, null);
                Lex.NextToken();
                return n;
            }
            else if (Lex.Sym == Lexer.Keywords.NUM)
            {
                var n = new Node(Kind.Const, Lex.Value, null, null, null);
                Lex.NextToken();
                return n;
            }
            else
            {
                return ParenExpr();
            }
        }

        public Node Summa()
        {
            var n = Term();
            while (Lex.Sym == Lexer.Keywords.PLUS || Lex.Sym == Lexer.Keywords.MINUS)
            {
                var kind = Lex.Sym == Lexer.Keywords.PLUS ? Kind.Add : Kind.Sub;
                Lex.NextToken();
                n = new Node(kind, "", n, Term(), null);
            }

            return n;
        }

        public Node Test()
        {
            var n = Summa();
            if (Lex.Sym == Lexer.Keywords.LESS)
            {
                Lex.NextToken();
                n = new Node(Kind.Lt, "", n, Summa(), null);
            }

            return n;
        }

        public Node Expr()
        {
            if (Lex.Sym != Lexer.Keywords.ID)
            {
                return Test();
            }

            var n = Test();
            if (n.Kind == Kind.Var && Lex.Sym == Lexer.Keywords.EQUAL)
            {
                Lex.NextToken();
                n = new Node(Kind.Set, "", n, Expr(), null);
            }

            return n;
        }

        public Node ParenExpr()
        {
            if (Lex.Sym != Lexer.Keywords.LPAR)
            {
                Error("( expected");
            }
            Lex.NextToken();
            var n = Expr();
            if (Lex.Sym != Lexer.Keywords.RPAR)
            {
                Error(") expected");
            }
            Lex.NextToken();
            return n;
        }

        public Node Statement()
        {
            Node n;
            if (Lex.Sym == Lexer.Keywords.IF)
            {
                n = new Node(Kind.If1, "", null, null, null);
                Lex.NextToken();
                n.Op1 = ParenExpr();
                n.Op2 = Statement();
                if (Lex.Sym == Lexer.Keywords.ELSE)
                {
                    n.Kind = Kind.If2;
                    Lex.NextToken();
                    n.Op3 = Statement();
                }
            }
            else if (Lex.Sym == Lexer.Keywords.WHILE)
            {
                n = new Node(Kind.While, "", null, null, null);
                Lex.NextToken();
                n.Op1 = ParenExpr();
                n.Op2 = Statement();
            }
            else if (Lex.Sym == Lexer.Keywords.DO)
            {
                n = new Node(Kind.Do, "", null, null, null);
                Lex.NextToken();
                n.Op1 = Statement();
                if (Lex.Sym != Lexer.Keywords.WHILE)
                {
                    Error("while expected");
                }
                Lex.NextToken();
                n.Op2 = ParenExpr();
                if (Lex.Sym != Lexer.Keywords.SEMICOLON)
                    Error("; expected");
            }
            else if (Lex.Sym == Lexer.Keywords.SEMICOLON)
            {
                n = new Node(Kind.Empty, "", null, null, null);
                Lex.NextToken();
            }
            else if (Lex.Sym == Lexer.Keywords.LBRA)
            {
                n = new Node(Kind.Empty, "", null, null, null);
                Lex.NextToken();
                while (Lex.Sym != Lexer.Keywords.RBRA)
                {
                    n = new Node(Kind.Seq, "", n, Statement(), null);
                }
                Lex.NextToken();
            }
            else
            {
                n = new Node(Kind.Expr, "", Expr(), null, null);
                if (Lex.Sym != Lexer.Keywords.SEMICOLON)
                    Error("; expected");
                Lex.NextToken();
            }
            return n;
        }

        public Node Parse()
        {
            Lex.NextToken();
            var node = new Node(Kind.Prog, "", Statement(), null, null);
            if (Lex.Sym != Lexer.Keywords.EOF)
                Error("Invalid statement syntax");
            return node;
        }
    }
}
