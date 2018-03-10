using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Expression;
using FPL.Parse.Structure;
using FPL.symbols;

namespace FPL.Parse.Sentences
{
    public class FunctionCall_s : Sentence
    {
        public Class Class;
        public Function Function;
        public bool isHead = true;
        public string Name;
        private Sentence Next;
        public List<Expr> Parameters = new List<Expr>();
        public Type ReturnType;
        public Type Type;

        public FunctionCall_s(int tag) : base(tag)
        {
            Name = ((Word) Lexer.NextToken).Lexeme;
        }

        public override Sentence Build()
        {
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            while (Lexer.NextToken.tag != Tag.RBRACKETS)
            {
                Parameters.Add(new Expr().BuildStart());
                if (Parameters[Parameters.Count - 1] == null)
                {
                    if (Lexer.NextToken.tag == Tag.COMMA) Error("缺少参数");
                    if (Lexer.NextToken.tag != Tag.RBRACKETS) Error("应输入\")\"");
                    Parameters.RemoveAt(Parameters.Count - 1);
                    break;
                }
            }

            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.DOT)
            {
                if (Lexer.NextToken.tag != Tag.SEMICOLON) Error("应输入\";\"");
                return this;
            }

            Next = BuildNext();
            return this;
        }

        public Sentence BuildNext()
        {
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.ID) Error("\"" + Lexer.NextToken + "\"无效");
            Lexer.Next();
            if (Lexer.NextToken.tag == Tag.LBRACKETS)
            {
                Lexer.Back();
                return new FunctionCall_s(Tag.FUNCTIONCALL).Build();
            }

            Lexer.Back();
            return new Object_s(Tag.OBJECT).Build();
        }

        public override void Check()
        {
            Function = Class.GetFunction(Name);
            if (Function == null) Error(this, "类型\"" + Class.Name + "\"中未包含\"" + Name + "\"的定义");
            if (Parameters.Count != Function.ParStatements.Count)
                Error("\"" + Name + "\"方法没有采用" + Parameters.Count + "个参数的重载");
            ReturnType = Function.ReturnType;
            if (Next != null)
            {
                if (ReturnType.tag == Tag.VOID) Error(Next, "运算符\".\"无法应用于\"void\"类型的操作数");
                if (Next.tag == Tag.FUNCTIONCALL)
                {
                    ((FunctionCall_s) Next).Class = GetClass(ReturnType.type_name);
                    ((FunctionCall_s) Next).isHead = false;
                }

                if (Next.tag == Tag.OBJECT)
                {
                    ((Object_s) Next).Class = GetClass(ReturnType.type_name);
                    ((Object_s) Next).IsHead = false;
                }

                Next.Check();
            }

            foreach (Expr item in Parameters) item.Check();
            if (Next == null) return;
        }

        public override void Code()
        {
            if (Function.FuncType != FuncType.Static)
                Encoder.Write(InstructionType.pusharg);
            if (Function.ParStatements.Count != 0)
                for (int i = Parameters.Count - 1; i >= 0; i--)
                    Parameters[i].Code();
            Encoder.Write(InstructionType.call, Function.ID);
            if (Function.ParStatements.Count != 0)
                for (int i = Parameters.Count - 1; i >= 0; i--)
                    Encoder.Write(InstructionType.pop);
        }
    }

    public class FunctionCall_e : Expr
    {
        public Function function;
        public bool is_head = true;
        public Class local_class;
        public List<Expr> parameters = new List<Expr>();
        public Type return_type;

        public FunctionCall_e(int tag)
        {
            this.Tag = tag;
            Name = ((Word) Lexer.NextToken).Lexeme;
        }

        public override void Build()
        {
            Lexer.Next();
            if (Lexer.NextToken.tag != LexicalAnalysis.Tag.LBRACKETS) Error("应输入\"(\"");
            while (Lexer.NextToken.tag != LexicalAnalysis.Tag.RBRACKETS)
            {
                parameters.Add(new Expr().BuildStart());
                if (parameters[parameters.Count - 1] != null) continue;
                if (Lexer.NextToken.tag == LexicalAnalysis.Tag.COMMA) Error("缺少参数");
                if (Lexer.NextToken.tag != LexicalAnalysis.Tag.RBRACKETS) Error("应输入\")\"");
                parameters.RemoveAt(parameters.Count - 1);
                break;
            }
        }

        public override void Check()
        {
            if (Class == null) Class = Parser.AnalyzingClass;
            local_class = Class;
            function = local_class.GetFunction(Name);
            if (function == null) Error(this, "类型\"" + local_class.Name + "\"中未包含\"" + Name + "\"的定义");
            if (parameters.Count != function.ParStatements.Count)
                Error("\"" + Name + "\"方法没有采用" + parameters.Count + "个参数的重载");
            return_type = function.ReturnType;
            Class = GetClass(return_type.type_name);
            Type = return_type;
            foreach (Expr item in parameters) item.Check();
        }

        public override void Code()
        {
            Encoder.Write(InstructionType.pusharg);
            if (function.ParStatements.Count != 0)
                for (int i = parameters.Count - 1; i >= 0; i--)
                    parameters[i].Code();
            Encoder.Write(InstructionType.call, function.ID);
        }
    }
}