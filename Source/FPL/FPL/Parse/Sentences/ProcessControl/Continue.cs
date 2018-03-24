using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Sentences.Loop;

namespace FPL.Parse.Sentences.ProcessControl
{
    internal class Continue : Sentence
    {
        private Sentence Loop;
        private CodingUnit Unit;

        public Continue(int tag) : base(tag)
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
            Unit = Encoder.Write(InstructionType.jmp);
        }

        public override void CodeSecond()
        {
            switch (Loop.tag)
            {
                case Tag.WHILE:
                    Unit.parameter = ((While) Loop).ToRel.parameter;
                    break;
                case Tag.FOR:
                    Unit.parameter = ((For) Loop).ToRel.parameter;
                    break;
                case Tag.DO:
                    Unit.parameter = ((Do) Loop).RelLine;
                    break;
            }
        }
    }
}