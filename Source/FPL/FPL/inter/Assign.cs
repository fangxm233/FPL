using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;

namespace FPL.inter
{
    [Serializable]
    public class Assign : Stmt
    {
        public Var left;
        public Expr right;
        int id;
        public Assign(int tag, int id) : base(tag)
        {
            this.id = id;
        }
        public Assign(int tag) : base(tag)
        {

        }
        public string name;

        public override Stmt Build()
        {
            name = ((Word)Lexer.Peek).lexeme;
            symbols.Type type = (symbols.Type)GetName(name);
            left = new Var(Lexer.Peek);
            Lexer.Next();
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

        public override void Run()
        {
            Parser.var_content[left.id] = right.Run();
        }
    }

    [Serializable]
    public class Int : Assign
    {
        public Int(Assign a) : base(a.tag, Tag.NUM)
        {
            left = a.left;
            right = a.right;
            name = a.name;
        }

        public override void Check()
        {
            right.Check();
            if (Expr.turn_to_string)
            {
                right = right.ToStringPlus();
                Expr.turn_to_string = false;
            }
            if (right.type.type != "int") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"int\"");
            return;
        }

        public override void Run()
        {
            Parser.var_content[left.id] = (int)(float)right.Run();
        }
    }
    [Serializable]
    public class Float : Assign
    {
        public Float(Assign a) : base(a.tag, Tag.NUM)
        {
            left = a.left;
            right = a.right;
            name = a.name;
        }

        public override void Check()
        {
            right.Check();
            if (Expr.turn_to_string)
            {
                right = right.ToStringPlus();
                Expr.turn_to_string = false;
            }
            if (right.type.type != "float" && right.type.type != "int") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"float\"");
            return;
        }
    }
    [Serializable]
    public class String : Assign
    {
        public String(Assign a) : base(a.tag, Tag.NUM)
        {
            left = a.left;
            right = a.right;
            name = a.name;
        }

        public override void Check()
        {
            right.Check();
            if (Expr.turn_to_string)
            {
                right = right.ToStringPlus();
                Expr.turn_to_string = false;
            }
            if (right.type.type != "string") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"string\"");
            return;
        }
    }
    [Serializable]
    public class Bool : Assign
    {
        public Bool(Assign a) : base(a.tag, Tag.NUM)
        {
            left = a.left;
            right = a.right;
            name = a.name;
        }

        public override void Check()
        {
            right.Check();
            if (Expr.turn_to_string)
            {
                right = right.ToStringPlus();
                Expr.turn_to_string = false;
            }
            if (right.type.type != "bool") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"bool\"");
            return;
        }
    }
}
