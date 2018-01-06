using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    [Serializable]
    public class For : Sentence
    {
        Sentence stmt;
        Rel rel;
        Sentence assign;
        List<Sentence> stmts;

        public For(int tag) : base(tag)
        {

        }

        public override Sentence Build()
        {
            NewScope();
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            Lexer.Next();
            stmt = new Statement(Tag.STATEMENT);
            stmt.Build();
            rel = new Rel();
            rel = rel.Build();
            Lexer.Next();
            assign = new Assign(Tag.ASSIGN);
            assign = assign.Build();
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
            stmt.Check();
            rel.Check();
            assign.Check();
            foreach (Sentence item in stmts)
            {
                in_loop = true;
                item.Check();
            }
            in_loop = false;
        }

        public override void Run()
        {
            NewScope();
            for (stmt.Run(); rel.Run(); assign.Run())
            {
                NewScope();
                in_loop = true;
                foreach (Sentence item in stmts)
                {
                    item.Run();
                    if (is_continue) break;
                    if (is_break) break;
                }
                if (is_continue)
                {
                    is_continue = false;
                    continue;
                }
                if (is_break)
                {
                    is_break = false;
                    break;
                }
                DestroyScope();
            }
            in_loop = false;
            DestroyScope();
        }
    }
}
