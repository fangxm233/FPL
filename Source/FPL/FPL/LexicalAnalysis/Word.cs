namespace FPL.LexicalAnalysis
{
    public class Word : Token
    {
        public static readonly Word
            and = new Word("&&", Tag.AND),
            or = new Word("||", Tag.OR),
            eq = new Word("==", Tag.EQ),
            ne = new Word("!=", Tag.NE),
            le = new Word("<=", Tag.LE),
            ge = new Word(">=", Tag.GE),
            more = new Word(">", Tag.MORE),
            less = new Word("<", Tag.LESS),
            increase = new Word("++", Tag.INCREASE),
            decline = new Word("--", Tag.DECLINE),
            modulo = new Word("%", Tag.MODULO),
            plus = new Word("+", Tag.PLUS),
            minus = new Word("-", Tag.MINUS),
            multiply = new Word("*", Tag.MULTIPLY),
            divide = new Word("/", Tag.DIVIDE),
            dot = new Word(".", Tag.DOT),
            assign = new Word("=", Tag.ASSIGN),
            semicolon = new Word(";", Tag.SEMICOLON),
            Lparenthesis = new Word("(", Tag.LBRACKETS),
            Rparenthesis = new Word(")", Tag.RBRACKETS),
            LBrace = new Word("{", Tag.LBRACE),
            RBrace = new Word("}", Tag.RBRACE),
            comma = new Word(",", Tag.COMMA),
            LSquBrackets = new Word("[", Tag.LSQUBRACKETS),
            RSquBrackets = new Word("[", Tag.RSQUBRACKETS),
            True = new Word("true", Tag.TRUE),
            False = new Word("false", Tag.FALSE),
            temp = new Word("temp", Tag.TEMP);

        public string lexeme = "";

        public Word(string s, int i) : base(i)
        {
            lexeme = s;
        }

        public override string ToString()
        {
            return lexeme;
        }
    }
}