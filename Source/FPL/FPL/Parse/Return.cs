using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse;
using FPL.Parse.Expression;

namespace FPL.Parse
{
    class Return : Sentence
    {
        public Expr expr;
        string function_name;
        Function function;
        public Class @class;

        public Return(int tag) : base(tag)
        {
            function_name = Parse.Parser.analyzing_function.name;
        }

        public Return(int tag, string name) : base(tag)
        {
            function_name = name;
        }

        public override Sentence Build()
        {
            @class = Parse.Parser.analyzing_class;
            expr = new Expr().BuildStart();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            function = @class.GetFunction(function_name);
            if (expr == null) return;
            expr.Check();
            if (expr.type != @class.GetFunction(function_name).return_type) Error(this, "无法将\"" + expr.type.lexeme + "\"类型作为返回值");
        }

        public override void Code()
        {
            if(function_name == "Main")
            {
                Encoder.Write(InstructionType.endP);
                return;
            }
            if (@class.GetFunction(function_name).return_type != symbols.Type.Void)
            {
                expr.Code();
                Encoder.Write(InstructionType.popEAX);
            }
            for (int i = 0; i < function.Statements.Count; i++)
            {
                Encoder.Write(InstructionType.pop);
            }
            Encoder.Write(InstructionType.ret);
        }
    }
}