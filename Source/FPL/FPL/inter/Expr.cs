using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;
using FPL.Encoding;

namespace FPL.inter
{
    [Serializable]
    public class Expr : Node
    {
        //public static bool turn_to_string; //标记所有语句检查完了以后是否要转为string表达式
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
                        Lexer.Next();
                        if (Lexer.Peek.tag == Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            right = new FunctionCall_e(Tag.FUNCTIONCALL).Build();
                            break;
                        }
                        else
                        {
                            Lexer.Back();
                            right = new Var(Lexer.Peek);
                        }
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
                case Tag.LBRACKETS: //括号整个可以算个为值的单元
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
                case Tag.RBRACKETS:
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
                        //right = new Plus(this).Build(); 
                        return new Plus(this).Build(); //把现在的对象传进去当做他的左值,然后把这个对象当作返回值
                        //break;
                    }
                case Tag.MINUS:
                    {
                        //right = new Minus(this).Build();
                        return new Minus(this).Build();
                        //break;
                    }
                case Tag.MULTIPLY:
                    {
                        right = new Multiply(right).Build();
                        switch (Lexer.Peek.tag) //把这个/*对象当做下一个次级符号的左值
                        {
                            case Tag.PLUS:
                                {
                                    return new Plus(this).Build(); //把现在的对象传进去当做他的左值,然后把这个对象当作返回值
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
                                    return new Plus(this).Build(); //把现在的对象传进去当做他的左值,然后把这个对象当作返回值
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
                        Lexer.Next();
                        if (Lexer.Peek.tag == Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            left = new FunctionCall_e(Tag.FUNCTIONCALL).Build();
                            break;
                        }
                        else
                        {
                            Lexer.Back();
                            left = new Var(Lexer.Peek);
                        }
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
                case Tag.LBRACKETS:
                    {
                        left = new Expr();
                        left = left.BuildStart();
                        break;
                    }
                case Tag.SEMICOLON:
                case Tag.RBRACKETS:
                case Tag.AND:
                case Tag.OR:
                case Tag.EQ:
                case Tag.NE:
                case Tag.LE:
                case Tag.GE:
                case Tag.MORE:
                case Tag.LESS:
                case Tag.COMMA:
                    {
                        return null;
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
                case Tag.RBRACKETS:
                case Tag.AND:
                case Tag.OR:
                case Tag.EQ:
                case Tag.NE:
                case Tag.LE:
                case Tag.GE:
                case Tag.MORE:
                case Tag.LESS:
                case Tag.COMMA:
                    {
                        return left; //到了各个可能为表达式的结束符号的时候就返回
                    }
                case Tag.PLUS:
                    {
                        return new Plus(left).Build(); //把现在的左值传进去当做他的左值,然后把这个对象当作返回值
                    }
                case Tag.MINUS:
                    {
                        return new Minus(left).Build();
                    }
                case Tag.MULTIPLY:
                    {
                        left = new Multiply(left).Build();
                        switch (Lexer.Peek.tag) //把这个/*对象当做下一个次级符号的左值
                        {
                            case Tag.PLUS:
                                {
                                    return new Plus(left).Build(); //把现在的左值传进去当做他的左值,然后把这个对象当作返回值
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
                        left = new Divide(left).Build();
                        switch (Lexer.Peek.tag)
                        {
                            case Tag.PLUS:
                                {
                                    return new Plus(left).Build(); //把现在的左值传进去当做他的左值,然后把这个对象当作返回值
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

        //分析是否需要转成string表达式，让functionCall确定返回值类型
        public virtual bool Check()
        {
            if (left.Check()) return true;
            if (right.Check()) return true;
            switch (left.type.type)
            {
                case "int":
                    {
                        break;
                    }
                case "string":
                    {
                        type = symbols.Type.String;
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
            return false;
        }

        public virtual Expr ToStringPlus() //目的是把所有的普通+换成string+, 如果有 - * / 的话就报错
        {
            left = left.ToStringPlus();
            right = right.ToStringPlus();
            return this;
        }

        public virtual void Code()
        {
            return;
        }

        public virtual void CodeSecond()
        {
            left.CodeSecond();
            right.CodeSecond();
        }
    }

    [Serializable]
    public class Plus : Expr
    {
        public Plus(Expr l)
        {
            left = l;
        }

        public override bool Check()
        {
            if (left.Check()) return true;
            if (right.Check()) return true;
            switch (left.type.type)
            {
                case "int":
                    {
                        break;
                    }
                case "string":
                    {
                        type = symbols.Type.String;
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
            switch (right.type.type)
            {
                case "int":
                    {
                        break;
                    }
                case "string":
                    {
                        type = symbols.Type.String;
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
            return false;
        }

        public override Expr ToStringPlus()
        {
            PlusString s = new PlusString(this);
            s.ToStringPlus();
            return s;
        }

        public override void Code()
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionsType.add);
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
    }
    [Serializable]
    public class Minus : Expr
    {
        public Minus(Expr l)
        {
            left = l;
        }

        public override Expr ToStringPlus()
        {
            Error(this, "运算符\"-\"无法用于String类型的表达式");
            return base.ToStringPlus();
        }

        public override void Code()
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionsType.sub);
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
                case Tag.LBRACKETS:
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
                case Tag.RBRACKETS:
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

        public override Expr ToStringPlus()
        {
            Error(this, "运算符\"*\"无法用于String类型的表达式");
            return base.ToStringPlus();
        }

        public override void Code()
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionsType.mul);
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
                case Tag.LBRACKETS:
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
                case Tag.RBRACKETS:
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

        public override Expr ToStringPlus()
        {
            Error(this, "运算符\"/\"无法用于String类型的表达式");
            return base.ToStringPlus();
        }

        public override void Code()
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionsType.div);
        }
    }
    [Serializable]
    public class Brackets : Expr
    {
        public override Expr Build()
        {
            left = BuildStart();
            return left;
        }

        public override bool Check()
        {
            if (left.Check())
            {
                left = left.ToStringPlus();
                return true;
            }
            return false;
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
            Parser.analyzing_function.vars.Add(this);
        }
        public override bool Check()
        {
            return false;
        }
        public override Expr ToStringPlus()
        {
            return this;
        }
        public override void Code()
        {
            Encoder.Write(InstructionsType.pushvar, id);
        }
        public override void CodeSecond()
        {
            return;
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
        public override bool Check()
        {
            return false;
        }
        public override Expr ToStringPlus()
        {
            return this;
        }
        public override void Code()
        {
            Encoder.Write(InstructionsType.pushval, (int)content.GetValue());
        }
        public override void CodeSecond()
        {
            return;
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
        public override bool Check()
        {
            return false;
        }
        public override Expr ToStringPlus()
        {
            return this;
        }
        public override void CodeSecond()
        {
            return;
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
        public override bool Check()
        {
            return true;
        }
        public override Expr ToStringPlus()
        {
            return this;
        }
        public override void CodeSecond()
        {
            return;
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
        public override bool Check()
        {
            return true;
        }
        public override Expr ToStringPlus()
        {
            return this;
        }
        public override void CodeSecond()
        {
            return;
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
        public override bool Check()
        {
            return true;
        }
        public override Expr ToStringPlus()
        {
            return this;
        }
        public override void CodeSecond()
        {
            return;
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
