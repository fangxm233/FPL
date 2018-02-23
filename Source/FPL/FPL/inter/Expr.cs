using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;
using FPL.Encoding;

namespace FPL.inter
{
    public class Expr : Node
    {
        public Expr left;
        public Expr right;
        public Token content;
        public symbols.Type type = symbols.Type.Int;
        public Class @class;
        public string name;

        public virtual Expr Build()
        {
            bool is_first_loop = true;
            Lexer.Next();
            switch (Lexer.Peek.tag) //检测所有可以为值的单元
            {
                case Tag.TRUE:
                    {
                        right = new True_e(Word.True);
                        break;
                    }
                case Tag.FALSE:
                    {
                        right = new False_e(Word.False);
                        break;
                    }
                case Tag.ID:
                    {
                        Lexer.Next();
                        if (Lexer.Peek.tag == Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            right = new FunctionCall_e(Tag.FUNCTIONCALL).Build();
                            if (is_first_loop)
                            {
                                Parser.analyzing_class.FunctionCalls_e.Add((FunctionCall_e)right);
                                is_first_loop = !is_first_loop;
                            }
                            break;
                        }
                        Lexer.Back();
                        right = new Object_e();
                        if (is_first_loop)
                        {
                            //Parser.analyzing_class.Objects_e.Add((Object_e)right);
                            ((Object_e)right).statement = GetStatement(right.name);
                            is_first_loop = !is_first_loop;
                        }
                        break;
                    }
                case Tag.NEW:
                    {
                        right = new New_e().Build();
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
                        right = new Brackets().Build();
                        break;
                    }
                default:
                    {
                        Error(this, "表达式无效");
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
                case Tag.ASSIGN:
                    {
                        break; //到了各个可能为表达式的结束符号的时候就返回
                    }
                case Tag.PLUS:
                    {
                        return new Plus(this).Build(); //把现在的对象传进去当做他的左值,然后把这个对象当作返回值
                    }
                case Tag.MINUS:
                    {
                        return new Minus(this).Build();
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
                case Tag.DOT:
                    {
                        right = new Dot(right).Build();
                        switch (Lexer.Peek.tag)
                        {
                            case Tag.PLUS:
                                {
                                    return new Plus(this).Build(); 
                                }
                            case Tag.MINUS:
                                {
                                    return new Minus(this).Build();
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
                        }
                        break;
                    }
                default:
                    {
                        Error(this, "表达式无效");
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
                        return new True_e(Word.True);
                    }
                case Tag.FALSE:
                    {
                        Lexer.Next();
                        return new False_e(Word.False);
                    }
                case Tag.ID:
                    {
                        Lexer.Next();
                        if (Lexer.Peek.tag == Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            left = new FunctionCall_e(Tag.FUNCTIONCALL).Build();
                            Parser.analyzing_class.FunctionCalls_e.Add((FunctionCall_e)left);
                            break;
                        }
                        Lexer.Back();
                        left = new Object_e();
                        //Parser.analyzing_class.Objects_e.Add((Object_e)left);
                        ((Object_e)left).statement = GetStatement(left.name);
                        break;
                    }
                case Tag.NEW:
                    {
                        left = new New_e().Build();
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
                        left = new Brackets().Build();
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
                        Error(this, "表达式无效");
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
                case Tag.ASSIGN:
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
                case Tag.DOT:
                    {
                        left = new Dot(left).Build();
                        switch (Lexer.Peek.tag)
                        {
                            case Tag.PLUS:
                                {
                                    return new Plus(this).Build();
                                }
                            case Tag.MINUS:
                                {
                                    return new Minus(this).Build();
                                }
                            case Tag.MULTIPLY:
                                {
                                    left = new Multiply(left).Build();
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
                                    left = new Divide(left).Build();
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
                        }
                        break;
                    }
                default:
                    {
                        Error(this, "表达式无效");
                        break;
                    }
            }
            return left;
        }

        //分析是否需要转成string表达式，让functionCall确定返回值类型
        public virtual Expr Check()
        {
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

        public virtual int GetIndex()
        {
            return 1;
        }
    }

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
            if(left.type == symbols.Type.String || right.type == symbols.Type.String)
            {
                return new PlusString(this);
            }
            switch (left.type.type_name)
            {
                case "int":
                case "float":
                case "bool":
                case "string":
                    break;
                default:
                    Error("表达式暂不支持除\"int\"\"float\"\"bool\"\"string\"以外的类型");
                    break;
            }
            if (left.type != right.type) Error(this, "运算符\"+\"不能用于\"" + left.type.type_name + "\"和\"" + right.type.type_name + "\"操作数");
            type = left.type;
            return this;
        }

        public override void Code()
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionType.add);
        }
    }
    public class PlusString : Expr
    {
        public PlusString(Expr e)
        {
            left = e.left;
            right = e.right;
            type = symbols.Type.String;
        }
    }
    public class Minus : Expr
    {
        public Minus(Expr l)
        {
            left = l;
        }

        public override Expr Check()
        {
            left = left.Check();
            right = right.Check();
            if (left.type == symbols.Type.String || right.type == symbols.Type.String)
            {
                Error(this, "运算符\"-\"不能用于\"" + left.type.type_name + "\"和\"" + right.type.type_name + "\"操作数");
            }
            switch (left.type.type_name)
            {
                case "int":
                case "float":
                case "bool":
                case "string":
                    break;
                default:
                    Error("表达式暂不支持除\"int\"\"float\"\"bool\"\"string\"以外的类型");
                    break;
            }
            if (left.type != right.type) Error(this, "运算符\"-\"不能用于\"" + left.type.type_name + "\"和\"" + right.type.type_name + "\"操作数");
            type = left.type;
            return this;
        }

        public override void Code()
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionType.sub);
        }
    }
    public class Multiply : Expr
    {
        public Multiply(Expr l)
        {
            left = l;
        }

