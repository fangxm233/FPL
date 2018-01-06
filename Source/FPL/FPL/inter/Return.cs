using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    class Return : Sentence
    {
        public Expr expr;
        string function_name;

        public Return(int tag) : base(tag)
        {
            function_name = building_function;
        }

        public override Sentence Build()
        {
            expr = new Expr().BuildStart();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            if (expr == null) return;
            if (expr.Check()) expr = expr.ToStringPlus();
            if (expr.type != GetFunction(function_name).return_type) Error(this, "无法将\"" + expr.type.lexeme + "\"类型作为返回值");
        }

        public override void Run()
        {
            if(expr!= null) function_return = expr.Run();
        }
    }
}