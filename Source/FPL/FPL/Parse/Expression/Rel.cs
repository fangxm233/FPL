using System.Collections.Generic;
using System.Linq;
using FPL.Encoding;
using FPL.LexicalAnalysis;

namespace FPL.Parse.Expression
{
    public class Rel : Node
    {
        public Expr left;
        public Expr right;
        public int tag;

        public List<Rel> rels = new List<Rel>();

        public virtual Rel BuildStart()
        {
            left = new Expr().BuildStart();
            switch (Lexer.Peek.tag)
            {
                case Tag.EQ:
                    {
                        rels.Add(new Eq(left).BuildStart());
                        break;
                    }
                case Tag.NE:
                    {
                        rels.Add(new Ne(left).BuildStart());
                        break;
                    }
                case Tag.LE:
                    {
                        rels.Add(new Le(left).BuildStart());
                        break;
                    }
                case Tag.GE:
                    {
                        rels.Add(new Ge(left).BuildStart());
                        break;
                    }
                case Tag.MORE:
                    {
                        rels.Add(new More(left).BuildStart());
                        break;
                    }
                case Tag.LESS:
                    {
                        rels.Add(new Less(left).BuildStart());
                        break;
                    }
                default:
                    {
                        rels.Add(new Bool_r(left));
                        break;
                    }
            }
            while (true)
            {
                Rel rel = null;
                switch (Lexer.Peek.tag)
                {
                    case Tag.AND:
                        rel = new Rel().Build();
                        rels.Add(new And(rels.Last(), rel));
                        rels.RemoveAt(rels.Count - 2);
                        rels.Add(((And)rels.Last()).right);
                        break;
                    case Tag.OR:
                        rel = new Rel().Build();
                        rels.Add(new Or(rels.Last(), rel));
                        rels.RemoveAt(rels.Count - 2);
                        rels.Add(((Or)rels.Last()).right);
                        break;
                    default:
                        {
                            if (rels.Count == 1) return rels.Last();
                            rels.RemoveAt(rels.Count - 1);
                            return this;
                        }
                }
            }
        }

        public virtual Rel Build()
        {
            left = new Expr().BuildStart();
            switch (Lexer.Peek.tag)
            {
                case Tag.EQ:
                    {
                        Eq a = new Eq(left);
                        a.BuildStart();
                        return a;
                    }
                case Tag.NE:
                    {
                        Ne a = new Ne(left);
                        a.BuildStart();
                        return a;
                    }
                case Tag.LE:
                    {
                        Le a = new Le(left);
                        a.BuildStart();
                        return a;
                    }
                case Tag.GE:
                    {
                        Ge a = new Ge(left);
                        a.BuildStart();
                        return a;
                    }
                case Tag.MORE:
                    {
                        More a = new More(left);
                        a.BuildStart();
                        return a;
                    }
                case Tag.LESS:
                    {
                        Less a = new Less(left);
                        a.BuildStart();
                        return a;
                    }
                default:
                    {
                        rels.Add(new Bool_r(left));
                        break;
                    }
            }
            return null;
        }

        public virtual void Check()
        {
            if(rels[0].tag == Tag.AND)
            {
                ((And)rels[0]).left.left.Check();
            }
            else if(rels[0].tag == Tag.OR)
            {
                ((Or)rels[0]).left.left.Check();
            }
            else
            {
                rels[0].Check();
            }
            foreach (var item in rels)
            {
                item.Check();
            }
            return;
        }

        public virtual List<CodingUnit> Code(int tag)
        {
            List<CodingUnit> list = new List<CodingUnit>();
            foreach (var item in rels)
            {
                if(item.tag == Tag.AND)
                {
                    list.AddRange(item.Code(0));
                }
            }
            if (list.Count != 0) Encoder.Write(InstructionType.jmp, Encoder.line + 2);
            foreach (var item in list)
            {
                item.parameter = Encoder.line;
            }
            list = new List<CodingUnit>();
            foreach (var item in rels)
            {
                if (item.tag == Tag.OR)
                {
                    list.AddRange(item.Code(0));
                }
            }
            if (list.Count != 0) Encoder.Write(InstructionType.jmp, Encoder.line + 2);
            foreach (var item in list)
            {
                item.parameter = Encoder.line;
            }
            return null;
        }
    }

    public class And : Rel
    {
        public new Rel left;
        public new Rel right;
        public And(Rel l)
        {
            tag = Tag.AND;
            left = l;
        }
        public And(Rel l, Rel r)
        {
            tag = Tag.AND;
            left = l;
            right = r;
        }

        public override void Check()
        {
            right.Check();
        }

        public override List<CodingUnit> Code(int tag)
        {
            List<CodingUnit> list = new List<CodingUnit>();
            left.Code(Tag.AND);
            Encoder.Write(InstructionType.jmp);
            list.Add(Encoder.code.Last());
            right.Code(Tag.AND);
            Encoder.Write(InstructionType.jmp);
            list.Add(Encoder.code.Last());
            return list;
        }
    }
    public class Or : Rel
    {
        public new Rel left;
        public new Rel right;
        public Or(Rel l)
        {
            tag = Tag.OR;
            left = l;
        }
        public Or(Rel l, Rel r)
        {
            tag = Tag.OR;
            left = l;
            right = r;
        }

