using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.Encoding;

namespace FPL.inter
{
    [Serializable]
    public class While : Sentence
    {
        Rel rel;
        public List<Sentence> sentences;
        public int end_line;
        public CodingUnit to_rel;

        public While(int tag) : base(tag)
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
            if (rel == null)
            {
                Error(this, "条件判断无效");
            }
            rel.Check();
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
            if(Encoder.line == to_rel.lineNum)
            {
                to_rel.Remove();
                return;
            }
            to_rel.parameter = Encoder.line + 1;
            rel.Code();
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
