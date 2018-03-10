using System.Collections.Generic;
using FPL.LexicalAnalysis;

namespace FPL.Parse.Expression
{
    public class Unary : Expr
    {
        public LinkedListNode<Expr> Position;

        public Unary(int tag)
        {
            Tag = tag;
        }

        public void Set_position(LinkedListNode<Expr> pos)
        {
            Position = pos;
        }

        public override void Build()
        {
            Right = Position.Next.Value;
            if (Parser.TypeOfExpr[Right.Tag] != LexicalAnalysis.Tag.FACTOR) Error(this, "表达式错误");
            Position.List.Remove(Position.Next);
        }
    }
}