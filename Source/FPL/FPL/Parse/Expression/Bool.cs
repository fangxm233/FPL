using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using FPL.Generator;
using FPL.LexicalAnalysis;
using FPL.OutPut;

namespace FPL.Parse.Expression
{
    public class Bool : Expr
    {
        static List<CodingUnit> AndString = new List<CodingUnit>();
        static List<CodingUnit> OrString = new List<CodingUnit>();
        public LinkedListNode<Expr> Position;
        private bool isBuilt;

        public Bool(int tag)
        {
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
            Type = symbols.Type.Bool;
            Left.Check();
            Right.Check();
            if ((Left.Type.type_name != symbols.Type.Bool.type_name ||
                 Right.Type.type_name != symbols.Type.Bool.type_name) &&
                (tag == Tag.OR || tag == Tag.AND)) Error(LogContent.ExprError);
            if (Left.Type.type_name != Right.Type.type_name) Error(LogContent.NoOverload);
        }

        public override void Code()
        {
            Left.Code();
            if (tag == Tag.AND)
            {
                OrString = new List<CodingUnit>();
                CodingUnit LE = FILGenerator.Code.Last();
                LE.InsType = LE.InsType == InstructionType.jt
                    ? InstructionType.jf
                    : InstructionType.jt;
                AndString.Add(LE);
                Right.Code();
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