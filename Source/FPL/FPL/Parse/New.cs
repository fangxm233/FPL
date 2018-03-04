using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse;
using FPL.Parse.Expression;

namespace FPL.Parse
{
    public class New_s : Sentence
    {
        string Type_Name;
        Sentence next;
        public List<Expr> parameters = new List<Expr>();
        symbols.Type type;
        Class @class;
        Function function;

        public New_s(int tag) : base(tag)
        {
            Lexer.Next();
            Type_Name = ((Word)Lexer.Peek).lexeme;
        }

        public override Sentence Build()
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
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.DOT)
            {
                if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
                return this;
            }
            next = BuildNext();
            Lexer.Next();
            return this;
        }

        public override void Check()
        {
            if (parameters.Count != 0) Error(this, "暂不支持构造函数重载");
            function = @class.GetFunction(Type_Name);
            if (function == null) Error(this, "该类型不存在构造函数");
            type = symbols.Type.GetType(@class.name);
            if (next != null)
            {
                if (next.tag == Tag.FUNCTIONCALL)
                {
                    ((FunctionCall_s)next).@class = GetClass(type.type_name);
                    ((FunctionCall_s)next).is_head = false;
                }
                if (next.tag == Tag.OBJECT)
                {
                    ((Object_s)next).@class = GetClass(type.type_name);
                    ((FunctionCall_s)next).is_head = false;
                }
                next.Check();
            }
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

    public class New_e : Expr
    {
        string Type_Name;
        public List<Expr> parameters = new List<Expr>();
        Function function;

        public New_e()
        {
            tag = Tag.NEW;
            Lexer.Next();
            Type_Name = ((Word)Lexer.Peek).lexeme;
        }

        public override void Build()
        {
            @class = GetClass(Type_Name);
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            while (Lexer.Peek.tag != Tag.RBRACKETS)
            {
                parameters.Add(new Expr().BuildStart());
                if (parameters[parameters.Count - 1] != null) continue;
                if (Lexer.Peek.tag == Tag.COMMA) Error("缺少参数");
                if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
                parameters.RemoveAt(parameters.Count - 1);
                break;
            }
        }

        public override void Check()
        {
            if (@class.GetFunction(@class.name).par_statements.Count != parameters.Count)
            {
                Error(this, "该类型不存在" + parameters.Count + "个参数的构造函数");
            }
            @class = GetClass(Type_Name);
            function = @class.GetFunction(Type_Name);
            if (function == null) Error(this, "该类型不存在构造函数");
            type = symbols.Type.GetType(@class.name);
            return;
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
}
