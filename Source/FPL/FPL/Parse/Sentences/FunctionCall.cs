using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Expression;
using FPL.Parse.Structure;

namespace FPL.Parse.Sentences
{
    public class FunctionCall_s : Sentence
    {
        public string name;
        public symbols.Type return_type;
        public List<Expr> parameters = new List<Expr>();
        Sentence next;
        public Class @class;
        public Function function;
        public symbols.Type type;
        public bool is_head = true;

        public FunctionCall_s(int tag) : base(tag)
        {
            name = ((Word)Lexer.Peek).lexeme;
        }

        public override Sentence Build()
        {
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
            return this;
        }

        public Sentence BuildNext()
        {
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.ID) Error("\"" + Lexer.Peek.ToString() + "\"无效");
            Lexer.Next();
            if (Lexer.Peek.tag == Tag.LBRACKETS)
            {
                Lexer.Back();
                return new FunctionCall_s(Tag.FUNCTIONCALL).Build();
            }
            Lexer.Back();
            return new Object_s(Tag.OBJECT).Build();
        }

        public override void Check()
        {
            function = @class.GetFunction(name);
            if (function == null) Error(this, "类型\"" + @class.name + "\"中未包含\"" + name + "\"的定义");
            if (parameters.Count != function.par_statements.Count) Error("\"" + name + "\"方法没有采用" + parameters.Count + "个参数的重载");
            return_type = function.return_type;
            if (next != null)
            {
                if (return_type.tag == Tag.VOID) Error(next, "运算符\".\"无法应用于\"void\"类型的操作数");
                if (next.tag == Tag.FUNCTIONCALL)
                {
                    ((FunctionCall_s)next).@class = GetClass(return_type.type_name);
                    ((FunctionCall_s)next).is_head = false;
                }
                if (next.tag == Tag.OBJECT)
                {
                    ((Object_s)next).@class = GetClass(return_type.type_name);
                    ((Object_s)next).is_head = false;
                }
                next.Check();
            }
            foreach (var item in parameters)
            {
                item.Check();
            }
            if (next == null) return;
        }

        public override void Code()
        {
            if (function.func_type != FuncType.Static)
                Encoder.Write(InstructionType.pusharg);
            if (function.par_statements.Count != 0)
            {
                for (int i = parameters.Count - 1; i >= 0; i--)
                {
                    parameters[i].Code();
                }
            }
            Encoder.Write(InstructionType.call, function.ID);
            if (function.par_statements.Count != 0)
            {
                for (int i = parameters.Count - 1; i >= 0; i--)
                {
                    Encoder.Write(InstructionType.pop);
                }
            }
        }
    }

    public class FunctionCall_e : Expr
    {
        public symbols.Type return_type;
        public List<Expr> parameters = new List<Expr>();
        public Function function;
        public Class local_class;
        public bool is_head = true;

        public FunctionCall_e(int tag)
        {
            this.tag = tag;
            name = ((Word)Lexer.Peek).lexeme;
        }

        public override void Build()
        {
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
            if (@class == null) @class = Parser.analyzing_class;
            local_class = @class;
            function = local_class.GetFunction(name);
            if (function == null) Error(this, "类型\"" + local_class.name + "\"中未包含\"" + name + "\"的定义");
            if (parameters.Count != function.par_statements.Count) Error("\"" + name + "\"方法没有采用" + parameters.Count + "个参数的重载");
            return_type = function.return_type;
            @class = GetClass(return_type.type_name);
            type = return_type;
            foreach (var item in parameters)
            {
                item.Check();
            }
            return;
        }

        public override void Code()
        {
            Encoder.Write(InstructionType.pusharg);
            if (function.par_statements.Count != 0)
            {
                for (int i = parameters.Count - 1; i >= 0; i--)
                {
                    parameters[i].Code();
                }
            }
            Encoder.Write(InstructionType.call, function.ID);
        }
    }
}