        public override Expr Build()
        {
            bool is_first_loop = true;
            Lexer.Next();
            switch (Lexer.Peek.tag)
            {
                case Tag.ID:
                    {
                        Lexer.Next();
                        if (Lexer.Peek.tag == Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            right = new FunctionCall_e(Tag.FUNCTIONCALL).Build();
                            if (is_first_loop)
                            {
                                Parser.analyzing_class.FunctionCalls_e.Add((FunctionCall_e)right);
                                is_first_loop = !is_first_loop;
                            }
                            break;
                        }
                        Lexer.Back();
                        right = new Object_e();
                        if (is_first_loop)
                        {
                            //Parser.analyzing_class.Objects_e.Add((Object_e)right);
                            ((Object_e)right).statement = GetStatement(right.name);
                            is_first_loop = !is_first_loop;
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
                case Tag.LBRACKETS:
                    {
                        right = new Expr();
                        right = right.BuildStart();
                        break;
                    }
                default:
                    {
                        Error(this, "表达式无效");
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
                case Tag.ASSIGN:
                    {
                        break; //到了各个可能为表达式的结束符号的时候就返回
                    }
                case Tag.MULTIPLY: //把所有同级或更高级的符号匹配掉
                    {
                        return new Multiply(this).Build();
                    }
                case Tag.DIVIDE:
                    {
                        return new Divide(this).Build();
                    }
                case Tag.DOT:
                    {
                        right = new Dot(right).Build();
                        switch (Lexer.Peek.tag)
                        {
                            case Tag.MULTIPLY: //把所有同级或更高级的符号匹配掉
                                {
                                    return new Multiply(this).Build();
                                }
                            case Tag.DIVIDE:
                                {
                                    return new Divide(this).Build();
                                }
                        }
                        break;
                    }
            }
            return this;
        }

        public override Expr Check()
        {
            left = left.Check();
            right = right.Check();
            if (left.type == symbols.Type.String || right.type == symbols.Type.String)
            {
                Error(this, "运算符\"-\"不能用于\"" + left.type.type_name + "\"和\"" + right.type.type_name + "\"操作数");
            }
            switch (left.type.type_name)
            {
                case "int":
                case "float":
                case "bool":
                case "string":
                    break;
                default:
                    Error("表达式暂不支持除\"int\"\"float\"\"bool\"\"string\"以外的类型");
                    break;
            }
            if (left.type != right.type) Error(this, "运算符\"*\"不能用于\"" + left.type.type_name + "\"和\"" + right.type.type_name + "\"操作数");
            type = left.type;
            return this;
        }

        public override void Code()
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionType.mul);
        }
    }
    public class Divide : Expr
    {
        public Divide(Expr l)
        {
            left = l;
        }
        public override Expr Build()
        {
            bool is_first_loop = true;
            Lexer.Next();
            switch (Lexer.Peek.tag)
            {
                case Tag.ID:
                    {
                        Lexer.Next();
                        if (Lexer.Peek.tag == Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            right = new FunctionCall_e(Tag.FUNCTIONCALL).Build();
                            if (is_first_loop)
                            {
                                Parser.analyzing_class.FunctionCalls_e.Add((FunctionCall_e)right);
                                is_first_loop = !is_first_loop;
                            }
                            break;
                        }
                        Lexer.Back();
                        right = new Object_e();
                        if (is_first_loop)
                        {
                            //Parser.analyzing_class.Objects_e.Add((Object_e)right);
                            ((Object_e)right).statement = GetStatement(right.name);
                            is_first_loop = !is_first_loop;
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
                case Tag.LBRACKETS:
                    {
                        right = new Expr();
                        right = right.BuildStart();
                        break;
                    }
                default:
                    {
                        Error(this, "表达式无效");
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
                case Tag.ASSIGN:
                    {
                        break; //到了各个可能为表达式的结束符号的时候就返回
                    }
                case Tag.MULTIPLY: //把所有同级或更高级的符号匹配掉
                    {
                        return new Multiply(this).Build();
                    }
                case Tag.DIVIDE:
                    {
                        return new Divide(this).Build();
                    }
                case Tag.DOT:
                    {
                        right = new Dot(right).Build();
                        switch (Lexer.Peek.tag)
                        {
                            case Tag.MULTIPLY: //把所有同级或更高级的符号匹配掉
                                {
                                    return new Multiply(this).Build();
                                }
                            case Tag.DIVIDE:
                                {
                                    return new Divide(this).Build();
                                }
                        }
                        break;
                    }
            }
            return this;
        }

        public override Expr Check()
        {
            left = left.Check();
            right = right.Check();
            if (left.type == symbols.Type.String || right.type == symbols.Type.String)
            {
                Error(this, "运算符\"-\"不能用于\"" + left.type.type_name + "\"和\"" + right.type.type_name + "\"操作数");
            }
            switch (left.type.type_name)
            {
                case "int":
                case "float":
                case "bool":
                case "string":
                    break;
                default:
                    Error("表达式暂不支持除\"int\"\"float\"\"bool\"\"string\"以外的类型");
                    break;
            }
            if (left.type != right.type) Error(this, "运算符\"\\\"不能用于\"" + left.type.type_name + "\"和\"" + right.type.type_name + "\"操作数");
            type = left.type;
            return this;
        }

        public override void Code()
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionType.div);
        }
    }
    public class Dot : Expr
    {
        public Dot(Expr left)
        {
            this.left = left;
        }

