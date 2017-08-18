using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
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
    }
}
