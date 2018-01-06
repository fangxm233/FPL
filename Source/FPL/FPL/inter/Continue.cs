using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    [Serializable]
    class Continue : Sentence
    {
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
            if (!in_loop) Error(this, "没有要中断或继续的循环");
        }

        public override void Run()
        {
            is_continue = true;
        }
    }
}
