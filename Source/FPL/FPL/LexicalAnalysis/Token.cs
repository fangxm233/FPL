namespace FPL.LexicalAnalysis
{
    public class Token
    {
        public int tag;
        public readonly int line;
        public readonly string file;
        public Token(int i)
        {
            tag = i;
            line = Lexer.line;
            file = Lexer.now_file_name;
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