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
    public class If : Stmt
    {
        Rel rel;
        List<Stmt> stmts;

        public If(int tag) : base(tag)
        {
            
        }

        public override Stmt Build()
        {
            NewScope();
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LPARENTHESIS) Error("应输入\"(\"");
            rel = new Rel();
            rel = rel.Build();
            if (Lexer.Peek.tag != Tag.RPARENTHESIS) Error("应输入\")\"");
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
            foreach (Stmt item in stmts)
            {
                item.Check();
            }
        }

        public override void Run()
        {
            if (rel.Run())
            {
                foreach (Stmt item in stmts)
                {
                    item.Run();
                }
            }
        }
    }
}
