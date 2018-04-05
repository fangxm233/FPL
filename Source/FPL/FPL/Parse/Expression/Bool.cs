using FPL.DataStorager;
using FPL.Generator;
using FPL.OutPut;
using System.Collections.Generic;
using System.Linq;
using FPL.Classification;
using FPL.LexicalAnalysis;
using FPL.Parse.Structure;

namespace FPL.Parse.Expression
{
    public class Bool : Expr
    {
        public static List<CodingUnit> AndString = new List<CodingUnit>();
        public static List<CodingUnit> OrString = new List<CodingUnit>();
        public LinkedListNode<Expr> Position;
        private Function OverloadFunction;
        private bool isBuilt;
        private bool isOverloaded;

        public Bool(int tag)
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
            Type = Type.Bool;
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
        }

        public override void Code()
        {
            Left.Code();
            if (tag == Tag.AND)
            {
                OrString = new List<CodingUnit>();
                CodingUnit LE;
                if (Left.tag != Tag.AND)
                {
                    LE = FILGenerator.Code.Last();
                    LE.InsType = LE.InsType == InstructionType.jt
                        ? InstructionType.jf
                        : InstructionType.jt;
                    AndString.Add(LE);
                }
                Right.Code();
                LE = FILGenerator.Code.Last();
                LE.InsType = LE.InsType == InstructionType.jt
                    ? InstructionType.jf
                    : InstructionType.jt;
                AndString.Add(LE);
                foreach (CodingUnit codingUnit in AndString)
                {
                    codingUnit.Parameter = FILGenerator.Line + 1;
                }
                return;
            }
            if (tag == Tag.OR)
            {
                AndString = new List<CodingUnit>();
                OrString.Add(FILGenerator.Code.Last());
                Right.Code();
                OrString.Add(FILGenerator.Code.Last());
                foreach (CodingUnit codingUnit in OrString)
                {
                    codingUnit.Parameter = FILGenerator.Line + 1;
                }
                return;
            }
            Right.Code();
            switch (tag)
            {
                case Tag.EQ:
                    FILGenerator.Write(InstructionType.eq);
                    FILGenerator.Write(InstructionType.jt);
                    break;
                case Tag.NE:
                    FILGenerator.Write(InstructionType.eq);
                    FILGenerator.Write(InstructionType.jf);
                    break;
                case Tag.LE:
                    FILGenerator.Write(InstructionType.mo);
                    FILGenerator.Write(InstructionType.jf);
                    break;
                case Tag.GE:
                    FILGenerator.Write(InstructionType.le);
                    FILGenerator.Write(InstructionType.jf);
                    break;
                case Tag.LESS:
                    FILGenerator.Write(InstructionType.le);
                    FILGenerator.Write(InstructionType.jt);
                    break;
                case Tag.MORE:
                    FILGenerator.Write(InstructionType.mo);
                    FILGenerator.Write(InstructionType.jt);
                    break;
            }
        }
    }
}