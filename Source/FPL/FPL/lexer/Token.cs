using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FPL.lexer
{
    public class Token
    {
        public readonly int tag;
        public Token(int i)
        {
            tag = i;
        }
        public override string ToString()
        {
            return "" + (char)tag;
        }
    }

    public class Num : Token
    {
        public readonly int value;
        public Num(int i) : base(Tag.NUM)
        {
            value = i;
        }
        public override string ToString()
        {
            return "" + value;
        }
    }

    public class Real : Token
    {
        public readonly float value;
        public Real(float f) : base(Tag.REAL)
        {
            value = f;
        }
        public override string ToString()
        {
            return "" + value;
        }
    }

    public class Str : Token
    {
        public readonly string value;
        public Str(string s) : base(Tag.STR)
        {
            value = s;
        }
        public override string ToString()
        {
            return "" + value;
        }
    }
}