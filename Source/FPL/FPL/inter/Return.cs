using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.Encoding;

namespace FPL.inter
{
    class Return : Sentence
    {
        public Expr expr;
        string function_name;
        Function function;

        public Return(int tag) : base(tag)
        {
            function_name = Parser.analyzing_function.name;
        }

        public Return(int tag, string name) : base(tag)
        {
            function_name = name;
        }

        public override Sentence Build()
        {
            expr = new Expr().BuildStart();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            function = GetFunction(function_name);
            if (expr == null) return;
            if (expr.Check()) expr = expr.ToStringPlus();
            if (expr.type != GetFunction(function_name).return_type) Error(this, "无法将\"" + expr.type.lexeme + "\"类型作为返回值");
        }

        public override void Code()
        {
            if(function_name == "Main")
            {
                Encoder.Write(InstructionsType.endP);
                return;
            }
            if (GetFunction(function_name).return_type == symbols.Type.Void)
                Encoder.Write(InstructionsType.pushval);
            else
                expr.Code();
            for (int i = 0; i < function.stmts.Count; i++)
            {
                Encoder.Write(InstructionsType.unloadi);
            }
            Encoder.Write(InstructionsType.ret);
        }
    }
}