using FPL.LexicalAnalysis;

namespace FPL.DataStorager
{
    public class Word : Token
    {
        public static readonly Word
            And = new Word("&&", Tag.AND),
            Or = new Word("||", Tag.OR),
            Eq = new Word("==", Tag.EQ),
            Ne = new Word("!=", Tag.NE),
            Le = new Word("<=", Tag.LE),
            Ge = new Word(">=", Tag.GE),
            More = new Word(">", Tag.MORE),
            Less = new Word("<", Tag.LESS),

            Increase = new Word("++", Tag.INCREASE),
            Decline = new Word("--", Tag.DECLINE),
            Modulo = new Word("%", Tag.MODULO),

            Plus = new Word("+", Tag.PLUS),
            Minus = new Word("-", Tag.MINUS),
            Multiply = new Word("*", Tag.MULTIPLY),
            Divide = new Word("/", Tag.DIVIDE),
            Dot = new Word(".", Tag.DOT),
            Assign = new Word("=", Tag.ASSIGN),

            Semicolon = new Word(";", Tag.SEMICOLON),
            Comma = new Word(",", Tag.COMMA),

            LBrace = new Word("{", Tag.LBRACE),
            RBrace = new Word("}", Tag.RBRACE),
            Lparenthesis = new Word("(", Tag.LBRACKETS),
            Rparenthesis = new Word(")", Tag.RBRACKETS),
            LSquBrackets = new Word("[", Tag.LSQUBRACKETS),
            RSquBrackets = new Word("]", Tag.RSQUBRACKETS),

            True = new Word("true", Tag.TRUE),
            False = new Word("false", Tag.FALSE),
            Temp = new Word("temp", Tag.TEMP);

        public string Lexeme = "";

        public Word(string s, int i) : base(i)
        {
            Lexeme = s;
        }

        public override string ToString()
        {
            return Lexeme;
        }
    }
}