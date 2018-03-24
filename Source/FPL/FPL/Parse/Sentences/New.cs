using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Expression;
using FPL.Parse.Sentences.ProcessControl;
using FPL.Parse.Structure;
using FPL.symbols;

namespace FPL.Parse.Sentences
{
    public class New_s : Sentence
    {
        private Class Class;
        private Function Function;
        private Sentence Next;
        public List<Expr> Parameters = new List<Expr>();
        private Type Type;
        private readonly string TypeName;

        public New_s(int tag) : base(tag)
        {
            Lexer.Next();
            TypeName = ((Word) Lexer.NextToken).Lexeme;
        }

        public override Sentence Build()
        {
            Class = GetClass(TypeName);
            Match("(");
            while (Lexer.NextToken.tag != Tag.RBRACKETS)
            {
                Parameters.Add(new Expr().BuildStart());
                if (Parameters[Parameters.Count - 1] == null)
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
            Lexer.Next();
            return this;
        }

        public override void Check()
        {
            if (Parameters.Count != 0) Error(LogContent.NoOverride);
            Function = Class.GetFunction(TypeName);
            if (Function == null) Error(LogContent.HaventConstructor);
            Type = Type.GetType(Class.Name);
            if (Next != null)
            {
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
        }

        public override void Code()
        {
            Encoder.Write(InstructionType.newobjc, Class.ID);
            Encoder.Write(InstructionType.call, Class.GetFunction(".init").ID);
            if (Function.ParStatements.Count != 0)
                for (int i = Parameters.Count - 1; i >= 0; i--)
                    Parameters[i].Code();
            Encoder.Write(InstructionType.call, Class.GetFunction(Class.Name).ID);
        }

        public Sentence BuildNext()
        {
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.ID) Error(LogContent.SthUseless, Lexer.NextToken);
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
        public List<Expr> Parameters = new List<Expr>();
        private readonly string TypeName;

        public New_e()
        {
            tag = Tag.NEW;
            Lexer.Next();
            TypeName = ((Word) Lexer.NextToken).Lexeme;
        }

        public override void Build()
        {
            Class = GetClass(TypeName);
            Match("(");
            while (Lexer.NextToken.tag != Tag.RBRACKETS)
            {
                Parameters.Add(new Expr().BuildStart());
                if (Parameters[Parameters.Count - 1] != null) continue;
                if (Lexer.NextToken.tag == Tag.COMMA) Error(LogContent.MissingParam);
                Match(")", false);
                Parameters.RemoveAt(Parameters.Count - 1);
                break;
            }
        }

        public override void Check()
        {
            if (Class.GetFunction(Class.Name).ParStatements.Count != Parameters.Count)
                Error(LogContent.ConstructorParmDoesNotMatch, Parameters.Count);
            Class = GetClass(TypeName);
            Function = Class.GetFunction(TypeName);
            if (Function == null) Error(LogContent.HaventConstructor);
            Type = Type.GetType(Class.Name);
        }

        public override void Code()
        {
            Encoder.Write(InstructionType.newobjc, Class.ID);
            Encoder.Write(InstructionType.call, Class.GetFunction(".init").ID);
            if (Function.ParStatements.Count != 0)
                for (int i = Parameters.Count - 1; i >= 0; i--)
                    Parameters[i].Code();
            Encoder.Write(InstructionType.call, Class.GetFunction(Class.Name).ID);
        }
    }
}