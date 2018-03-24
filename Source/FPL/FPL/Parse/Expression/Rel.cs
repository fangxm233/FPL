using System.Collections.Generic;
using System.Linq;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.symbols;

namespace FPL.Parse.Expression
{
    public class Rel : Node
    {
        public Expr Left;

        public List<Rel> Rels = new List<Rel>();
        public Expr Right;
        public int Tag;

        public virtual Rel BuildStart()
        {
            //Left = new Expr().BuildStart();
            //switch (Lexer.NextToken.tag)
            //{
            //    case LexicalAnalysis.Tag.EQ:
            //        {
            //            Rels.Add(new Eq(Left).BuildStart());
            //            break;
            //        }
            //    case LexicalAnalysis.Tag.NE:
            //        {
            //            Rels.Add(new Ne(Left).BuildStart());
            //            break;
            //        }
            //    case LexicalAnalysis.Tag.LE:
            //        {
            //            Rels.Add(new Le(Left).BuildStart());
            //            break;
            //        }
            //    case LexicalAnalysis.Tag.GE:
            //        {
            //            Rels.Add(new Ge(Left).BuildStart());
            //            break;
            //        }
            //    case LexicalAnalysis.Tag.MORE:
            //        {
            //            Rels.Add(new More(Left).BuildStart());
            //            break;
            //        }
            //    case LexicalAnalysis.Tag.LESS:
            //        {
            //            Rels.Add(new Less(Left).BuildStart());
            //            break;
            //        }
            //    default:
            //        {
            //            Rels.Add(new Bool_r(Left));
            //            break;
            //        }
            //}

            //while (true)
            //{
            //    Rel rel = null;
            //    switch (Lexer.NextToken.tag)
            //    {
            //        case LexicalAnalysis.Tag.AND:
            //            rel = new Rel().Build();
            //            Rels.Add(new And(Rels.Last(), rel));
            //            Rels.RemoveAt(Rels.Count - 2);
            //            Rels.Add(((And)Rels.Last()).right);
            //            break;
            //        case LexicalAnalysis.Tag.OR:
            //            rel = new Rel().Build();
            //            Rels.Add(new Or(Rels.Last(), rel));
            //            Rels.RemoveAt(Rels.Count - 2);
            //            Rels.Add(((Or)Rels.Last()).right);
            //            break;
            //        default:
            //            {
            //                if (Rels.Count == 1) return Rels.Last();
            //                Rels.RemoveAt(Rels.Count - 1);
            //                return this;
            //            }
            //    }
            //}
            return null;
        }

        public virtual Rel Build()
        {
            //Left = new Expr().BuildStart();
            //switch (Lexer.NextToken.tag)
            //{
            //    case LexicalAnalysis.Tag.EQ:
            //        {
            //            Eq a = new Eq(Left);
            //            a.BuildStart();
            //            return a;
            //        }
            //    case LexicalAnalysis.Tag.NE:
            //        {
            //            Ne a = new Ne(Left);
            //            a.BuildStart();
            //            return a;
            //        }
            //    case LexicalAnalysis.Tag.LE:
            //        {
            //            Le a = new Le(Left);
            //            a.BuildStart();
            //            return a;
            //        }
            //    case LexicalAnalysis.Tag.GE:
            //        {
            //            Ge a = new Ge(Left);
            //            a.BuildStart();
            //            return a;
            //        }
            //    case LexicalAnalysis.Tag.MORE:
            //        {
            //            More a = new More(Left);
            //            a.BuildStart();
            //            return a;
            //        }
            //    case LexicalAnalysis.Tag.LESS:
            //        {
            //            Less a = new Less(Left);
            //            a.BuildStart();
            //            return a;
            //        }
            //    default:
            //        {
            //            Rels.Add(new Bool_r(Left));
            //            break;
            //        }
            //}

            return null;
        }

        public virtual void Check()
        {
            //if (Rels[0].Tag == LexicalAnalysis.Tag.AND)
            //    ((And)Rels[0]).left.Left.Check();
            //else if (Rels[0].Tag == LexicalAnalysis.Tag.OR)
            //    ((Or)Rels[0]).left.Left.Check();
            //else
            //    Rels[0].Check();
            //foreach (Rel item in Rels) item.Check();
        }

        public virtual List<CodingUnit> Code(int tag)
        {
            var list = new List<CodingUnit>();
            foreach (Rel item in Rels)
                if (item.Tag == LexicalAnalysis.Tag.AND)
                    list.AddRange(item.Code(0));
            if (list.Count != 0) Encoder.Write(InstructionType.jmp, Encoder.Line + 2);
            foreach (CodingUnit item in list) item.parameter = Encoder.Line;
            list = new List<CodingUnit>();
            foreach (Rel item in Rels)
                if (item.Tag == LexicalAnalysis.Tag.OR)
                    list.AddRange(item.Code(0));
            if (list.Count != 0) Encoder.Write(InstructionType.jmp, Encoder.Line + 2);
            foreach (CodingUnit item in list) item.parameter = Encoder.Line;
            return null;
        }
    }

