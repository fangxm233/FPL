using System.Collections.Generic;
using System.Linq;
using FPL.Classification;
using FPL.DataStorager;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Sentences;
using FPL.Parse.Sentences.ProcessControl;
using FPL.Parse.Structure;

namespace FPL.Parse.Expression
{
    public class Expr : Node
    {
        public Class Class;
        public Token Content;
        public Expr Left;
        public string Name;
        public Expr Right;
        public int tag;
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
                    if (dictionary.ContainsKey(node.Value.tag))
                        if (node.Value.tag != Tag.ID && node.Value.tag != Tag.NEW)
                            node.Value.Build();
        }

        private static LinkedList<Expr> MatchAll()
        {
            var expr = new LinkedList<Expr>();
            expr.AddFirst(new Expr());
            var brackets = new List<Factor>();
            while (true)
            {
                Lexer.Next();
                if (Classifier.ClassificateIn(ClassificateMethod.ExprEnd, Lexer.NextToken.tag) == 1) return expr;
                switch (Lexer.NextToken.tag)
                {
                    case Tag.ID:
                        Lexer.Next();
                        if (Lexer.NextToken.tag == Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            expr.AddLast(new FunctionCall_e(Tag.FUNCTIONCALL));
                            expr.Last.Value.Build();
                            continue;
                        }

                        Lexer.Back();
                        break;
                    case Tag.NEW:
                        expr.AddLast(new New_e());
                        expr.Last.Value.Build();
                        continue;
                }

                if (Classifier.ClassificateIn(ClassificateMethod.ExprType, Lexer.NextToken.tag) == -1)
                    ErrorSta(LogContent.SthUnexpect, Lexer.NextToken);
                switch (Classifier.ClassificateIn(ClassificateMethod.ExprType, Lexer.NextToken.tag))
                {
                    case Tag.FACTOR:
                        Factor f = new Factor(Lexer.NextToken.tag, Lexer.NextToken);
                        expr.AddLast(f);
                        f.Set_position(expr.Last);
                        if (Lexer.NextToken.tag == Tag.LBRACKETS) brackets.Add((Factor) expr.Last());
                        if (Lexer.NextToken.tag == Tag.RBRACKETS)
                        {
                            if (brackets.Count == 0)
                            {
                                expr.RemoveLast();
                                return expr;
                            }

                            brackets.Last().EndPosition = expr.Last;
                            brackets.RemoveAt(brackets.Count - 1);
                        }

                        break;
                    case Tag.BINATY:
                        Binary b = new Binary(Lexer.NextToken.tag);
                        expr.AddLast(b);
                        b.Set_position(expr.Last);
                        break;
                    case Tag.UNARY:
                        Unary u = new Unary(Lexer.NextToken.tag);
                        expr.AddLast(u);
                        u.Set_position(expr.Last);
                        break;
                    case Tag.BOOL:
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

        public override int GetTokenLength()
        {
            return Left.GetTokenLength() + Right.GetTokenLength() + 1;
        }
    }
}