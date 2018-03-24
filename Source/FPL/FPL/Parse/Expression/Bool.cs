using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using FPL.Encoding;
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
            Left.Check();
            Right.Check();
            if (Left.Type.type_name != Right.Type.type_name) Error(LogContent.NoOverload);
        }

        public override void Code()
        {
            Left.Code();
            if (tag == Tag.AND)
            {
                OrString = new List<CodingUnit>();
                CodingUnit LE = Encoder.Code.Last();
                LE.ins_type = LE.ins_type == InstructionType.jt
                    ? InstructionType.jf
                    : InstructionType.jt;
                AndString.Add(LE);
                Right.Code();
                foreach (CodingUnit codingUnit in AndString)
                {
                    codingUnit.parameter = Encoder.Line + 1;
                }
                return;
            }
            if (tag == Tag.OR)
            {
                AndString = new List<CodingUnit>();
                OrString.Add(Encoder.Code.Last());
                Right.Code();
                foreach (CodingUnit codingUnit in OrString)
                {
                    codingUnit.parameter = Encoder.Line + 1;
                }
                return;
            }
            Right.Code();
            switch (tag)
            {
                case Tag.EQ:
                    Encoder.Write(InstructionType.eq);
                    Encoder.Write(InstructionType.jt);
                    break;
                case Tag.NE:
                    Encoder.Write(InstructionType.eq);
                    Encoder.Write(InstructionType.jf);
                    break;
                case Tag.LE:
                    Encoder.Write(InstructionType.mo);
                    Encoder.Write(InstructionType.jf);
                    break;
                case Tag.GE:
                    Encoder.Write(InstructionType.le);
                    Encoder.Write(InstructionType.jf);
                    break;
                case Tag.LESS:
                    Encoder.Write(InstructionType.le);
                    Encoder.Write(InstructionType.jt);
                    break;
                case Tag.MORE:
                    Encoder.Write(InstructionType.mo);
                    Encoder.Write(InstructionType.jt);
                    break;
            }
        }
    }
}