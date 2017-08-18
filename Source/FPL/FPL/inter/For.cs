using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    public class For : Stmt
    {
        Stmt stmt;
        Rel rel;
        Stmt assign;
        List<Stmt> stmts;

        public For(int tag) : base(tag)
        {

        }

        public override Stmt Build(Lexer lex)
        {
            NewScope();
            lex.Scan();
            if (Lexer.Peek.tag != Tag.LPARENTHESIS) Error("应输入\"(\"");
            lex.Scan();
            stmt = new Statement(Tag.STATEMENT);
            stmt.Build(lex);
            rel = new Rel();
            rel.Build(lex);
            lex.Scan();
            assign = new Assign(Tag.ASSIGN);
            assign = assign.Build(lex);
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
