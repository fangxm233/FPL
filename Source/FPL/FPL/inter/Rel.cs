using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;
using FPL.Encoding;

namespace FPL.inter
{
    [Serializable]
    public class Rel : Node
    {
        public Expr left;
        public Expr right;
        //public Token content;

        public virtual Rel Build()
        {
            left = new Expr().BuildStart();
            switch (Lexer.Peek.tag)
            {
                case Tag.EQ:
                    {
                        Eq a = new Eq(left);
                        a.Build();
                        return a;
                    }
                case Tag.NE:
                    {
                        Ne a = new Ne(left);
                        a.Build();
                        return a;
                    }
                case Tag.LE:
                    {
                        Le a = new Le(left);
                        a.Build();
                        return a;
                    }
                case Tag.GE:
                    {
                        Ge a = new Ge(left);
                        a.Build();
                        return a;
                    }
                case Tag.MORE:
                    {
                        More a = new More(left);
                        a.Build();
                        return a;
                    }
                case Tag.LESS:
                    {
                        Less a = new Less(left);
                        a.Build();
                        return a;
                    }
            }
            return null;
        }

        public virtual void Check()
        {
            if (left.content.tag != Tag.TRUE && left.content.tag != Tag.FALSE)
            {
                Error(this, "表达式无效");
                return;
            }
            return;
        }

        public virtual void Code()
        {
            return;
        }
    }

    [Serializable]
    public class Eq : Rel
    {
        public Eq(Expr l)
        {
            left = l;
        }

        public override Rel Build()
        {
            right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            if (left.Check()) left = left.ToStringPlus();
            if (right.Check()) right = right.ToStringPlus();
            switch (left.type.type)
            {
                case "string":
                    {
                        if (right.type.type != "string") Error(this, "表达式无效");
                        return;
                    }
                case "int":
                case "float":
                    {
                        if (right.type.type != "int" && right.type.type != "float") Error(this, "表达式无效");
                        return;
                    }
                case "bool":
                    {
                        if (right.type.type != "bool") Error(this, "表达式无效");
                        return;
                    }
            }
        }

        public override void Code()
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionsType.eqt);
        }
    }
    [Serializable]
    public class Ne : Rel
    {
        public Ne(Expr l)
        {
            left = l;
        }

        public override Rel Build()
        {
            right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            if (left.Check()) left = left.ToStringPlus();
            if (right.Check()) right = right.ToStringPlus();
            switch (left.type.type)
            {
                case "string":
                    {
                        if (right.type.type != "string") Error(this, "表达式无效");
                        return;
                    }
                case "int":
                case "float":
                    {
                        if (right.type.type != "int" && right.type.type != "float") Error(this, "表达式无效");
                        return;
                    }
                case "bool":
                    {
                        if (right.type.type != "bool") Error(this, "表达式无效");
                        return;
                    }
            }
        }

        public override void Code()
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionsType.eqf);
        }
    }
    [Serializable]
    public class Le : Rel
    {
        public Le(Expr l)
        {
            left = l;
        }

        public override Rel Build()
        {
            right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            if (left.Check()) left = left.ToStringPlus();
            if (right.Check()) right = right.ToStringPlus();
            switch (left.type.type)
            {
                case "int":
                case "float":
                    {
                        if (right.type.type != "int" && right.type.type != "float") Error(this, "表达式无效");
                        return;
                    }
            }
        }

        public override void Code()
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionsType.mof);
        }
    }
    [Serializable]
    public class Ge : Rel
    {
        public Ge(Expr l)
        {
            left = l;
        }

        public override Rel Build()
        {
            right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            if (left.Check()) left = left.ToStringPlus();
            if (right.Check()) right = right.ToStringPlus();
            switch (left.type.type)
            {
                case "int":
                case "float":
                    {
                        if (right.type.type != "int" && right.type.type != "float") Error(this, "表达式无效");
                        return;
                    }
            }
        }

        public override void Code()
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionsType.lef);
        }
    }
    [Serializable]
    public class More : Rel
    {
        public More(Expr l)
        {
            left = l;
        }

        public override Rel Build()
        {
            right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            if (left.Check()) left = left.ToStringPlus();
            if (right.Check()) right = right.ToStringPlus();
            switch (left.type.type)
            {
                case "int":
                case "float":
                    {
                        if (right.type.type != "int" && right.type.type != "float") Error(this, "表达式无效");
                        return;
                    }
            }
        }

        public override void Code()
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionsType.mot);
        }
    }
    [Serializable]
    public class Less : Rel
    {
        public Less(Expr l)
        {
            left = l;
        }

        public override Rel Build()
        {
            right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            if (left.Check()) left = left.ToStringPlus();
            if (right.Check()) right = right.ToStringPlus();
            switch (left.type.type)
            {
                case "int":
                case "float":
                    {
                        if (right.type.type != "int" && right.type.type != "float") Error(this, "表达式无效");
                        return;
                    }
            }
        }

        public override void Code()
        {
            left.Code();
            right.Code();
            Encoder.Write(InstructionsType.let);
        }
    }
}
