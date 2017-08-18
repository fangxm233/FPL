using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;

namespace FPL.inter
{
    public class Assign : Stmt
    {
        Token id;
        Expr left;
        Expr right;
        public Assign(int tag) : base(tag)
        {

        }

        public override Stmt Build(Lexer lex)
        {
            GetName(((Word)Lexer.Peek).lexeme);
            left = new Var(Lexer.Peek);
            lex.Scan();
            right = new Expr().BuildStart(lex);
            return this;
        }
    }
}
