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
        Expr left;
        Expr right;
        public Assign(int tag) : base(tag)
        {

        }

        public override Stmt Build(Lexer lex)
        {
            GetName(((Word)Lexer.Peek).lexeme);
            left = new Var(Lexer.Peek);
            lex.Scan();
            if (Lexer.Peek.tag != Tag.ASSIGN) Error("应输入\"=\"");
            right = new Expr().BuildStart(lex);
            return this;
        }

        public override void Check()
        {
            right.Check();
            if (Expr.turn_to_string)
            {
                right = right.ToStringPlus();
                Expr.turn_to_string = false;
            }
            switch (left.type.type)
            {
                case "int":
                    {
                        if (right.type.type != "int") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"int\"");
                        return;
                    }
                case "float":
                    {
                        if (right.type.type != "float" || right.type.type != "float") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"float\"");
                        return;
                    }
                case "string":
                    {
                        if (right.type.type != "string") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"string\"");
                        return;
                    }
                case "bool":
                    {
                        if (right.type.type != "bool") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"bool\"");
                        return;
                    }
            }
        }
    }
}
