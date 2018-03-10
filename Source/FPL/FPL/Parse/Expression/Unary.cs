using System.Collections.Generic;
using FPL.LexicalAnalysis;

namespace FPL.Parse.Expression
{
    public class Unary : Expr
    {
        public LinkedListNode<Expr> position;

        public Unary(int tag)
        {
            this.tag = tag;
        }

        public void Set_position(LinkedListNode<Expr> pos)
        {
            position = pos;
        }

        public override void Build()
        {
            right = position.Next.Value;
            if (Parser.TypeOfExpr[right.tag] != Tag.FACTOR) Error(this, "表达式错误");
            position.List.Remove(position.Next);
        }
    }
}