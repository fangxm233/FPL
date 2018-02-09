using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    [Serializable]
    public class Quote : Sentence
    {
        public Quote(int tag) : base(tag)
        {

        }

        public override Sentence Build()
        {
            switch (((Word)Lexer.Peek).lexeme)
            {
                case "Println":
                    {
                        return new Println(Tag.QUOTE).Build();
                    }
                case "Readln":
                    {
                        return new Readln(Tag.QUOTE).Build();
                    }
            }
            return this;
        }
    }

    [Serializable]
    public class Println : Quote
    {
        Expr expr;
        public Println(int tag) : base(tag)
        {

        }

        public override Sentence Build()
        {
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            expr = new Expr().BuildStart();
            if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            //Console.WriteLine(expr.Check());
            if (expr.Check()) expr = expr.ToStringPlus();
        }
    }
    [Serializable]
    public class Readln : Quote
    {
        Var expr;
        public Readln(int tag) : base(tag)
        {

        }

        public override Sentence Build()
        {
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            Lexer.Next();
            expr = new Var(Lexer.Peek);
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }
    }
}
