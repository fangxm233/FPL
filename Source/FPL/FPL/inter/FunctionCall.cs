using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    [Serializable]
    public class FunctionCall_s : Sentence
    {
        public string name;
        public symbols.Type return_type;
        public FunctionCall_s(int tag) : base(tag)
        {
            name = ((Word)Lexer.Peek).lexeme;
        }

        public override Sentence Build()
        {
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            return_type = GetFunction(name).return_type;
        }

        public override void Run()
        {
            GetFunction(name).Run();
        }
    }

    [Serializable]
    public class FunctionCall_e : Expr
    {
        public string name;
        public symbols.Type return_type;
        public FunctionCall_e(int tag)
        {
            name = ((Word)Lexer.Peek).lexeme;
        }

        public override Expr Build()
        {
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
            return this;
        }

        public override bool Check()
        {
            return_type = GetFunction(name).return_type;
            return return_type == symbols.Type.String ? true : false;
        }

        public override Expr ToStringPlus()
        {
            return this;
        }

        public override object Run()
        {
            GetFunction(name).Run();
            if (Sentence.function_return == null) Error(this, "函数\"" + name + "\"未返回返回值");
            return Sentence.function_return;
        }
    }
}