        public override void Check()
        {
            right.Check();
        }

        public override List<CodingUnit> Code(int tag)
        {
            List<CodingUnit> list = new List<CodingUnit>
            {
                left.Code(Tag.OR)[0],
                right.Code(Tag.OR)[0]
            };
            return list;
        }
    }
    public class Eq : Rel
    {
        public Eq(Expr l)
        {
            left = l;
        }

        public override Rel BuildStart()
        {
            right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            left.Check();
            right.Check();
            switch (left.type.type_name)
            {
                case "string":
                    {
                        if (right.type.type_name != "string") Error(this, "表达式无效");
                        return;
                    }
                case "int":
                case "float":
                    {
                        if (right.type.type_name != "int" && right.type.type_name != "float") Error(this, "表达式无效");
                        return;
                    }
                case "bool":
                    {
                        if (right.type.type_name != "bool") Error(this, "表达式无效");
                        return;
                    }
            }
        }

        public override List<CodingUnit> Code(int tag)
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionType.eqt, tag != Tag.OR ? Encoder.line + 3 : 0);
            return new List<CodingUnit>()
            {
                Encoder.code.Last()
            };
        }
    }
    public class Ne : Rel
    {
        public Ne(Expr l)
        {
            left = l;
        }

        public override Rel BuildStart()
        {
            right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            left.Check();
            right.Check();
            switch (left.type.type_name)
            {
                case "string":
                    {
                        if (right.type.type_name != "string") Error(this, "表达式无效");
                        return;
                    }
                case "int":
                case "float":
                    {
                        if (right.type.type_name != "int" && right.type.type_name != "float") Error(this, "表达式无效");
                        return;
                    }
                case "bool":
                    {
                        if (right.type.type_name != "bool") Error(this, "表达式无效");
                        return;
                    }
            }
        }

        public override List<CodingUnit> Code(int tag)
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionType.eqf, tag != Tag.OR ? Encoder.line + 3 : 0);
            return new List<CodingUnit>()
            {
                Encoder.code.Last()
            };
        }
    }
    public class Le : Rel
    {
        public Le(Expr l)
        {
            left = l;
        }

        public override Rel BuildStart()
        {
            right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            left.Check();
            right.Check();
            switch (left.type.type_name)
            {
                case "int":
                case "float":
                    {
                        if (right.type.type_name != "int" && right.type.type_name != "float") Error(this, "表达式无效");
                        return;
                    }
            }
        }

        public override List<CodingUnit> Code(int tag)
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionType.mof, tag != Tag.OR ? Encoder.line + 3 : 0);
            return new List<CodingUnit>()
            {
                Encoder.code.Last()
            };
        }
    }
    public class Ge : Rel
    {
        public Ge(Expr l)
        {
            left = l;
        }

        public override Rel BuildStart()
        {
            right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            left.Check();
            right.Check();
            switch (left.type.type_name)
            {
                case "int":
                case "float":
                    {
                        if (right.type.type_name != "int" && right.type.type_name != "float") Error(this, "表达式无效");
                        return;
                    }
            }
        }

        public override List<CodingUnit> Code(int tag)
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionType.lef, tag != Tag.OR ? Encoder.line + 3 : 0);
            return new List<CodingUnit>()
            {
                Encoder.code.Last()
            };        }
    }
    public class More : Rel
    {
        public More(Expr l)
        {
            left = l;
        }

        public override Rel BuildStart()
        {
            right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            left.Check();
            right.Check();
            switch (left.type.type_name)
            {
                case "int":
                case "float":
                    {
                        if (right.type.type_name != "int" && right.type.type_name != "float") Error(this, "表达式无效");
                        return;
                    }
            }
        }

        public override List<CodingUnit> Code(int tag)
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionType.mot, tag != Tag.OR ? Encoder.line + 3 : 0);
            return new List<CodingUnit>()
            {
                Encoder.code.Last()
            };
        }
    }
    public class Less : Rel
    {
        public Less(Expr l)
        {
            left = l;
        }

        public override Rel BuildStart()
        {
            right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            left.Check();
            right.Check();
            switch (left.type.type_name)
            {
                case "int":
                case "float":
                    {
                        if (right.type.type_name != "int" && right.type.type_name != "float") Error(this, "表达式无效");
                        return;
                    }
            }
        }

        public override List<CodingUnit> Code(int tag)
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionType.let, tag != Tag.OR ? Encoder.line + 3 : 0);
            return new List<CodingUnit>()
            {
                Encoder.code.Last()
            };
        }
    }
    public class Bool_r : Rel
    {
        public Bool_r(Expr l)
        {
            left = l;
        }

        public override void Check()
        {
            left.Check();
            if (left.type != symbols.Type.Bool) Error(this, "无法将\"" + left.type.type_name + "\"类型转换为\"bool\"");
        }

        public override List<CodingUnit> Code(int tag)
        {
            left.Code();
            Encoder.Write(InstructionType.pushval, 1);
            Encoder.Write(InstructionType.mot, tag != Tag.OR ? Encoder.line + 3 : 0);
            return new List<CodingUnit>()
            {
                Encoder.code.Last()
            };
        }
    }
}
