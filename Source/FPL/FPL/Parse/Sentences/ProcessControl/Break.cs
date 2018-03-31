using FPL.Generator;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Sentences.Loop;

namespace FPL.Parse.Sentences.ProcessControl
{
    public class Break : Sentence
    {
        private Sentence Loop;
        private CodingUnit Unit;

        public Break(int tag) : base(tag)
        {
        }

        public override Sentence Build()
        {
            Match(";");
            return this;
        }

        public override void Check()
        {
            if (Parser.AnalyzingLoop == null) Error(LogContent.NoLoopToBreakOrContinue);
            Loop = Parser.AnalyzingLoop;
        }

        public override void Code()
        {
            Unit = FILGenerator.Write(InstructionType.jmp);
        }

        public override void CodeSecond()
        {
            switch (Loop.tag)
            {
                case Tag.WHILE:
                    Unit.Parameter = ((While) Loop).EndLine + 1;
                    break;
                case Tag.FOR:
                    Unit.Parameter = ((For) Loop).EndLine + 1;
                    break;
                case Tag.DO:
                    Unit.Parameter = ((Do) Loop).EndLine + 1;
                    break;
            }
        }
    }
}