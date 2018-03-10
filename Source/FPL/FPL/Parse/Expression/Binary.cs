using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;

namespace FPL.Parse.Expression
{
    public class Binary : Expr
    {
        public LinkedListNode<Expr> Position;

        public Binary(int tag)
        {
            this.Tag = tag;
        }

        public void Set_position(LinkedListNode<Expr> pos)
        {
            Position = pos;
        }

        public override void Build()
        {
            Left = Position.Previous.Value;
            Right = Position.Next.Value;
            //if ((left.tag != Tag.ID || right.tag != Tag.ID)&&tag == Tag.DOT)
            //    Error(this, "表达式错误");
            Position.List.Remove(Left);
            Position.List.Remove(Right);
        }

        public override void Check()
        {
            if (Tag == LexicalAnalysis.Tag.DOT)
            {
                DotCheck();
                return;
            }

            if (Left == null || Right == null) Error(this, "表达式错误");
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
                    Error("表达式暂不支持除\"int\"\"float\"\"bool\"\"string\"以外的类型");
                    break;
            }

            if (Left.Type != Right.Type)
                Error(this, "运算符\"+\"不能用于\"" + Left.Type.type_name + "\"和\"" + Right.Type.type_name + "\"操作数");
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

            if (Tag == LexicalAnalysis.Tag.DOT) return;
            if (Parser.InsTable.ContainsKey(Tag))
                if (Parser.InsTable[Tag].ContainsKey(Type.type_name))
                    Encoder.Write(Parser.InsTable[Tag][Type.type_name]);
                else
                    Error(this, "暂无符号重载");
            else
                Error(this, "表达式错误");
        }
    }
}