        public override Expr Build()
        {
            Lexer.Next();
            switch (Lexer.Peek.tag) //检测所有可以为值的单元
            {
                case Tag.ID:
                    {
                        Lexer.Next();
                        if (Lexer.Peek.tag == Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            right = new FunctionCall_e(Tag.FUNCTIONCALL).Build();
                            break;
                        }
                        Lexer.Back();
                        right = new Object_e();
                        break;
                    }
                default:
                    {
                        Error(this, "表达式无效");
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
                case Tag.ASSIGN:
                    {
                        break; //到了各个可能为表达式的结束符号的时候就返回
                    }
                case Tag.DOT: //把所有同级或更高级的符号匹配掉
                    {
                        return new Dot(this).Build();
                    }
            }
            return this;
        }

        public override Expr Check()//左边的右边
        {
            left = left.Check();
            right.@class = left.@class;
            right = right.Check();
            type = right.type;
            @class = right.@class;
            return this;
        }

        public override void Code()
        {
            left.Code();
            right.Code();
        }

        public override int GetIndex()
        {
            int i = left.GetIndex();
            int a = right.GetIndex();
            return i > a ? i + 1 : a + 1;
        }
    }

    public class Brackets : Expr
    {
        public override Expr Build()
        {
            BuildStart();
            if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
            return left;
        }
    }
    public class New_e : Expr
    {
        string Type_Name;
        public List<Expr> parameters = new List<Expr>();
        Function function;

