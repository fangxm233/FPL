using System.Collections.Generic;
using System.Linq;
using FPL.DataStorager;
using FPL.Generator;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Expression;
using FPL.Parse.Structure;

namespace FPL.Parse.Sentences.ProcessControl
{
    public class FunctionCall_s : Sentence
    {
        public Class Class;
        public Function Function;
        public bool isHead = true;
        public string Name;
        private Sentence Next;
        public List<Parameter> Parameters = new List<Parameter>();
        public Type ReturnType;
        public Type Type;

        public FunctionCall_s(int tag) : base(tag)
        {
            Name = ((Word) Lexer.NextToken).Lexeme;
        }

        public override Sentence Build()
        {
            Match("(");
            while (Lexer.NextToken.tag != Tag.RBRACKETS)
            {
                Parameters.Add(new Parameter(new Expr().BuildStart()));
                if (Parameters.Last().Expr == null)
                {
                    if (Lexer.NextToken.tag == Tag.COMMA) Error(LogContent.MissingParam);
                    Match(")", false);
                    Parameters.RemoveAt(Parameters.Count - 1);
                    break;
                }
            }

            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.DOT)
            {
                Match(";", false);
                return this;
            }

            Next = BuildNext();
            return this;
        }

        public Sentence BuildNext()
        {
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.ID) Error(LogContent.SthUnexpect, Lexer.NextToken);
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
            if (Class == null) Class = Parser.AnalyzingClass;
            foreach (Parameter item in Parameters) item.Check();
            Function = Class.GetFunction(this, Name, Parameters);
            ReturnType = Function.ReturnType;
            if (Next != null)
            {
                if (ReturnType.tag == Tag.VOID) Error(Next, LogContent.OperandNonsupport, ".", "void");
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

            //if (Next == null) return;
        }

        public override void Code()
        {
            for (int i = Parameters.Count - 1; i >= 0; i--)
                Parameters[i].Code();
            if (Function.FuncType != FuncType.Static)
                FILGenerator.Write(InstructionType.pusharg);

            FILGenerator.Write(InstructionType.call, Function.ID);

            for (int i = Parameters.Count - 1; i >= 0; i--)
                FILGenerator.Write(InstructionType.pop);
            if (Function.FuncType != FuncType.Static)
                FILGenerator.Write(InstructionType.pop);
        }
    }

    public class FunctionCall_e : Expr
    {
        public Function Function;
        public bool isHead = true;
        public Class LocalClass;
        public List<Parameter> Parameters = new List<Parameter>();
        public Type ReturnType;

        public FunctionCall_e(int tag)
        {
            this.tag = tag;
            Name = ((Word) Lexer.NextToken).Lexeme;
        }

        public override void Build()
        {
            Match("(");
            while (Lexer.NextToken.tag != Tag.RBRACKETS)
            {
                Parameters.Add(new Parameter(new Expr().BuildStart()));
                if (Parameters.Last().Expr != null) continue;
                if (Lexer.NextToken.tag == Tag.COMMA) Error(LogContent.MissingParam);
                Match(")", false);
                Parameters.RemoveAt(Parameters.Count - 1);
                break;
            }
        }

        public override void Check()
        {
            if (Class == null) Class = Parser.AnalyzingClass;
            LocalClass = Class;
            Function = LocalClass.GetFunction(this, Name, Parameters);
            if (Function == null) Error(LogContent.NotExistingDefinitionInType, LocalClass.Name, Name);
            if (Parameters.Count != Function.Parameters.Count)
                Error(LogContent.NumberOfParamDoesNotMatch, Name, Parameters.Count);
            ReturnType = Function.ReturnType;
            Class = GetClass(ReturnType.type_name);
            Type = ReturnType;
            foreach (Parameter item in Parameters) item.Check();
        }

        public override void Code()
        {
            for (int i = Parameters.Count - 1; i >= 0; i--)
                Parameters[i].Code();
            if (Function.FuncType != FuncType.Static)
                FILGenerator.Write(InstructionType.pusharg);

            FILGenerator.Write(InstructionType.call, Function.ID);

            for (int i = Parameters.Count - 1; i >= 0; i--)
                FILGenerator.Write(InstructionType.pop);
            if (Function.FuncType != FuncType.Static)
                FILGenerator.Write(InstructionType.pop);
        }
    }
}