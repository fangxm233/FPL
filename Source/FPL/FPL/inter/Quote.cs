using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    public class Quote : Sentence
    {
        public Quote(int tag) : base(tag)
        {
        }

        public override Sentence Build()
        {
            return this;
        }
    }
}
