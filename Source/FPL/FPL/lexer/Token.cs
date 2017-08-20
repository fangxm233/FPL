using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FPL.lexer
{
    [Serializable]
    public class Token
    {
        public readonly int tag;
        public readonly int line;
        public Token(int i)
        {
            tag = i;
            line = Lexer.line;
        }
        public override string ToString()
        {
            return "" + (char)tag;
        }
        public virtual object GetValue()
        {
            return null;
        }
    }

    [Serializable]
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
        public override object GetValue()
        {
            return value;
        }
    }

    [Serializable]
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
        public override object GetValue()
        {
            return value;
        }
    }

    [Serializable]
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
        public override object GetValue()
        {
            return value;
        }
    }
}