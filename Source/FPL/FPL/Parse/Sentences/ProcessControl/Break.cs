using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Sentences.Loop;

namespace FPL.Parse.Sentences.ProcessControl
{
    public class Break : Sentence
    {
        Sentence loop;
        CodingUnit unit;

        public Break(int tag) : base(tag) { }

        public override Sentence Build()
        {
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            if (Parser.analyzing_loop == null) Error(this, "没有要中断或继续的循环");
            loop = Parser.analyzing_loop;
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
                    unit.parameter = ((While)loop).end_line + 1;
                    break;
                case Tag.FOR:
                    unit.parameter = ((For)loop).end_line + 1;
                    break;
                case Tag.DO:
                    unit.parameter = ((Do)loop).end_line + 1;
                    break;
            }
        }
    }
}
