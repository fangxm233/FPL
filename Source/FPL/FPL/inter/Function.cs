using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    [Serializable]
    public class Function : Stmt
    {
        List<Stmt> stmts = new List<Stmt>();
        public string name;
        public Function(int tag) : base(tag)
        {
            name = ((Word)Lexer.Peek).lexeme;
        }

        public override Stmt Build(Lexer lex)
        {
            AddFunction(name, this);
            NewScope();
            lex.Next();
            if (Lexer.Peek.tag != Tag.LPARENTHESIS) Error("应输入\"(\"");
            lex.Next();
            if (Lexer.Peek.tag != Tag.RPARENTHESIS) Error("应输入\")\"");
            lex.Next();
            if (Lexer.Peek.tag != Tag.LBRACE) Error("应输入\"{\"");
            stmts = Builds(lex);
            if (Lexer.Peek.tag != Tag.RBRACE) Error("应输入\"}\"");
            DestroyScope();
            return this;
        }

        public override void Check()
        {
            foreach (Stmt item in stmts)
            {
                item.Check();
            }
        }

        public override void Run()
        {
            foreach (Stmt item in stmts)
            {
                item.Run();
            }
        }
    }
}
