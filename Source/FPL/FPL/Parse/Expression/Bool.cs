using System.Collections.Generic;

namespace FPL.Parse.Expression
{
    public class Bool : Expr
    {
        public LinkedListNode<Expr> Position;

        public Bool(int tag)
        {
            Tag = tag;
        }

        public void Set_position(LinkedListNode<Expr> pos)
        {
            Position = pos;
        }

        public override void Build()
        {
            Left = Position.Previous.Value;
            Right = Position.Next.Value;
            Position.List.Remove(Left);
            Position.List.Remove(Right);
        }
    }
}