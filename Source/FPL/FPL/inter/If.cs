using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;

namespace FPL.inter
{
    [Serializable]
    public class If : Sentence
    {
        Rel rel;
        List<Sentence> stmts;

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
            stmts = Builds();
            if (Lexer.Peek.tag != Tag.RBRACE) Error("应输入\"}\"");
            DestroyScope();
            return this;
        }

        public override void Check()
        {
            rel.Check();
            foreach (Sentence item in stmts)
            {
                item.Check();
            }
        }

        public override void Run()
        {
            if (rel.Run())
            {
                foreach (Sentence item in stmts)
                {
                    item.Run();
                }
            }
        }
    }
}
