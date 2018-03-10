using FPL.Encoding;
using FPL.LexicalAnalysis;
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
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            if (Parser.AnalyzingLoop == null) Error(this, "没有要中断或继续的循环");
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
                    Unit.parameter = ((While) Loop).EndLine + 1;
                    break;
                case Tag.FOR:
                    Unit.parameter = ((For) Loop).EndLine + 1;
                    break;
                case Tag.DO:
                    Unit.parameter = ((Do) Loop).EndLine + 1;
                    break;
            }
        }
    }
}