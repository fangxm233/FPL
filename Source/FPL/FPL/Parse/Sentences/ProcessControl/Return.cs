using FPL.Encoding;
using FPL.OutPut;
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
        private Type RetrunType;

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
            Match(";", false);
            return this;
        }

        public override void Check()
        {
            Function = Class.GetFunction(FunctionName);
            RetrunType = Function.ReturnType;
            if (Expr == null)
            {
                if (RetrunType != Type.Void) Error(LogContent.ReturnValueMissing, RetrunType);
                return;
            }
            Expr.Check();
            if (Expr.Type != RetrunType)
                Error(LogContent.UnableToConvertType, Expr.Type,
                    RetrunType);
        }

        public override void Code()
        {
            if (FunctionName == "Main")
            {
                Encoder.Write(InstructionType.endP);
                return;
            }

            if (RetrunType != Type.Void)
            {
                Expr.Code();
                Encoder.Write(InstructionType.popEAX);
            }

            for (int i = 0; i < Function.Statements.Count; i++) Encoder.Write(InstructionType.pop);
            Encoder.Write(InstructionType.ret);
        }
    }
}