    //public class And : Rel
    //{
    //    public Rel left;
    //    public Rel right;

    //    public And(Rel l)
    //    {
    //        Tag = LexicalAnalysis.Tag.AND;
    //        left = l;
    //    }

    //    public And(Rel l, Rel r)
    //    {
    //        Tag = LexicalAnalysis.Tag.AND;
    //        left = l;
    //        right = r;
    //    }

    //    public override void Check()
    //    {
    //        right.Check();
    //    }

    //    public override List<CodingUnit> Code(int tag)
    //    {
    //        var list = new List<CodingUnit>();
    //        left.Code(LexicalAnalysis.Tag.AND);
    //        Encoder.Write(InstructionType.jmp);
    //        list.Add(Encoder.Code.Last());
    //        right.Code(LexicalAnalysis.Tag.AND);
    //        Encoder.Write(InstructionType.jmp);
    //        list.Add(Encoder.Code.Last());
    //        return list;
    //    }
    //}
    //public class Or : Rel
    //{
    //    public Rel left;
    //    public Rel right;

    //    public Or(Rel l)
    //    {
    //        Tag = LexicalAnalysis.Tag.OR;
    //        left = l;
    //    }

    //    public Or(Rel l, Rel r)
    //    {
    //        Tag = LexicalAnalysis.Tag.OR;
    //        left = l;
    //        right = r;
    //    }

    //    public override void Check()
    //    {
    //        right.Check();
    //    }

    //    public override List<CodingUnit> Code(int tag)
    //    {
    //        var list = new List<CodingUnit>
    //        {
    //            left.Code(LexicalAnalysis.Tag.OR)[0],
    //            right.Code(LexicalAnalysis.Tag.OR)[0]
    //        };
    //        return list;
    //    }
    //}
    //public class Eq : Rel
    //{
    //    public Eq(Expr l)
    //    {
    //        Left = l;
    //    }

    //    public override Rel BuildStart()
    //    {
    //        Right = new Expr().BuildStart();
    //        return this;
    //    }

    //    public override void Check()
    //    {
    //        Left.Check();
    //        Right.Check();
    //        switch (Left.Type.type_name)
    //        {
    //            case "string":
    //            {
    //                if (Right.Type.type_name != "string") Error(this, "表达式无效");
    //                return;
    //            }
    //            case "int":
    //            case "float":
    //            {
    //                if (Right.Type.type_name != "int" && Right.Type.type_name != "float") Error(this, "表达式无效");
    //                return;
    //            }
    //            case "bool":
    //            {
    //                if (Right.Type.type_name != "bool") Error(this, "表达式无效");
    //                return;
    //            }
    //        }
    //    }

    //    public override List<CodingUnit> Code(int tag)
    //    {
    //        Left.Code();
    //        Right.Code();
    //        Encoder.Write(InstructionType.eqt, tag != LexicalAnalysis.Tag.OR ? Encoder.Line + 3 : 0);
    //        return new List<CodingUnit>
    //        {
    //            Encoder.Code.Last()
    //        };
    //    }
    //}
    //public class Ne : Rel
    //{
    //    public Ne(Expr l)
    //    {
    //        Left = l;
    //    }

    //    public override Rel BuildStart()
    //    {
    //        Right = new Expr().BuildStart();
    //        return this;
    //    }

    //    public override void Check()
    //    {
    //        Left.Check();
    //        Right.Check();
    //        switch (Left.Type.type_name)
    //        {
    //            case "string":
    //            {
    //                if (Right.Type.type_name != "string") Error(this, "表达式无效");
    //                return;
    //            }
    //            case "int":
    //            case "float":
    //            {
    //                if (Right.Type.type_name != "int" && Right.Type.type_name != "float") Error(this, "表达式无效");
    //                return;
    //            }
    //            case "bool":
    //            {
    //                if (Right.Type.type_name != "bool") Error(this, "表达式无效");
    //                return;
    //            }
    //        }
    //    }

    //    public override List<CodingUnit> Code(int tag)
    //    {
    //        Left.Code();
    //        Right.Code();
    //        Encoder.Write(InstructionType.eqf, tag != LexicalAnalysis.Tag.OR ? Encoder.Line + 3 : 0);
    //        return new List<CodingUnit>
    //        {
    //            Encoder.Code.Last()
    //        };
    //    }
    //}
    //public class Le : Rel
    //{
    //    public Le(Expr l)
    //    {
    //        Left = l;
    //    }

