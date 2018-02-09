using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.Encoding;

namespace FPL.inter
{
    [Serializable]
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
            statement = new Statement(Tag.STATEMENT);
            statement.Build();
            rel = new Rel();
            rel = rel.Build();
            Lexer.Next();
            assign = new Assign(Tag.ASSIGN);
            assign = assign.Build();
            if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACE) Error("应输入\"{\"");
            sentences = Builds();
            if (Lexer.Peek.tag != Tag.RBRACE) Error("应输入\"}\"");
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
            to_rel = Encoder.Write(InstructionsType.jmp);
            foreach (var item in sentences)
            {
                item.Code();
            }
            if (assign != null)
                assign.Code();
            to_rel.parameter = Encoder.line + 1;
            if (rel != null)
                rel.Code();
            else
                Encoder.Write(InstructionsType.jmp);
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
