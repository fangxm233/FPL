using FPL.DataStorager;
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
            switch (((Word) Lexer.NextToken).Lexeme)
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
                    //Error("未知的引用");
                    break;
                }
            }

            Match(";");
            return this;
        }
    }
}