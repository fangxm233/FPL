using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;

namespace FPL.inter
{
    [Serializable]
    public class Expr : Node
    {
        //Token op;
        public static bool turn_to_string;
        public Expr left;
        public Expr right;
        public Token content;
        public symbols.Type type = symbols.Type.Int;

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
                case Tag.REAL:
                    {
                        right = new Real(Lexer.Peek);
                        break;
                    }
                case Tag.STR:
                    {
                        right = new Str(Lexer.Peek);
                        break;
                    }
                case Tag.LPARENTHESIS:
                    {
                        right = new Expr();
                        right = right.BuildStart(lex);
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
                case Tag.MORE:
                case Tag.LESS:
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

        public Expr BuildStart(Lexer lex)
        {
            lex.Scan();
            switch (Lexer.Peek.tag)
            {
                case Tag.TRUE:
                    {
                        lex.Scan();
                        return new True(Word.True);
                    }
                case Tag.FALSE:
                    {
                        lex.Scan();
                        return new False(Word.False);
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
                case Tag.REAL:
                    {
                        left = new Real(Lexer.Peek);
                        break;
                    }
                case Tag.STR:
                    {
                        left = new Str(Lexer.Peek);
                        break;
                    }
                case Tag.LPARENTHESIS:
                    {
                        left = new Expr();
                        left = left.BuildStart(lex);
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
                case Tag.MORE:
                case Tag.LESS:
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
                            case Tag.MORE:
                            case Tag.LESS:
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
                            case Tag.MORE:
                            case Tag.LESS:
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

        public virtual Expr Check()
        {
            left = left.Check();
            right = right.Check();
            switch (left.type.type)
            {
                case "int":
                    {
                        break;
                    }
                case "string":
                    {
                        type = symbols.Type.String;
                        turn_to_string = true;
                        break;
                    }
                case "float":
                    {
                        if (type == symbols.Type.String) break;
                        type = symbols.Type.Float;
                        break;
                    }
                default:
                    {
                        Error(left,"表达式无效");
                        break;
                    }
            }
            switch (right.type.type)
            {
                case "int":
                    {
                        break;
                    }
                case "string":
                    {
                        type = symbols.Type.String;
                        turn_to_string = true;
                        break;
                    }
                case "float":
                    {
                        if (type == symbols.Type.String) break;
                        type = symbols.Type.Float;
                        break;
                    }
                default:
                    {
                        Error(right, "表达式无效");
                        break;
                    }
            }
            return this;
        }

        public virtual Expr ToStringPlus()
        {
            left = left.ToStringPlus();
            right = right.ToStringPlus();
            return this;
        }

        public virtual object Run()
        {
            return null;
        }
    }

    [Serializable]
    public class Plus : Expr
    {
        public Plus(Expr l)
        {
            left = l;
        }

        public override Expr Check()
        {
            left = left.Check();
            right = right.Check();
            switch (left.type.type)
            {
                case "int":
                    {
                        break;
                    }
                case "string":
                    {
                        type = symbols.Type.String;
                        turn_to_string = true;
                        break;
                    }
                case "float":
                    {
                        if (type == symbols.Type.String) break;
                        type = symbols.Type.Float;
                        break;
                    }
                default:
                    {
                        Error(left, "表达式无效");
                        break;
                    }
            }
            switch (left.type.type)
            {
                case "int":
                    {
                        break;
                    }
                case "string":
                    {
                        type = symbols.Type.String;
                        turn_to_string = true;
                        break;
                    }
                case "float":
                    {
                        if (type == symbols.Type.String) break;
                        type = symbols.Type.Float;
                        break;
                    }
                default:
                    {
                        Error(right, "表达式无效");
                        break;
                    }
            }
            return this;
        }

        public override Expr ToStringPlus()
        {
            PlusString s = new PlusString(this);
            s.ToStringPlus();
            return s;
        }

        public override object Run()
        {
            return (float)left.Run() + (float)right.Run();
        }
    }
    [Serializable]
    public class PlusString : Expr
    {
        public PlusString(Expr e)
        {
            left = e.left;
            right = e.right;
            type = e.type;
        }

        public override object Run()
        {
            return left.Run().ToString() + right.Run().ToString();
        }
    }
    [Serializable]
    public class Minus : Expr
    {
        public Minus(Expr l)
        {
            left = l;
        }

        public override object Run()
        {
            return (float)left.Run() - (float)right.Run();
        }
    }
    [Serializable]
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
                case Tag.REAL:
                    {
                        right = new Real(Lexer.Peek);
                        break;
                    }
                case Tag.LPARENTHESIS:
                    {
                        right = new Expr();
                        right = right.BuildStart(lex);
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
                case Tag.MORE:
                case Tag.LESS:
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

        public override object Run()
        {
            return (float)left.Run() * (float)right.Run();
        }
    }
    [Serializable]
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
                case Tag.REAL:
                    {
                        right = new Real(Lexer.Peek);
                        break;
                    }
                case Tag.LPARENTHESIS:
                    {
                        right = new Expr();
                        right = right.BuildStart(lex);
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
                case Tag.MORE:
                case Tag.LESS:
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

        public override object Run()
        {
            return (float)left.Run() / (float)right.Run();
        }
    }

    [Serializable]
    public class Var : Expr
    {
        public string name;
        public Var(Token c)
        {
            content = c;
            type = (symbols.Type)GetName(((Word)content).lexeme);
            name = ((Word)content).lexeme;
        }
        public override Expr Check()
        {
            return this;
        }
        public override Expr ToStringPlus()
        {
            return this;
        }
        public override object Run()
        {
            if(type.type == "int")
                return (float)(int)GetVar(name);
            return GetVar(name);
        }
    }
    [Serializable]
    public class Num : Expr
    {
        public Num(Token c)
        {
            content = c;
            type = symbols.Type.Int;
        }
        public override Expr Check()
        {
            return this;
        }
        public override Expr ToStringPlus()
        {
            return this;
        }
        public override object Run()
        {
            return (float)(int)content.GetValue();
        }
    }
    [Serializable]
    public class Real : Expr
    {
        public Real(Token c)
        {
            content = c;
            type = symbols.Type.Float;
        }
        public override Expr Check()
        {
            return this;
        }
        public override Expr ToStringPlus()
        {
            return this;
        }
        public override object Run()
        {
            return content.GetValue();
        }
    }
    [Serializable]
    public class True : Expr
    {
        public True(Token c)
        {
            content = c;
            type = symbols.Type.Bool;
        }
        public override Expr Check()
        {
            return this;
        }
        public override Expr ToStringPlus()
        {
            return this;
        }
        public override object Run()
        {
            return true;
        }
    }
    [Serializable]
    public class False : Expr
    {
        public False(Token c)
        {
            content = c;
            type = symbols.Type.Bool;
        }
        public override Expr Check()
        {
            return this;
        }
        public override Expr ToStringPlus()
        {
            return this;
        }
        public override object Run()
        {
            return false;
        }
    }
    [Serializable]
    public class Str : Expr
    {
        public Str(Token c)
        {
            content = c;
            type = symbols.Type.String;
        }
        public override Expr Check()
        {
            return this;
        }
        public override Expr ToStringPlus()
        {
            return this;
        }
        public override object Run()
        {
            return content.GetValue();
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
