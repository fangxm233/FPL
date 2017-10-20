using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    [Serializable]
    public class FunctionCall : Stmt
    {
        public string name;
        public FunctionCall(int tag) : base(tag)
        {
            name = ((Word)Lexer.Peek).lexeme;
        }

        public override Stmt Build()
        {
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LPARENTHESIS) Error("应输入\"(\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.RPARENTHESIS) Error("应输入\")\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            GetFunction(name);
        }

        public override void Run()
        {
            GetFunction(name).Run();
        }
    }
}
