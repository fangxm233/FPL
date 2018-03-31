using System.Collections.Generic;
using System.Linq;
using FPL.DataStorager;
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
            if (Expr.Type.type_name != Type.Bool.type_name)
                Error(LogContent.UnableToConvertType, Expr.Type.type_name, Type.Bool.type_name);
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

            if (Bool.AndString.Count != 0)
                foreach (CodingUnit codingUnit in Bool.AndString)
                    codingUnit.Parameter = FILGenerator.Line + 1;
            else if (Bool.OrString.Count != 0)
                foreach (CodingUnit codingUnit in Bool.OrString)
                    codingUnit.Parameter = HeadLine;
            else
                FILGenerator.Code.Last().Parameter = HeadLine;

            EndLine = FILGenerator.Code.Last().LineNum;
        }
    }
}