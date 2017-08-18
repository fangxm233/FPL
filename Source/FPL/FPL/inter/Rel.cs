using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;

namespace FPL.inter
{
    public class Rel : Node
    {
        Token op;
        public Expr left;
        public Expr right;
        public Token content;

        public virtual Rel Build(Lexer lex)
        {
            left = new Expr().BuildStart(lex);
            switch (Lexer.Peek.tag)
            {
                case Tag.AND:
                    {
                        And a = new And(left);
                        a.Build(lex);
                        return a;
                    }
                case Tag.OR:
                    {
                        Or a = new Or(left);
                        a.Build(lex);
                        return a;
                    }
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
    }

    public class And : Rel
    {
        public And(Expr l)
        {
            left = l;
        }

        public override Rel Build(Lexer lex)
        {
            right = new Expr().BuildStart(lex);
            return this;
        }
    }
    public class Or : Rel
    {
        public Or(Expr l)
        {
            left = l;
        }

        public override Rel Build(Lexer lex)
        {
            right = new Expr().BuildStart(lex);
            return this;
        }
    }
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
    }
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
    }
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
    }
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
    }
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
    }
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
    }
}
