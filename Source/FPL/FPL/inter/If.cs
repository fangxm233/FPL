using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;

namespace FPL.inter
{
    public class If : Stmt
    {
        Rel rel;
        List<Stmt> stmts;

        public If(int tag) : base(tag)
        {
            
        }

        public override Stmt Build(Lexer lex)
        {
            NewScope();
            lex.Scan();
            if (Lexer.Peek.tag != Tag.LPARENTHESIS) Error("应输入\"(\"");
            rel = new Rel();
            rel = rel.Build(lex);
            if (Lexer.Peek.tag != Tag.RPARENTHESIS) Error("应输入\")\"");
            lex.Scan();
            if (Lexer.Peek.tag != Tag.LBRACE) Error("应输入\"{\"");
            stmts = base.Builds(lex);
            if (Lexer.Peek.tag != Tag.RBRACE) Error("应输入\"}\"");
            DestroyScope();
            return this;
        }
    }
}