    //    public override Rel BuildStart()
    //    {
    //        Right = new Expr().BuildStart();
    //        return this;
    //    }

    //    public override void Check()
    //    {
    //        Left.Check();
    //        Right.Check();
    //        switch (Left.Type.type_name)
    //        {
    //            case "int":
    //            case "float":
    //            {
    //                if (Right.Type.type_name != "int" && Right.Type.type_name != "float") Error(this, "表达式无效");
    //                return;
    //            }
    //        }
    //    }

    //    public override List<CodingUnit> Code(int tag)
    //    {
    //        Left.Code();
    //        Right.Code();
    //        Encoder.Write(InstructionType.mof, tag != LexicalAnalysis.Tag.OR ? Encoder.Line + 3 : 0);
    //        return new List<CodingUnit>
    //        {
    //            Encoder.Code.Last()
    //        };
    //    }
    //}
    //public class Ge : Rel
    //{
    //    public Ge(Expr l)
    //    {
    //        Left = l;
    //    }

    //    public override Rel BuildStart()
    //    {
    //        Right = new Expr().BuildStart();
    //        return this;
    //    }

    //    public override void Check()
    //    {
    //        Left.Check();
    //        Right.Check();
    //        switch (Left.Type.type_name)
    //        {
    //            case "int":
    //            case "float":
    //            {
    //                if (Right.Type.type_name != "int" && Right.Type.type_name != "float") Error(this, "表达式无效");
    //                return;
    //            }
    //        }
    //    }

    //    public override List<CodingUnit> Code(int tag)
    //    {
    //        Left.Code();
    //        Right.Code();
    //        Encoder.Write(InstructionType.lef, tag != LexicalAnalysis.Tag.OR ? Encoder.Line + 3 : 0);
    //        return new List<CodingUnit>
    //        {
    //            Encoder.Code.Last()
    //        };
    //    }
    //}
    //public class More : Rel
    //{
    //    public More(Expr l)
    //    {
    //        Left = l;
    //    }

    //    public override Rel BuildStart()
    //    {
    //        Right = new Expr().BuildStart();
    //        return this;
    //    }

    //    public override void Check()
    //    {
    //        Left.Check();
    //        Right.Check();
    //        switch (Left.Type.type_name)
    //        {
    //            case "int":
    //            case "float":
    //            {
    //                if (Right.Type.type_name != "int" && Right.Type.type_name != "float") Error(this, "表达式无效");
    //                return;
    //            }
    //        }
    //    }

    //    public override List<CodingUnit> Code(int tag)
    //    {
    //        Left.Code();
    //        Right.Code();
    //        Encoder.Write(InstructionType.mot, tag != LexicalAnalysis.Tag.OR ? Encoder.Line + 3 : 0);
    //        return new List<CodingUnit>
    //        {
    //            Encoder.Code.Last()
    //        };
    //    }
    //}
    //public class Less : Rel
    //{
    //    public Less(Expr l)
    //    {
    //        Left = l;
    //    }

    //    public override Rel BuildStart()
    //    {
    //        Right = new Expr().BuildStart();
    //        return this;
    //    }

    //    public override void Check()
    //    {
    //        Left.Check();
    //        Right.Check();
    //        switch (Left.Type.type_name)
    //        {
    //            case "int":
    //            case "float":
    //            {
    //                if (Right.Type.type_name != "int" && Right.Type.type_name != "float") Error(this, "表达式无效");
    //                return;
    //            }
    //        }
    //    }

    //    public override List<CodingUnit> Code(int tag)
    //    {
    //        Left.Code();
    //        Right.Code();
    //        Encoder.Write(InstructionType.let, tag != LexicalAnalysis.Tag.OR ? Encoder.Line + 3 : 0);
    //        return new List<CodingUnit>
    //        {
    //            Encoder.Code.Last()
    //        };
    //    }
    //}
    //public class Bool_r : Rel
    //{
    //    public Bool_r(Expr l)
    //    {
    //        Left = l;
    //    }

    //    public override void Check()
    //    {
    //        Left.Check();
    //        if (Left.Type != Type.Bool) Error(LogContent.UnableToConvertType, Left.Type.type_name, "bool");
    //    }

    //    public override List<CodingUnit> Code(int tag)
    //    {
    //        Left.Code();
    //        Encoder.Write(InstructionType.pushval, 1);
    //        Encoder.Write(InstructionType.mot, tag != LexicalAnalysis.Tag.OR ? Encoder.Line + 3 : 0);
    //        return new List<CodingUnit>
    //        {
    //            Encoder.Code.Last()
    //        };
    //    }
    //}
}