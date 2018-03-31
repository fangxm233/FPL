using System.Collections.Generic;
using FPL.Generator;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Expression;

namespace FPL.Parse.Sentences.Loop
{
    public class For : Sentence
    {
        private Sentence Assign;
        public int EndLine;
        private Expr Expr;
        public List<Sentence> Sentences;
        private Statement Statement;
        public CodingUnit ToRel;

        public For(int tag) : base(tag)
        {
        }

        public override Sentence Build()
        {
            NewScope();
            Match("(");
            Lexer.Next();
            Statement = new Statement(VarType.Local, Tag.STATEMENT);
            Statement.Build();
            Expr = new Expr().BuildStart();
            Assign = new Assign(new Expr().BuildStart(), Tag.ASSIGN);
            Assign = Assign.Build();
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
            Statement.Check();
            Expr?.Check();
            Assign?.Check();
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
            Statement.Code();
            ToRel = FILGenerator.Write(InstructionType.jmp);
            foreach (Sentence item in Sentences) item.Code();
            if (Assign != null)
                Assign.Code();
            ToRel.Parameter = FILGenerator.Line + 1;
            if (Expr != null)
                Expr.Code();
            else
                FILGenerator.Write(InstructionType.jmp);
            CodingUnit u = FILGenerator.Code[FILGenerator.Code.Count - 1];
            u.Parameter = ToRel.LineNum + 1;
            EndLine = u.LineNum;
        }

        public override void CodeSecond()
        {
            foreach (Sentence item in Sentences) item.CodeSecond();
        }
    }
}