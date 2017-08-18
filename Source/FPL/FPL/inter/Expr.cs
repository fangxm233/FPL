using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;

namespace FPL.inter
{
    public class Expr : Node
    {
        //Token op;
        public Expr left;
        public Expr right;
        public Token content;

        public virtual Expr Build(Lexer lex)
        {
            lex.Scan();
            switch (Lexer.Peek.tag)
            {
                case Tag.TRUE:
                    {
                        return new True(Word.True);
                    }
                case Tag.FALSE:
                    {
                        return new False(Word.False);
                    }
                case Tag.ID:
                    {
                        right = new Var(Lexer.Peek);
                        break;
                    }
                case Tag.NUM:
                    {
                        right = new Num(Lexer.Peek);
                        break;
                    }
                case Tag.STR:
                    {
                        right = new Str(Lexer.Peek);
                        break;
                    }
                default:
                    {
                        Error("表达式无效");
                        break;
                    }
            }
            lex.Scan();
            switch (Lexer.Peek.tag)
            {
                case Tag.SEMICOLON:
                case Tag.RPARENTHESIS:
                case Tag.AND:
                case Tag.OR:
                case Tag.EQ:
                case Tag.NE:
                case Tag.LE:
                case Tag.GE:
                    {
                        break;
                    }
                case Tag.PLUS:
                    {
                        right = new Plus(right);
                        right.Build(lex);
                        break;
                    }
                case Tag.MINUS:
                    {
                        right = new Minus(right);
                        right.Build(lex);
                        break;
                    }
                case Tag.MULTIPLY:
                    {
                        right = new Multiply(right);
                        right.Build(lex);
                        switch (Lexer.Peek.tag)
                        {
                            case Tag.PLUS:
                                {
                                    right = new Plus(right);
                                    right.Build(lex);
                                    break;
                                }
                            case Tag.MINUS:
                                {
                                    right = new Minus(right);
                                    right.Build(lex);
                                    break;
                                }
                        }
                        break;
                    }
                case Tag.DIVIDE:
                    {
                        right = new Divide(right);
                        right.Build(lex);
                        switch (Lexer.Peek.tag)
                        {
                            case Tag.PLUS:
                                {
                                    right = new Plus(right);
                                    right.Build(lex);
                                    break;
                                }
                            case Tag.MINUS:
                                {
                                    right = new Minus(right);
                                    right.Build(lex);
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        Error("表达式无效");
                        break;
                    }
            }
            return this;
        }

        public virtual Expr BuildStart(Lexer lex)
        {
            lex.Scan();
            switch (Lexer.Peek.tag)
            {
                case Tag.TRUE:
                    {
                        content = Word.True;
                        lex.Scan();
                        return this;
                    }
                case Tag.FALSE:
                    {
                        content = Word.False;
                        lex.Scan();
                        return this;
                    }
                case Tag.ID:
                    {
                        left = new Var(Lexer.Peek);
                        break;
                    }
                case Tag.NUM:
                    {
                        left = new Num(Lexer.Peek);
                        break;
                    }
                case Tag.STR:
                    {
                        right = new Str(Lexer.Peek);
                        break;
                    }
                default:
                    {
                        Error("表达式无效");
                        break;
                    }
            }
            lex.Scan();
            switch (Lexer.Peek.tag)
            {
                case Tag.SEMICOLON:
                case Tag.RPARENTHESIS:
                case Tag.AND:
                case Tag.OR:
                case Tag.EQ:
                case Tag.NE:
                case Tag.LE:
                case Tag.GE:
                    {
                        return left;
                    }
                case Tag.PLUS:
                    {
                        right = new Plus(left);
                        right.Build(lex);
                        break;
                    }
                case Tag.MINUS:
                    {
                        right = new Minus(left);
                        right.Build(lex);
                        break;
                    }
                case Tag.MULTIPLY:
                    {
                        right = new Multiply(left);
                        right.Build(lex);
                        switch (Lexer.Peek.tag)
                        {
                            case Tag.SEMICOLON:
                            case Tag.RPARENTHESIS:
                            case Tag.AND:
                            case Tag.OR:
                            case Tag.EQ:
                            case Tag.NE:
                            case Tag.LE:
                            case Tag.GE:
                                {
                                    break;
                                }
                            case Tag.PLUS:
                                {
                                    right = new Plus(right);
                                    right.Build(lex);
                                    break;
                                }
                            case Tag.MINUS:
                                {
                                    right = new Minus(right);
                                    right.Build(lex);
                                    break;
                                }
                            default:
                                {
                                    Error("表达式无效");
                                    break;
                                }
                        }
                        break;
                    }
                case Tag.DIVIDE:
                    {
                        right = new Divide(left);
                        right.Build(lex);
                        switch (Lexer.Peek.tag)
                        {
                            case Tag.SEMICOLON:
                            case Tag.RPARENTHESIS:
                            case Tag.AND:
                            case Tag.OR:
                            case Tag.EQ:
                            case Tag.NE:
                            case Tag.LE:
                            case Tag.GE:
                                {
                                    break;
                                }
                            case Tag.PLUS:
                                {
                                    right = new Plus(right);
                                    right.Build(lex);
                                    break;
                                }
                            case Tag.MINUS:
                                {
                                    right = new Minus(right);
                                    right.Build(lex);
                                    break;
                                }
                            default:
                                {
                                    Error("表达式无效");
                                    break;
                                }
                        }
                        break;
                    }
                default:
                    {
                        Error("表达式无效");
                        break;
                    }
            }
            return right;
        }

        public virtual void Check()
        {

        }

        public virtual void Run()
        {

        }
    }
    public class Plus : Expr
    {
        public Plus(Expr l)
        {
            left = l;
        }
    }
    public class PlusString : Expr
    {
        public PlusString()
        {

        }
    }
    public class Minus : Expr
    {
        public Minus(Expr l)
        {
            left = l;
        }
    }
    public class Multiply : Expr
    {
        public Multiply(Expr l)
        {
            left = l;
        }

        public override Expr Build(Lexer lex)
        {
            lex.Scan();
            switch (Lexer.Peek.tag)
            {
                case Tag.ID:
                    {
                        right = new Var(Lexer.Peek);
                        break;
                    }
                case Tag.NUM:
                    {
                        right = new Num(Lexer.Peek);
                        break;
                    }
                default:
                    {
                        Error("表达式无效");
                        break;
                    }
            }
            lex.Scan();
            switch (Lexer.Peek.tag)
            {
                case Tag.SEMICOLON:
                case Tag.RPARENTHESIS:
                case Tag.AND:
                case Tag.OR:
                case Tag.EQ:
                case Tag.NE:
                case Tag.LE:
                case Tag.GE:
                    {
                        break;
                    }
                case Tag.MULTIPLY:
                    {
                        right = new Multiply(right);
                        right.Build(lex);
                        break;
                    }
                case Tag.DIVIDE:
                    {
                        right = new Divide(right);
                        right.Build(lex);
                        break;
                    }
            }
            return this;
        }
    }
    public class Divide : Expr
    {
        public Divide(Expr l)
        {
            left = l;
        }
        public override Expr Build(Lexer lex)
        {
            lex.Scan();
            switch (Lexer.Peek.tag)
            {
                case Tag.ID:
                    {
                        right = new Var(Lexer.Peek);
                        break;
                    }
                case Tag.NUM:
                    {
                        right = new Num(Lexer.Peek);
                        break;
                    }
                default:
                    {
                        Error("表达式无效");
                        break;
                    }
            }
            lex.Scan();
            switch (Lexer.Peek.tag)
            {
                case Tag.SEMICOLON:
                case Tag.RPARENTHESIS:
                case Tag.AND:
                case Tag.OR:
                case Tag.EQ:
                case Tag.NE:
                case Tag.LE:
                case Tag.GE:
                    {
                        break;
                    }
                case Tag.MULTIPLY:
                    {
                        right = new Multiply(right);
                        right.Build(lex);
                        break;
                    }
                case Tag.DIVIDE:
                    {
                        right = new Divide(right);
                        right.Build(lex);
                        break;
                    }
            }
            return this;
        }
    }

    public class Var : Expr
    {
        public Var(Token c)
        {
            content = c;
            GetName(((Word)content).lexeme);
        }
    }
    public class Num : Expr
    {
        public Num(Token c)
        {
            content = c;
        }
    }
    public class Real : Expr
    {
        public Real(Token c)
        {
            content = c;
        }
    }
    public class True : Expr
    {
        public True(Token c)
        {
            content = c;
        }
    }
    public class False : Expr
    {
        public False(Token c)
        {
            content = c;
        }
    }
    public class Str : Expr
    {
        public Str(Token c)
        {
            content = c;
        }
    }
    /*
    public class Basic : Expr
    {
        public Basic(Token c)
        {
            content = c;
        }
    }*/
}
