using FPL.LexicalAnalysis;

namespace FPL.Parse.Sentences
{
    public class Using : Sentence
    {
        public Using(int tag) : base(tag)
        {

        }

        public override Sentence Build()
        {
            Lexer.Next();
            switch (((Word)Lexer.Peek).lexeme)
            {
                case "Console":
                    {
                        Lexer.AddQuote("Println");
                        Lexer.AddQuote("Readln");
                        break;
                    }
                case "IO":
                    {

                        break;
                    }
                default:
                    {
                        Error("未知的引用");
                        break;
                    }
            }
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }
    }
}
