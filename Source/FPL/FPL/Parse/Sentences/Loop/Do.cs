using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Expression;

namespace FPL.Parse.Sentences.Loop
{
    public class Do : Sentence
    {
        public int EndLine;
        private int HeadLine;
        private Rel Rel;
        public int RelLine;
        public List<Sentence> Sentences;

        public Do(int tag) : base(tag)
        {
        }

        public override Sentence Build()
        {
            NewScope();
            Lexer.Next();
            if (Lexer.NextToken.tag == Tag.LBRACE)
            {
                Sentences = BuildMethod();
                if (Lexer.NextToken.tag != Tag.RBRACE) Error("应输入\"}\"");
            }
            else
            {
                Lexer.Back();
                Sentences = new List<Sentence>
                {
                    BuildOne()
                };
            }

            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.WHILE) Error("应输入\"while\"");
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            Rel = new Rel().BuildStart();
            if (Lexer.NextToken.tag != Tag.RBRACKETS) Error("应输入\")\"");
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.SEMICOLON) Error("应输入\";\"");
            DestroyScope();
            return this;
        }

        public override void Check()
        {
            Rel.Check();
            foreach (Sentence item in Sentences)
            {
                Parser.AnalyzingLoop = this;
                item.Check();
            }

            Parser.AnalyzingLoop = null;
        }

        public override void Code()
        {
            HeadLine = Encoder.Line + 1;
            foreach (Sentence item in Sentences) item.Code();
            if (Encoder.Line == HeadLine) return;
            RelLine = Encoder.Line + 1;
            Rel.Code(0);
            CodingUnit u = Encoder.Code[Encoder.Code.Count - 1];
            u.parameter = HeadLine;
            EndLine = u.line_num;
        }
    }
}