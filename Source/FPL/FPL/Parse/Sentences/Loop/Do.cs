using System.Collections.Generic;
using FPL.Generator;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Expression;

namespace FPL.Parse.Sentences.Loop
{
    public class Do : Sentence
    {
        public int EndLine;
        private int HeadLine;
        private Expr Expr;
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
            Expr = new Expr().BuildStart();
            if (Expr == null) Error(LogContent.ExprError);
            Match(")", false);
            Match(";");
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
            HeadLine = FILGenerator.Line + 1;
            foreach (Sentence item in Sentences) item.Code();
            if (FILGenerator.Line == HeadLine) return;
            RelLine = FILGenerator.Line + 1;
            Expr.Code();
            CodingUnit u = FILGenerator.Code[FILGenerator.Code.Count - 1];
            u.Parameter = HeadLine;
            EndLine = u.LineNum;
        }
    }
}