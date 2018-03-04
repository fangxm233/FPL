using System.Collections.Generic;

namespace FPL.Parse.Expression
{
    public class Bool : Expr
    {
        public LinkedListNode<Expr> position;

        public Bool(int tag)
        {
            this.tag = tag;
        }

        public void Set_position(LinkedListNode<Expr> pos)
        {
            position = pos;
        }

        public override void Build()
        {
            left = position.Previous.Value;
            right = position.Next.Value;
            position.List.Remove(left);
            position.List.Remove(right);
        }
    }
}
