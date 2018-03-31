using System.Collections.Generic;
using FPL.DataStorager;
using FPL.LexicalAnalysis;
using FPL.OutPut;

namespace FPL.Parse.Expression
{
    public class Unary : Expr
    {
        public LinkedListNode<Expr> Position;
        private bool isBuilt;

        public Unary(int tag)
        {
            this.tag = tag;
        }

        public void Set_position(LinkedListNode<Expr> pos)
        {
            Position = pos;
        }

        public override void Build()
        {
            if (isBuilt) return;
            Right = Position.Next.Value;
            if (Parser.TypeOfExpr[Right.tag] != Tag.FACTOR) Error(LogContent.ExprError);
            Position.List.Remove(Position.Next);
            isBuilt = true;
        }
    }
}