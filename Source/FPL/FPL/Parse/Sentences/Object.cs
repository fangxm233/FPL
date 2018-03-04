using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Structure;

namespace FPL.Parse.Sentences
{
    public class Object_s : Sentence
    {
        public string name;
        Sentence next;
        public symbols.Type type;
        public Class @class;
        public Statement statement;
        public bool is_head = true;
        public VarType varType;
        int ID;

        public Object_s(int tag) : base(tag)
        {
            name = ((Word)Lexer.Peek).lexeme;
        }
        
        public override Sentence Build()
        {
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.DOT)
            {
                //Error("只有赋值，函数调用，new 对象表达式可作为语句");
                Lexer.Back();
                return this;
            }
            next = BuildNext();
            return this;
        }

        public override void Check()
        {
            if (@class == null && statement != null) { @class = statement.@class; is_head = true; }
            else if (statement != null)
            {
                statement = @class.GetStatement(name);
            }
            else
            {
                if (symbols.Type.GetType(name) == null)
                {
                    if (@class == null)
                        @class = Parser.analyzing_class;
                    else if (Parser.analyzing_function.func_type != FuncType.Static)
                    statement = @class.GetStatement(name);
                }
                else
                {
                    @class = GetClass(name);
                    varType = VarType.Class;
                    return;
                }
            }
            varType = statement.varType;
            if (Parser.analyzing_function != null)
            {
                type = Parser.analyzing_function.GetTypeByLocalName(name);
                if (type != null)
                {
                    Parser.analyzing_function.objects_s.Add(this);
                    return;
                }
            }
            type = @class.GetTypeByLocalName(name);
            if (type == null) Error(this, "类型\"" + @class.name + "\"中未包含\"" + name + "\"的定义");
            if (next == null) Error(this, "只有赋值，函数调用和new 对象表达式可用作语句");
            if (next.tag == Tag.FUNCTIONCALL) ((FunctionCall_s)next).@class = GetClass(type.type_name);
            if (next.tag == Tag.OBJECT) ((Object_s)next).@class = GetClass(type.type_name);
            @class = GetClass(type.type_name);
            @class.Objects_s.Add(this);
            next.Check();
        }

        public override void Code()
        {
            ID = statement.ID;
            
            if(varType == VarType.Static)
            {
                Encoder.Write(InstructionType.pushsta, ID);
                return;
            }
            if (is_head && varType == VarType.Field)
            {
                if (Parser.analyzing_function.func_type == FuncType.Static)
                    Error(this, "对象引用对于非静态的字段、方法或属性\"" + name + "\"是必须的");
                Encoder.Write(InstructionType.pusharg);//this
                Encoder.Write(InstructionType.pushfield, ID);
                return;
            }

            switch (varType)
            {
                case VarType.Arg:
                    Encoder.Write(InstructionType.pusharg, ID);
                    break;
                case VarType.Field:
                    Encoder.Write(InstructionType.pushfield, ID);
                    break;
                case VarType.Local:
                    Encoder.Write(InstructionType.pushloc, ID);
                    break;
            }
        }

        public Sentence BuildNext()
        {
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.ID) Error(this, "\"" + Lexer.Peek.ToString() + "\"无效");
            Lexer.Next();
            if (Lexer.Peek.tag == Tag.LBRACKETS)
            {
                Lexer.Back();
                return new FunctionCall_s(Tag.FUNCTIONCALL).Build();
            }
            Lexer.Back();
            return new Object_s(Tag.OBJECT).Build();
        }
    }
}
