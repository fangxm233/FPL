using System.Collections.Generic;
using System.Linq;
using FPL.DataStorager;
using FPL.Generator;
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
            if (Expr == null) ErrorSta(LogContent.ExprError);
            Expr.Check();
            if (Expr.Type.type_name != Type.Bool.type_name)
                Error(this, LogContent.UnableToConvertType, Expr.Type.type_name, Type.Bool.type_name);
            foreach (Sentence item in Sentences)
            {
                Parser.AnalyzingLoop = this;
                item.Check();
            }

            Parser.AnalyzingLoop = null;
        }

        public override void Code()
        {
            ToRel = FILGenerator.Write(InstructionType.jmp);
            foreach (Sentence item in Sentences) item.Code();
            if (FILGenerator.Line == ToRel.LineNum)
            {
                ToRel.Remove();
                return;
            }

            ToRel.Parameter = FILGenerator.Line + 1;
            Expr.Code();

            if (Bool.AndString.Count != 0)
                foreach (CodingUnit codingUnit in Bool.AndString)
                    codingUnit.Parameter = FILGenerator.Line + 1;
            else if (Bool.OrString.Count != 0)
                foreach (CodingUnit codingUnit in Bool.OrString)
                    codingUnit.Parameter = ToRel.LineNum + 1;
            else
                FILGenerator.Code.Last().Parameter = ToRel.LineNum + 1;

            EndLine = FILGenerator.Code.Last().LineNum;
        }

        public override void CodeSecond()
        {
            foreach (Sentence item in Sentences) item.CodeSecond();
        }
    }
}