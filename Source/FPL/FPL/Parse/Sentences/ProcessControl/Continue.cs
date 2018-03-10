using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Sentences.Loop;

namespace FPL.Parse.Sentences.ProcessControl
{
    internal class Continue : Sentence
    {
        private Sentence loop;
        private CodingUnit unit;

        public Continue(int tag) : base(tag)
        {
        }

        public override Sentence Build()
        {
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            if (Parser.AnalyzingLoop == null) Error(this, "没有要中断或继续的循环");
            loop = Parser.AnalyzingLoop;
        }

        public override void Code()
        {
            unit = Encoder.Write(InstructionType.jmp);
        }

        public override void CodeSecond()
        {
            switch (loop.tag)
            {
                case Tag.WHILE:
                    unit.parameter = ((While) loop).to_rel.parameter;
                    break;
                case Tag.FOR:
                    unit.parameter = ((For) loop).to_rel.parameter;
                    break;
                case Tag.DO:
                    unit.parameter = ((Do) loop).rel_line;
                    break;
            }
        }
    }
}