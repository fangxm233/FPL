using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;
using FPL.Encoding;

namespace FPL.inter
{
    [Serializable]
    public class Assign : Sentence
    {
        public Var left;
        public Expr right;
        public int id;
        public Assign(int tag) : base(tag)
        {

        }
        public string name;

        public override Sentence Build()
        {
            name = ((Word)Lexer.Peek).lexeme;
            symbols.Type type = (symbols.Type)GetName(name);
            left = new Var(Lexer.Peek);
            Lexer.Next();
            switch (Lexer.Peek.tag)
            {
                case Tag.SEMICOLON:
                case Tag.RBRACKETS:
                case Tag.COMMA:
                    return this;
            }
            if (Lexer.Peek.tag != Tag.ASSIGN) Error("应输入\"=\"");
            right = new Expr().BuildStart();
            switch (type.type)
            {
                case "int":
                    {
                        return new Int(this);
                    }
                case "float":
                    {
                        return new Float(this);
                    }
                case "string":
                    {
                        return new String(this);
                    }
                case "bool":
                    {
                        return new Bool(this);
                    }
                default:
                    {
                        Error("未知的类型\"" + type.type + "\"");
                        break;
                    }
            }
            return this;
        }

        public override void Check()
        {
            return;
        }

        public override void Code()
        {
            if (right != null)
                right.Code();
            else
                Encoder.Write(InstructionsType.pushval);
            Encoder.Write(InstructionsType.popvar, left.id);
        }
    }

    [Serializable]
    public class Int : Assign
    {
        public Int(Assign a) : base(Tag.NUM)
        {
            left = a.left;
            right = a.right;
            name = a.name;
        }

        public override void Check()
        {
            if (right.Check()) right = right.ToStringPlus();
            if (right.type.type != "int") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"int\"");
            return;
        }
    }
    [Serializable]
    public class Float : Assign
    {
        public Float(Assign a) : base(Tag.NUM)
        {
            left = a.left;
            right = a.right;
            name = a.name;
        }

        public override void Check()
        {
            if (right.Check()) right = right.ToStringPlus();
            if (right.type.type != "float" && right.type.type != "int") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"float\"");
            return;
        }
    }
    [Serializable]
    public class String : Assign
    {
        public String(Assign a) : base(Tag.NUM)
        {
            left = a.left;
            right = a.right;
            name = a.name;
        }

        public override void Check()
        {
            if (right.Check()) right = right.ToStringPlus();
            if (right.type.type != "string") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"string\"");
            return;
        }
    }
    [Serializable]
    public class Bool : Assign
    {
        public Bool(Assign a) : base(Tag.NUM)
        {
            left = a.left;
            right = a.right;
            name = a.name;
        }

        public override void Check()
        {
            if (right.Check()) right = right.ToStringPlus();
            if (right.type.type != "bool") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"bool\"");
            return;
        }
    }
}
