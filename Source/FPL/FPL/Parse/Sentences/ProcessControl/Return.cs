using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Expression;
using FPL.Parse.Structure;
using FPL.symbols;

namespace FPL.Parse.Sentences.ProcessControl
{
    internal class Return : Sentence
    {
        public Class Class;
        public Expr Expr;
        private Function Function;
        private readonly string FunctionName;

        public Return(int tag) : base(tag)
        {
            FunctionName = Parser.AnalyzingFunction.Name;
        }

        public Return(int tag, string name) : base(tag)
        {
            FunctionName = name;
        }

        public override Sentence Build()
        {
            Class = Parser.AnalyzingClass;
            Expr = new Expr().BuildStart();
            if (Lexer.NextToken.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            Function = Class.GetFunction(FunctionName);
            if (Expr == null) return;
            Expr.Check();
            if (Expr.Type != Class.GetFunction(FunctionName).ReturnType)
                Error(this, "无法将\"" + Expr.Type.Lexeme + "\"类型作为返回值");
        }

        public override void Code()
        {
            if (FunctionName == "Main")
            {
                Encoder.Write(InstructionType.endP);
                return;
            }

            if (Class.GetFunction(FunctionName).ReturnType != Type.Void)
            {
                Expr.Code();
                Encoder.Write(InstructionType.popEAX);
            }

            for (int i = 0; i < Function.Statements.Count; i++) Encoder.Write(InstructionType.pop);
            Encoder.Write(InstructionType.ret);
        }
    }
}