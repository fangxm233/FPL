using System.Collections.Generic;

namespace FPL.Parse.Expression
{
    public class Bool : Expr
    {
        public LinkedListNode<Expr> Position;
        private bool isBuilt;

        public Bool(int tag)
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
            Left = Position.Previous.Value;
            Right = Position.Next.Value;
            Position.List.Remove(Left);
            Position.List.Remove(Right);
            isBuilt = true;
        }
    }
}