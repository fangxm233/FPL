using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    [Serializable]
    public class Quote : Stmt
    {
        public Quote(int tag) : base(tag)
        {

        }

        public override Stmt Build(Lexer lex)
        {
            switch (((Word)Lexer.Peek).lexeme)
            {
                case "cout":
                    {
                        return new Cout(Tag.QUOTE).Build(lex);
                    }
                case "cin":
                    {
                        return new Cin(Tag.QUOTE).Build(lex);
                    }
            }
            return this;
        }
    }

    [Serializable]
    public class Cout : Quote
    {
        Expr expr;
        public Cout(int tag) : base(tag)
        {

        }

        public override Stmt Build(Lexer lex)
        {
            lex.Next();
            if (Lexer.Peek.tag != Tag.LESS) Error("应输入\"<\"");
            lex.Next();
            if (Lexer.Peek.tag != Tag.LESS) Error("应输入\"<\"");
            expr = new Expr().BuildStart(lex);
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            expr.Check();
            if (Expr.turn_to_string)
            {
                expr = expr.ToStringPlus();
                Expr.turn_to_string = false;
            }
        }

        public override void Run()
        {
            Console.WriteLine(expr.Run());
        }
    }
    [Serializable]
    public class Cin : Quote
    {
        Var expr;
        public Cin(int tag) : base(tag)
        {

        }

        public override Stmt Build(Lexer lex)
        {
            lex.Next();
            if (Lexer.Peek.tag != Tag.MORE) Error("应输入\">\"");
            lex.Next();
            if (Lexer.Peek.tag != Tag.MORE) Error("应输入\">\"");
            lex.Next();
            expr = new Var(Lexer.Peek);
            lex.Next();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Run()
        {
            string s = Console.ReadLine();
            try
            {
                switch (expr.type.type)
                {
                    case "int":
                        {
                            for (int i = Praser.symbols_list.Count - 1; i > -1; i--)
                            {
                                if (Praser.symbols_list[i][expr.name] != null)
                                {
                                    Praser.symbols_list[i][expr.name] = int.Parse(s);
                                    return;
                                }
                            }
                            break;
                        }
                    case "float":
                        {
                            for (int i = Praser.symbols_list.Count - 1; i > -1; i--)
                            {
                                if (Praser.symbols_list[i][expr.name] != null)
                                {
                                    Praser.symbols_list[i][expr.name] = float.Parse(s);
                                    return;
                                }
                            }
                            break;
                        }
                    case "string":
                        {
                            for (int i = Praser.symbols_list.Count - 1; i > -1; i--)
                            {
                                if (Praser.symbols_list[i][expr.name] != null)
                                {
                                    Praser.symbols_list[i][expr.name] = s;
                                    return;
                                }
                            }
                            break;
                        }
                    case "bool":
                        {
                            for (int i = Praser.symbols_list.Count - 1; i > -1; i--)
                            {
                                if (Praser.symbols_list[i][expr.name] != null)
                                {
                                    Praser.symbols_list[i][expr.name] = bool.Parse(s);
                                    return;
                                }
                            }
                            break;
                        }
                }
            }
            catch (Exception)
            {
                Praser.Error("无法将输入转换为类型\"" + expr.type.type + "\"");
            }
        }
    }
}
