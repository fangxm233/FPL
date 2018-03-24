using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Expression;

namespace FPL.Parse.Sentences.Loop
{
    public class While : Sentence
    {
        public int EndLine;
        private Expr Expr;
        public List<Sentence> Sentences;
        public CodingUnit ToRel;

        public While(int tag) : base(tag)
        {
        }

        public override Sentence Build()
        {
            NewScope();
            Match("(");
            Expr = new Expr().BuildStart();
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
            return this;
        }

        public override void Check()
        {
            if (Expr == null) Error(LogContent.ExprError);
            Expr.Check();
            if (Expr.Type.type_name != symbols.Type.Bool.type_name)
                Error(LogContent.UnableToConvertType, Expr.Type.type_name, symbols.Type.Bool.type_name);
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
            Expr.Code();
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