using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Expression;
using FPL.Parse.Structure;
using FPL.symbols;

namespace FPL.Parse.Sentences.ProcessControl
{
    internal class Return : Sentence
    {
        public Class @class;
        public Expr expr;
        private Function function;
        private readonly string function_name;

        public Return(int tag) : base(tag)
        {
            function_name = Parser.AnalyzingFunction.name;
        }

        public Return(int tag, string name) : base(tag)
        {
            function_name = name;
        }

        public override Sentence Build()
        {
            @class = Parser.AnalyzingClass;
            expr = new Expr().BuildStart();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            function = @class.GetFunction(function_name);
            if (expr == null) return;
            expr.Check();
            if (expr.type != @class.GetFunction(function_name).return_type)
                Error(this, "无法将\"" + expr.type.lexeme + "\"类型作为返回值");
        }

        public override void Code()
        {
            if (function_name == "Main")
            {
                Encoder.Write(InstructionType.endP);
                return;
            }

            if (@class.GetFunction(function_name).return_type != Type.Void)
            {
                expr.Code();
                Encoder.Write(InstructionType.popEAX);
            }

            for (int i = 0; i < function.Statements.Count; i++) Encoder.Write(InstructionType.pop);
            Encoder.Write(InstructionType.ret);
        }
    }
}