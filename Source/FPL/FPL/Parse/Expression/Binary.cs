using FPL.DataStorager;
using FPL.Generator;
using FPL.OutPut;
using System.Collections.Generic;
using FPL.Classification;
using FPL.LexicalAnalysis;
using FPL.Parse.Structure;

namespace FPL.Parse.Expression
{
    public class Binary : Expr
    {
        public LinkedListNode<Expr> Position;
        private bool isBuilt;
        private bool isOverloaded;
        private Function OverloadFunction;

        public Binary(int tag)
        {
            Name = Lexer.NextToken.ToString();
            this.tag = tag;
        }

        public void Set_position(LinkedListNode<Expr> pos)
        {
            Position = pos;
        }

        public override void Build()
        {
            if (isBuilt) return;
            Left = Position.Previous.Value;
            Right = Position.Next.Value;
            Position.List.Remove(Left);
            Position.List.Remove(Right);
            isBuilt = true;
        }

        public override void Check()
        {
            if (tag == Tag.DOT)
            {
                DotCheck();
                return;
            }

            if (Left == null || Right == null) Error(LogContent.ExprError);
            Left.Check();
            Right.Check();
            if (Classifier.ClassificateIn(ClassificateMethod.VarType, Left.Type.type_name) != Tag.BASIC ||
                Classifier.ClassificateIn(ClassificateMethod.VarType, Right.Type.type_name) != Tag.BASIC)
            {
                Parameter[] parameters = {
                    new Parameter(Left.Type, Left.Name),
                    new Parameter(Right.Type, Right.Name)
                };
                if (GetClass(Left.Type.type_name).ContainsFunction(Name, parameters))
                    OverloadFunction = GetClass(Left.Type.type_name).GetFunction(this, Name, parameters);
                else if (GetClass(Right.Type.type_name).ContainsFunction(Name, parameters))
                    OverloadFunction = GetClass(Right.Type.type_name).GetFunction(this, Name, parameters);
                else
                    Error(LogContent.OperandNonsupportD, Name, Left.Type, Right.Type);
                isOverloaded = true;
                Type = OverloadFunction.ReturnType;
            }
            if (!isOverloaded)
                Type = Left.Type;
        }

        public void DotCheck()
        {
            Left.Check();
            Right.Class = Left.Class;
            Right.Check();
            Type = Right.Type;
            Class = Right.Class;
        }

        public override void Code()
        {
            Left.Code();
            Right.Code();

            if (tag == Tag.DOT) return;
            if (isOverloaded)
            {
                FILGenerator.Write(InstructionType.call, OverloadFunction.ID);
                FILGenerator.Write(InstructionType.pop);
                FILGenerator.Write(InstructionType.pop);
                return;
            }
            if (Parser.InsTable.ContainsKey(tag))
                if (Parser.InsTable[tag].ContainsKey(Type.type_name))
                    FILGenerator.Write(Parser.InsTable[tag][Type.type_name]);
                else
                    Error(LogContent.NoOverride); //要改..
            else
                Error(LogContent.ExprError);
        }
    }
}