using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Expression;

namespace FPL.Parse.Sentences.ProcessControl
{
    public class If : Sentence
    {
        private Expr Rel;
        private List<Sentence> Sentences;
        private List<Sentence> SentencesElse;
        private CodingUnit ToEnd;

        public If(int tag) : base(tag)
        {
        }

        public override Sentence Build()
        {
            NewScope();
            Match("(");
            Rel = new Expr().BuildStart();
            Match(")", false);
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

            DestroyScope();
            Lexer.Next();
            if (Lexer.NextToken.tag == Tag.ELSE)
            {
                NewScope();
                Lexer.Next();
                if (Lexer.NextToken.tag == Tag.LBRACKETS)
                {
                    SentencesElse = BuildMethod();
                    Match("}", false);
                }
                else
                {
                    Lexer.Back();
                    SentencesElse = new List<Sentence>
                    {
                        BuildOne()
                    };
                }

                DestroyScope();
            }
            else
            {
                Lexer.Back();
            }

            return this;
        }

        public override void Check()
        {
            Rel.Check();
            foreach (Sentence item in Sentences) item.Check();
            if (SentencesElse == null) return;
            foreach (Sentence item in SentencesElse) item.Check();
        }

        public override void Code()
        {
            Rel.Code();
            CodingUnit u = Encoder.Code[Encoder.Code.Count - 1];
            u.parameter = Encoder.Line + 2;
            ToEnd = Encoder.Write(InstructionType.jmp);
            foreach (Sentence item in Sentences) item.Code();
            ToEnd.parameter = Encoder.Line + 1;
            if (SentencesElse == null) return;
            foreach (Sentence item in SentencesElse) item.Code();
        }

        public override void CodeSecond()
        {
            foreach (Sentence item in Sentences) item.CodeSecond();
        }
    }
}