﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;

namespace FPL.inter
{
    [Serializable]
    public class Rel : Node
    {
        public Expr left;
        public Expr right;
        //public Token content;

        public virtual Rel Build(Lexer lex)
        {
            left = new Expr().BuildStart(lex);
            switch (Lexer.Peek.tag)
            {
                case Tag.EQ:
                    {
                        Eq a = new Eq(left);
                        a.Build(lex);
                        return a;
                    }
                case Tag.NE:
                    {
                        Ne a = new Ne(left);
                        a.Build(lex);
                        return a;
                    }
                case Tag.LE:
                    {
                        Le a = new Le(left);
                        a.Build(lex);
                        return a;
                    }
                case Tag.GE:
                    {
                        Ge a = new Ge(left);
                        a.Build(lex);
                        return a;
                    }
                case Tag.MORE:
                    {
                        More a = new More(left);
                        a.Build(lex);
                        return a;
                    }
                case Tag.LESS:
                    {
                        Less a = new Less(left);
                        a.Build(lex);
                        return a;
                    }
            }
            return this;
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

        public virtual bool Run()
        {
            return (bool)left.Run();
        }
    }

    [Serializable]
    public class Eq : Rel
    {
        public Eq(Expr l)
        {
            left = l;
        }

        public override Rel Build(Lexer lex)
        {
            right = new Expr().BuildStart(lex);
            return this;
        }

        public override void Check()
        {
            left.Check();
            if (Expr.turn_to_string)
            {
                left = left.ToStringPlus();
                Expr.turn_to_string = false;
            }
            right.Check();
            if (Expr.turn_to_string)
            {
                right = right.ToStringPlus();
                Expr.turn_to_string = false;
            }
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

        public override bool Run()
        {
            return left.Run().ToString() == right.Run().ToString();
        }
    }
    [Serializable]
    public class Ne : Rel
    {
        public Ne(Expr l)
        {
            left = l;
        }

        public override Rel Build(Lexer lex)
        {
            right = new Expr().BuildStart(lex);
            return this;
        }

        public override void Check()
        {
            left.Check();
            if (Expr.turn_to_string)
            {
                left = left.ToStringPlus();
                Expr.turn_to_string = false;
            }
            right.Check();
            if (Expr.turn_to_string)
            {
                right = right.ToStringPlus();
                Expr.turn_to_string = false;
            }
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

        public override bool Run()
        {
            return left.Run().ToString() != right.Run().ToString();
        }
    }
    [Serializable]
    public class Le : Rel
    {
        public Le(Expr l)
        {
            left = l;
        }

        public override Rel Build(Lexer lex)
        {
            right = new Expr().BuildStart(lex);
            return this;
        }

        public override void Check()
        {
            left.Check();
            if (Expr.turn_to_string)
            {
                left = left.ToStringPlus();
                Expr.turn_to_string = false;
            }
            right.Check();
            if (Expr.turn_to_string)
            {
                right = right.ToStringPlus();
                Expr.turn_to_string = false;
            }
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

        public override bool Run()
        {
            return (float)left.Run() <= (float)right.Run();
        }
    }
    [Serializable]
    public class Ge : Rel
    {
        public Ge(Expr l)
        {
            left = l;
        }

        public override Rel Build(Lexer lex)
        {
            right = new Expr().BuildStart(lex);
            return this;
        }

        public override void Check()
        {
            left.Check();
            if (Expr.turn_to_string)
            {
                left = left.ToStringPlus();
                Expr.turn_to_string = false;
            }
            right.Check();
            if (Expr.turn_to_string)
            {
                right = right.ToStringPlus();
                Expr.turn_to_string = false;
            }
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

        public override bool Run()
        {
            return (float)left.Run() >= (float)right.Run();
        }
    }
    [Serializable]
    public class More : Rel
    {
        public More(Expr l)
        {
            left = l;
        }

        public override Rel Build(Lexer lex)
        {
            right = new Expr().BuildStart(lex);
            return this;
        }

        public override void Check()
        {
            left.Check();
            if (Expr.turn_to_string)
            {
                left = left.ToStringPlus();
                Expr.turn_to_string = false;
            }
            right.Check();
            if (Expr.turn_to_string)
            {
                right = right.ToStringPlus();
                Expr.turn_to_string = false;
            }
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

        public override bool Run()
        {
            return (float)left.Run() > (float)right.Run();
        }
    }
    [Serializable]
    public class Less : Rel
    {
        public Less(Expr l)
        {
            left = l;
        }

        public override Rel Build(Lexer lex)
        {
            right = new Expr().BuildStart(lex);
            return this;
        }

        public override void Check()
        {
            left.Check();
            if (Expr.turn_to_string)
            {
                left = left.ToStringPlus();
                Expr.turn_to_string = false;
            }
            right.Check();
            if (Expr.turn_to_string)
            {
                right = right.ToStringPlus();
                Expr.turn_to_string = false;
            }
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

        public override bool Run()
        {
            return (float)left.Run() < (float)right.Run();
        }
    }
}
