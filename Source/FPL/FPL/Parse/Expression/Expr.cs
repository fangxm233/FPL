using System.Collections.Generic;
using System.Linq;
using FPL.LexicalAnalysis;
using FPL.Parse.Sentences;
using FPL.Parse.Structure;
using FPL.symbols;

namespace FPL.Parse.Expression
{
    public class Expr : Node
    {
        public Class Class;
        public Token Content;
        public Expr Left;
        public string Name;
        public Expr Right;
        public int Tag;
        public Type Type = Type.Int;

        public virtual void Build()
        {
        }

        public Expr BuildStart()
        {
            var expr = MatchAll();
            if (expr.Count == 1)
                return null;
            BuildTree(expr.First, expr.Last);
            return expr.Last.Value;
        }

        protected void BuildTree(LinkedListNode<Expr> start, LinkedListNode<Expr> end)
        {
            foreach (var dictionary in Parser.SymbolPriority)
                for (var node = start.Next; node != end.Next; node = node.Next)
                    if (dictionary.ContainsKey(node.Value.Tag))
                        node.Value.Build();
        }

        private LinkedList<Expr> MatchAll()
        {
            var expr = new LinkedList<Expr>();
            expr.AddFirst(new Expr());
            var Brackets = new List<Factor>();
            while (true)
            {
                Lexer.Next();
                switch (Lexer.NextToken.tag)
                {
                    case LexicalAnalysis.Tag.SEMICOLON:
                    case LexicalAnalysis.Tag.AND:
                    case LexicalAnalysis.Tag.OR:
                    case LexicalAnalysis.Tag.EQ:
                    case LexicalAnalysis.Tag.NE:
                    case LexicalAnalysis.Tag.LE:
                    case LexicalAnalysis.Tag.GE:
                    case LexicalAnalysis.Tag.MORE:
                    case LexicalAnalysis.Tag.LESS:
                    case LexicalAnalysis.Tag.COMMA:
                    case LexicalAnalysis.Tag.ASSIGN:
                        return expr;
                    case LexicalAnalysis.Tag.ID:
                        Lexer.Next();
                        if (Lexer.NextToken.tag == LexicalAnalysis.Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            expr.AddLast(new FunctionCall_e(LexicalAnalysis.Tag.FUNCTIONCALL));
                            expr.Last.Value.Build();
                            continue;
                        }

                        Lexer.Back();
                        break;
                    case LexicalAnalysis.Tag.NEW:
                        expr.AddLast(new New_e());
                        expr.Last.Value.Build();
                        continue;
                }

                if (!Parser.TypeOfExpr.ContainsKey(Lexer.NextToken.tag))
                    Error("意外的字符" + Lexer.NextToken);
                switch (Parser.TypeOfExpr[Lexer.NextToken.tag])
                {
                    case LexicalAnalysis.Tag.FACTOR:
                        Factor f = new Factor(Lexer.NextToken.tag, Lexer.NextToken);
                        expr.AddLast(f);
                        f.Set_position(expr.Last);
                        if (Lexer.NextToken.tag == LexicalAnalysis.Tag.LBRACKETS) Brackets.Add((Factor) expr.Last());
                        if (Lexer.NextToken.tag == LexicalAnalysis.Tag.RBRACKETS)
                        {
                            if (Brackets.Count == 0)
                            {
                                expr.RemoveLast();
                                return expr;
                            }

                            Brackets.Last().EndPosition = expr.Last;
                            Brackets.RemoveAt(Brackets.Count - 1);
                        }

                        break;
                    case LexicalAnalysis.Tag.BINATY:
                        Binary b = new Binary(Lexer.NextToken.tag);
                        expr.AddLast(b);
                        b.Set_position(expr.Last);
                        break;
                    case LexicalAnalysis.Tag.UNARY:
                        Unary u = new Unary(Lexer.NextToken.tag);
                        expr.AddLast(u);
                        u.Set_position(expr.Last);
                        break;
                    case LexicalAnalysis.Tag.BOOL:
                        Bool bo = new Bool(Lexer.NextToken.tag);
                        expr.AddLast(bo);
                        bo.Set_position(expr.Last);
                        break;
                }
            }
        }

        public virtual void Check()
        {
        }

        public virtual void Code()
        {
        }
    }
}