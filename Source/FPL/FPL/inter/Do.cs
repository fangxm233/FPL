using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    [Serializable]
    public class Do : Stmt
    {
        Rel rel;
        List<Stmt> stmts;

        public Do(int tag) : base(tag)
        {

        }

        public override Stmt Build()
        {
            NewScope();
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACE) Error("应输入\"{\"");
            stmts = base.Builds();
            if (Lexer.Peek.tag != Tag.RBRACE) Error("应输入\"}\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.WHILE) Error("应输入\"while\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LPARENTHESIS) Error("应输入\"(\"");
            rel = new Rel();
            rel = rel.Build();
            if (Lexer.Peek.tag != Tag.RPARENTHESIS) Error("应输入\")\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            DestroyScope();
            return this;
        }

        public override void Check()
        {
            rel.Check();
            foreach (Stmt item in stmts)
            {
                in_loop = true;
                item.Check();
            }
            in_loop = false;
        }

        public override void Run()
        {
            do
            {
                NewScope();
                in_loop = true;
                foreach (Stmt item in stmts)
                {
                    item.Run();
                    if (is_continue) break;
                    if (!in_loop) break;
                }
                if (is_continue)
                {
                    is_continue = false;
                    continue;
                }
                if (!in_loop)
                {
                    in_loop = false;
                    break;
                }
                DestroyScope();
            } while (rel.Run());
        }
    }
}
