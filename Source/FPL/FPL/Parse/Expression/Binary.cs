using System.Collections.Generic;
using FPL.Generator;
using FPL.LexicalAnalysis;
using FPL.OutPut;

namespace FPL.Parse.Expression
{
    public class Binary : Expr
    {
        public LinkedListNode<Expr> Position;
        private bool isBuilt;

        public Binary(int tag)
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

        public override void Check()
        {
            if (tag == Tag.DOT)
            {
                DotCheck();
                return;
            }

            if (Left == null || Right == null) Error(LogContent.ExprError);
            Left.Check();
            Right.Check();
            switch (Left.Type.type_name)
            {
                case "int":
                case "float":
                case "bool":
                case "string":
                    break;
                default:
                    Error(LogContent.NoOverload);
                    break;
            }

            if (Left.Type != Right.Type)
                Error(LogContent.OperandNonsupport, "+", Left.Type.type_name, Right.Type.type_name);
            Type = Left.Type;
        }

        public void DotCheck()
        {
            Left.Check();
            Right.Class = Left.Class;
            Right.Check();
            Type = Right.Type;
            Class = Right.Class;
        }

        public override void Code()
        {
            Left.Code();
            Right.Code();

            if (tag == Tag.DOT) return;
            if (Parser.InsTable.ContainsKey(tag))
                if (Parser.InsTable[tag].ContainsKey(Type.type_name))
                    FILGenerator.Write(Parser.InsTable[tag][Type.type_name]);
                else
                    Error(LogContent.NoOverride);
            else
                Error(LogContent.ExprError);
        }
    }
}