﻿using FPL.DataStorager;
using FPL.Generator;
using FPL.OutPut;
using FPL.Parse.Expression;
using FPL.Parse.Structure;

namespace FPL.Parse.Sentences.ProcessControl
{
    internal class Return : Sentence
    {
        public Class Class;
        public Expr Expr;
        private Function Function;
        private readonly string FunctionName;
        private Type RetrunType;

        public Return(int tag) : base(tag)
        {
            Function = Parser.AnalyzingFunction;
        }

        public override Sentence Build()
        {
            Class = Parser.AnalyzingClass;
            Expr = new Expr().BuildStart();
            Match(";", false);
            return this;
        }

        public override void Check()
        {
            RetrunType = Function.ReturnType;
            if (Expr == null)
            {
                if (RetrunType != Type.Void) Error(LogContent.ReturnValueMissing, RetrunType);
                return;
            }
            Expr.Check();
            if (Expr.Type != RetrunType)
                Error(Expr, LogContent.UnableToConvertType, Expr.Type,
                    RetrunType);
        }

        public override void Code()
        {
            //if (FunctionName == "Main")
            //{
            //    Encoder.Write(InstructionType.endP);
            //    return;
            //}

            if (Expr != null)
                Expr.Code();
            else FILGenerator.Write(InstructionType.pushval);
            //Encoder.Write(InstructionType.popEAX);

            //for (int i = 0; i < Function.Statements.Count; i++) Encoder.Write(InstructionType.pop);
            FILGenerator.Write(InstructionType.ret);
        }
    }
}