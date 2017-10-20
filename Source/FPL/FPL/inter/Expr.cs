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
        public static bool turn_to_string; //标记所有语句检查完了以后是否要转为string表达式
        public Expr left;
        public Expr right;
        public Token content;
        public symbols.Type type = symbols.Type.Int;

        public virtual Expr Build()
        {
            Lexer.Next();
            switch (Lexer.Peek.tag) //检测所有可以为值的单元
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
                case Tag.LPARENTHESIS: //括号整个可以算个为值的单元
                    {
                        right = new Expr();
                        right = right.BuildStart();
                        break;
                    }
                default:
                    {
                        Error("表达式无效");
                        break;
                    }
            }
            Lexer.Next();
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
                        break; //到了各个可能为表达式的结束符号的时候就返回
                    }
                case Tag.PLUS:
                    {
                        right = new Plus(this).Build(); //把现在的右值传进去当做他的左值,然后把这个对象当作右值
                        break;
                    }
                case Tag.MINUS:
                    {
                        right = new Minus(this).Build();
                        break;
                    }
                case Tag.MULTIPLY:
                    {
                        right = new Multiply(right).Build();
                        switch (Lexer.Peek.tag) //把这个/*对象当做下一个次级符号的左值
                        {
                            case Tag.PLUS:
                                {
                                    return new Plus(this).Build(); //把现在的右值传进去当做他的左值,然后把这个对象当作右值
                                }
                            case Tag.MINUS:
                                {
                                    return new Minus(this).Build();
                                }
                        }
                        break;
                    }
                case Tag.DIVIDE:
                    {
                        right = new Divide(right).Build();
                        switch (Lexer.Peek.tag)
                        {
                            case Tag.PLUS:
                                {
                                    return new Plus(this).Build(); //把现在的右值传进去当做他的左值,然后把这个对象当作右值
                                }
                            case Tag.MINUS:
                                {
                                    return new Minus(this).Build();
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

        public Expr BuildStart() //表达式建立的开始调用的，不可重写
        {
            Lexer.Next();
            switch (Lexer.Peek.tag)
            {
                case Tag.TRUE:
                    {
                        Lexer.Next();
                        return new True(Word.True);
                    }
                case Tag.FALSE:
                    {
                        Lexer.Next();
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
                        left = left.BuildStart();
                        break;
                    }
                default:
                    {
                        Error("表达式无效");
                        break;
                    }
            }
            Lexer.Next();
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
                        return left; //到了各个可能为表达式的结束符号的时候就返回
                    }
                case Tag.PLUS:
                    {
                        return new Plus(left).Build(); //把现在的右值传进去当做他的左值,然后把这个对象当作右值
                    }
                case Tag.MINUS:
                    {
                        return new Minus(left).Build();
                    }
                case Tag.MULTIPLY:
                    {
                        left = new Multiply(left);
                        right.Build();
                        switch (Lexer.Peek.tag) //把这个/*对象当做下一个次级符号的左值
                        {
                            case Tag.PLUS:
                                {
                                    return new Plus(left).Build(); //把现在的右值传进去当做他的左值,然后把这个对象当作右值
                                }
                            case Tag.MINUS:
                                {
                                    return new Minus(left).Build();
                                }
                        }
                        break;
                    }
                case Tag.DIVIDE:
                    {
                        left = new Divide(left);
                        right.Build();
                        switch (Lexer.Peek.tag)
                        {
                            case Tag.PLUS:
                                {
                                    return new Plus(left).Build(); //把现在的右值传进去当做他的左值,然后把这个对象当作右值
                                }
                            case Tag.MINUS:
                                {
                                    return new Minus(left).Build();
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

        public virtual Expr ToStringPlus() //目的是把所有的普通+换成string+
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

        public override Expr Build()
        {
            Lexer.Next();
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
                        right = right.BuildStart();
                        break;
                    }
                default:
                    {
                        Error("表达式无效");
                        break;
                    }
            }
            Lexer.Next();
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
                case Tag.MULTIPLY: //把所有同级或更高级的符号匹配掉
                    {
                        return new Multiply(this).Build();
                    }
                case Tag.DIVIDE:
                    {
                        return new Divide(this).Build();
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
        public override Expr Build()
        {
            Lexer.Next();
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
                        right = right.BuildStart();
                        break;
                    }
                default:
                    {
                        Error("表达式无效");
                        break;
                    }
            }
            Lexer.Next();
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
                case Tag.MULTIPLY: //把所有同级或更高级的符号匹配掉
                    {
                        return new Multiply(this).Build();
                    }
                case Tag.DIVIDE:
                    {
                        return new Divide(this).Build();
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
        public int id;
        public Var(Token c)
        {
            content = c;
            name = ((Word)content).lexeme;
            type = (symbols.Type)GetName(name);
            id = GetID(name);
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
                return (float)(int)GetVar(id);
            return GetVar(id);
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
