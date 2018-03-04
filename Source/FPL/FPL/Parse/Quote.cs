using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPL.Parse
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
