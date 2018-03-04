using FPL.Encoding;
using FPL.LexicalAnalysis;
using System.Collections.Generic;

namespace FPL.Parse.Expression
{
    public class Binary : Expr
    {
        public LinkedListNode<Expr> position;

        public Binary(int tag)
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
            //if ((left.tag != Tag.ID || right.tag != Tag.ID)&&tag == Tag.DOT)
            //    Error(this, "表达式错误");
            position.List.Remove(left);
            position.List.Remove(right);
        }

        public override void Check()
        {
            if(tag == Tag.DOT)
            {
                DotCheck();
                return;
            }

            if (left == null || right == null) Error(this, "表达式错误");
            left.Check();
            right.Check();
            switch (left.type.type_name)
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
            if (left.type != right.type) Error(this, "运算符\"+\"不能用于\"" + left.type.type_name + "\"和\"" + right.type.type_name + "\"操作数");
            type = left.type;
            return;
        }

        public void DotCheck()
        {
            left.Check();
            right.@class = left.@class;
            right.Check();
            type = right.type;
            @class = right.@class;
        }

        public override void Code()
        {
            left.Code();
            right.Code();

            if (tag == Tag.DOT)return;
            if (Parser.ins_table.ContainsKey(tag))
                if (Parser.ins_table[tag].ContainsKey(type.type_name))
                    Encoder.Write(Parser.ins_table[tag][type.type_name]);
                else
                    Error(this, "暂无符号重载");
            else
                Error(this, "表达式错误");
        }
    }
}