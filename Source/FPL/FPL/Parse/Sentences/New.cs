using System.Collections.Generic;
using System.Linq;
using FPL.DataStorager;
using FPL.Generator;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Expression;
using FPL.Parse.Sentences.ProcessControl;
using FPL.Parse.Structure;

namespace FPL.Parse.Sentences
{
    public class New_s : Sentence
    {
        private Class Class;
        private Function Function;
        private Sentence Next;
        public List<Parameter> Parameters = new List<Parameter>();
        private Type Type;
        private readonly string TypeName;

        public New_s(int tag) : base(tag)
        {
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.ID) ErrorSta(LogContent.IDExpect);
            TypeName = Lexer.NextToken.ToString();
        }

        public override Sentence Build()
        {
            Class = GetClass(TypeName);
            Match("(");
            while (Lexer.NextToken.tag != Tag.RBRACKETS)
            {
                Parameters.Add(new Parameter(new Expr().BuildStart()));
                if (Parameters.Last().Expr == null)
                {
                    if (Lexer.NextToken.tag == Tag.COMMA) ErrorSta(LogContent.MissingParam);
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
            Lexer.Next();
            return this;
        }

        public override void Check()
        {
            Function = Class.GetFunction(this, TypeName, Parameters);
            if (Function == null) Error(LogContent.HaventConstructor);
            Type = Type.GetType(TypeName);
            if (Next == null) return;
            if (Next.tag == Tag.FUNCTIONCALL)
            {
                ((FunctionCall_s) Next).Class = GetClass(Type.type_name);
                ((FunctionCall_s) Next).isHead = false;
            }

            if (Next.tag == Tag.OBJECT)
            {
                ((Object_s) Next).Class = GetClass(Type.type_name);
                ((FunctionCall_s) Next).isHead = false;
            }

            Next.Check();
        }

        public override void Code()
        {
            FILGenerator.Write(InstructionType.newobjc, Class.ID);
            FILGenerator.Write(InstructionType.call, Class.GetFunction(this, ".init").ID);
            if (Function.Parameters.Count != 0)
                for (int i = Parameters.Count - 1; i >= 0; i--)
                    Parameters[i].Code();
            FILGenerator.Write(InstructionType.call, Function.ID);
        }

        public Sentence BuildNext()
        {
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.ID) ErrorSta(LogContent.SthUseless, Lexer.NextToken);
            Lexer.Next();
            if (Lexer.NextToken.tag == Tag.LBRACKETS)
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
        private Function Function;
        public List<Parameter> Parameters = new List<Parameter>();
        private readonly string TypeName;

        public New_e()
        {
            tag = Tag.NEW;
            Lexer.Next();
            TypeName = Lexer.NextToken.ToString();
        }

        public override void Build()
        {
            Class = GetClass(TypeName);
            Match("(");
            while (Lexer.NextToken.tag != Tag.RBRACKETS)
            {
                Parameters.Add(new Parameter(new Expr().BuildStart()));
                if (Parameters.Last().Expr != null) continue;
                if (Lexer.NextToken.tag == Tag.COMMA) ErrorSta(LogContent.MissingParam);
                Match(")", false);
                Parameters.RemoveAt(Parameters.Count - 1);
                break;
            }
        }

        public override void Check()
        {
            foreach (Parameter parameter in Parameters) parameter.Check();

            if (!Class.ContainsFunction(TypeName, out bool h, Parameters))
                Error(LogContent.ConstructorParmDoesNotMatch, Parameters.Count);
            Class = GetClass(TypeName);
            Function = Class.GetFunction(this, TypeName, Parameters);
            if (Function == null) Error(LogContent.HaventConstructor);
            Type = Type.GetType(TypeName);
        }

        public override void Code()
        {
            FILGenerator.Write(InstructionType.newobjc, Class.ID);
            FILGenerator.Write(InstructionType.call, Class.GetFunction(this, ".init").ID);
            if (Function.Parameters.Count != 0)
                for (int i = Parameters.Count - 1; i >= 0; i--)
                    Parameters[i].Code();
            FILGenerator.Write(InstructionType.call, Function.ID);
        }

        public override int GetTokenLength()
        {
            int l = 3;
            foreach (Parameter parameter in Parameters)
            {
                l += parameter.GetTokenLength();
                l++;
            }

            return l;
        }
    }
}