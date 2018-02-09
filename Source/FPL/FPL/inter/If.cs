using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;
using FPL.Encoding;

namespace FPL.inter
{
    [Serializable]
    public class If : Sentence
    {
        Rel rel;
        List<Sentence> sentences;
        CodingUnit to_end;

        public If(int tag) : base(tag)
        {
            
        }

        public override Sentence Build()
        {
            NewScope();
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            rel = new Rel();
            rel = rel.Build();
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
            rel.Check();
            foreach (Sentence item in sentences)
            {
                item.Check();
            }
        }

        public override void Code()
        {
            rel.Code();
            CodingUnit u = Encoder.code[Encoder.code.Count - 1];
            u.parameter = Encoder.line + 2;
            to_end = Encoder.Write(InstructionsType.jmp);
            foreach (var item in sentences)
            {
                item.Code();
            }
            to_end.parameter = Encoder.line + 1;
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
