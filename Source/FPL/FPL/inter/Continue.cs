using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.Encoding;

namespace FPL.inter
{
    [Serializable]
    class Continue : Sentence
    {
        Sentence loop;
        CodingUnit unit;

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
            if (Parser.analyzing_loop == null) Error(this, "没有要中断或继续的循环");
            loop = Parser.analyzing_loop;
        }

        public override void Code()
        {
            unit = Encoder.Write(InstructionsType.jmp);
        }

        public override void CodeSecond()
        {
            switch (loop.tag)
            {
                case Tag.WHILE:
                    unit.parameter = ((While)loop).to_rel.parameter;
                    break;
                case Tag.FOR:
                    unit.parameter = ((For)loop).to_rel.parameter;
                    break;
                case Tag.DO:
                    unit.parameter = ((Do)loop).rel_line;
                    break;
            }
        }
    }
}
