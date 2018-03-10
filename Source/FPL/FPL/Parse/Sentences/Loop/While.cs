using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Expression;

namespace FPL.Parse.Sentences.Loop
{
    public class While : Sentence
    {
        public int EndLine;
        private Rel Rel;
        public List<Sentence> Sentences;
        public CodingUnit ToRel;

        public While(int tag) : base(tag)
        {
        }

        public override Sentence Build()
        {
            NewScope();
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            Rel = new Rel().BuildStart();
            if (Lexer.NextToken.tag != Tag.RBRACKETS) Error("应输入\")\"");
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

            DestroyScope();
            return this;
        }

        public override void Check()
        {
            if (Rel == null) Error(this, "条件判断无效");
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
            ToRel = Encoder.Write(InstructionType.jmp);
            foreach (Sentence item in Sentences) item.Code();
            if (Encoder.Line == ToRel.line_num)
            {
                ToRel.Remove();
                return;
            }

            ToRel.parameter = Encoder.Line + 1;
            Rel.Code(0);
            CodingUnit u = Encoder.Code[Encoder.Code.Count - 1];
            u.parameter = ToRel.line_num + 1;
            EndLine = u.line_num;
        }

        public override void CodeSecond()
        {
            foreach (Sentence item in Sentences) item.CodeSecond();
        }
    }
}