using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Expression;

namespace FPL.Parse.Sentences.Loop
{
    public class For : Sentence
    {
        private Sentence Assign;
        public int EndLine;
        private Rel Rel;
        public List<Sentence> Sentences;
        private Statement Statement;
        public CodingUnit ToRel;

        public For(int tag) : base(tag)
        {
        }

        public override Sentence Build()
        {
            NewScope();
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            Lexer.Next();
            Statement = new Statement(VarType.Local, Tag.STATEMENT);
            Statement.Build();
            Rel = new Rel();
            Rel = Rel.BuildStart();
            Assign = new Assign(new Expr().BuildStart(), Tag.ASSIGN);
            Assign = Assign.Build();
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
            Statement.Check();
            if (Rel != null)
                Rel.Check();
            if (Assign != null)
                Assign.Check();
            foreach (Sentence item in Sentences)
            {
                Parser.AnalyzingLoop = this;
                item.Check();
            }

            Parser.AnalyzingLoop = null;
        }

        public override void Code()
        {
            Statement.Code();
            ToRel = Encoder.Write(InstructionType.jmp);
            foreach (Sentence item in Sentences) item.Code();
            if (Assign != null)
                Assign.Code();
            ToRel.parameter = Encoder.Line + 1;
            if (Rel != null)
                Rel.Code(0);
            else
                Encoder.Write(InstructionType.jmp);
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