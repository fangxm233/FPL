using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    [Serializable]
    public class Break : Stmt
    {
        public Break(int tag) : base(tag)
        {
            
        }

        public override Stmt Build(Lexer lex)
        {
            lex.Scan();
            return this;
        }

        public override void Check()
        {
            if (!in_loop) Error(this, "没有要中断或继续的循环");
        }

        public override void Run()
        {
            in_loop = false;
        }
    }
}