        public New_e()
        {
            Lexer.Next();
            Type_Name = ((Word)Lexer.Peek).lexeme;
        }

        public override Expr Build()
        {
            @class = GetClass(Type_Name);
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            while (Lexer.Peek.tag != Tag.RBRACKETS)
            {
                parameters.Add(new Expr().BuildStart());
                if (parameters[parameters.Count - 1] == null)
                {
                    if (Lexer.Peek.tag == Tag.COMMA) Error("缺少参数");
                    if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
                    parameters.RemoveAt(parameters.Count - 1);
                    break;
                }
            }
            return this;
        }

        public override Expr Check()
        {
            if (@class.GetFunction(@class.name).par_statements.Count != parameters.Count)
            {
                Error(this, "该类型不存在" + parameters.Count + "个参数的构造函数");
            }
            @class = GetClass(Type_Name);
            function = @class.GetFunction(Type_Name);
            if (function == null) Error(this, "该类型不存在构造函数");
            type = symbols.Type.GetType(@class.name);
            return this;
        }
        
        public override void Code()
        {
            Encoder.Write(InstructionType.newobjc, @class.ID);
            Encoder.Write(InstructionType.call, @class.GetFunction(".init").ID);
            if (function.par_statements.Count != 0)
            {
                for (int i = parameters.Count - 1; i >= 0; i--)
                {
                    parameters[i].Code();
                }
            }
            Encoder.Write(InstructionType.call, @class.GetFunction(@class.name).ID);
        }
    }
    public class Object_e : Expr
    {
        public Statement statement;
        public bool is_head;
        public VarType varType;
        public int ID;

        public Object_e()
        {
            name = ((Word)Lexer.Peek).lexeme;
        }
        
        public override Expr Check()
        {//@class == null意味着是这一串对象中是第一个
            if (@class == null) { @class = statement.@class; is_head = true; }
            else statement = @class.GetStatement(name);
            varType = statement.varType;
            if (Parser.analyzing_function != null)
            {
                type = Parser.analyzing_function.GetTypeByLocalName(name);
                if (type != null)
                {
                    Parser.analyzing_function.objects_e.Add(this);
                    return this;
                }
            }
            type = @class.GetTypeByLocalName(name);
            if (type == null) Error(this, "类型\"" + @class.name + "\"中未包含\"" + name + "\"的定义");
            @class = GetClass(type.type_name);
            @class.Objects_e.Add(this);
            return this;
        }

        public override void Code()
        {
            ID = statement.ID;
            if (is_head)
            {
                Encoder.Write(InstructionType.pusharg);//this
                Encoder.Write(InstructionType.pushfield, ID);
                return;
            }
            switch (varType)
            {
                case VarType.arg:
                    Encoder.Write(InstructionType.pusharg, ID);
                    break;
                case VarType.field:
                    Encoder.Write(InstructionType.pushfield, ID);
                    break;
                case VarType.local:
                    Encoder.Write(InstructionType.pushloc, ID);
                    break;
            }
        }

        public override int GetIndex()
        {
            return is_head ? 2 : 1;
        }
    }
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
        public override void Code()
        {
            Encoder.Write(InstructionType.pushval, (int)content.GetValue());
        }
    }
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
    }
    public class True_e : Expr
    {
        public True_e(Token c)
        {
            content = c;
            type = symbols.Type.Bool;
        }
        public override Expr Check()
        {
            return this;
        }
    }
    public class False_e : Expr
    {
        public False_e(Token c)
        {
            content = c;
            type = symbols.Type.Bool;
        }
        public override Expr Check()
        {
            return this;
        }
    }
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
    }
}