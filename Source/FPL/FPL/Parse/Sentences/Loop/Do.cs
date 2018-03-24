using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.OutPut;
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
                Match("}", false);
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
            if (Lexer.NextToken.tag != Tag.WHILE) Error(LogContent.SthExpect, "while");
            Match("(");
            Rel = new Rel().BuildStart();
            Match(")", false);
            Match(";");
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