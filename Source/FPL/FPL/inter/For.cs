using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.Encoding;

namespace FPL.inter
{

    public class For : Sentence
    {
        Statement statement;
        Rel rel;
        Sentence assign;
        public List<Sentence> sentences;
        public int end_line;
        public CodingUnit to_rel;

        public For(int tag) : base(tag)
        {

        }

        public override Sentence Build()
        {
            NewScope();
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            Lexer.Next();
            statement = new Statement(VarType.local, Tag.STATEMENT);
            statement.Build();
            rel = new Rel();
            rel = rel.BuildStart();
            assign = new Assign(new Expr().BuildStart(),Tag.ASSIGN);
            assign = assign.Build();
            if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
            Lexer.Next();
            if (Lexer.Peek.tag == Tag.LBRACE)
            {
                sentences = BuildMethod();
                if (Lexer.Peek.tag != Tag.RBRACE) Error("应输入\"}\"");
            }
            else
            {
                Lexer.Back();
                sentences = new List<Sentence>
                {
                    BuildOne()
                };
            }
            DestroyScope();
            return this;
        }

        public override void Check()
        {
            statement.Check();
            if (rel != null)
                rel.Check();
            if (assign != null)
                assign.Check();
            foreach (Sentence item in sentences)
            {
                Parser.analyzing_loop = this;
                item.Check();
            }
            Parser.analyzing_loop = null;
        }

        public override void Code()
        {
            statement.Code();
            to_rel = Encoder.Write(InstructionType.jmp);
            foreach (var item in sentences)
            {
                item.Code();
            }
            if (assign != null)
                assign.Code();
            to_rel.parameter = Encoder.line + 1;
            if (rel != null)
                rel.Code(0);
            else
                Encoder.Write(InstructionType.jmp);
            CodingUnit u = Encoder.code[Encoder.code.Count - 1];
            u.parameter = to_rel.lineNum + 1;
            end_line = u.lineNum;
        }

        public override void CodeSecond()
        {
            foreach (var item in sentences)
            {
                item.CodeSecond();
            }
        }
    }
}
