using FPL.Parse.Expression;
using System;
using System.Collections.Generic;
using FPL.Parse.Sentences;

namespace FPL.DataStorager
{
    public class Parameter : IEquatable<Parameter>
    {
        public Type Type;
        public string Name;
        public Statement Statement;
        public Expr Expr;

        public Parameter(Statement statement)
        {
            Statement = statement;
        }

        public Parameter(Expr expr)
        {
            Expr = expr;
        }

        public Parameter(Type type, string name)
        {
            Type = type;
            Name = name;
        }
        public void Check()
        {
            if (Statement != null)
            {
                Statement.Check();
                Type = Statement.Assign.Type;
                Name = Statement.Name;
            }

            if (Expr != null)
            {
                Expr.Check();
                Type = Expr.Type;
            }
        }

        public void Code()
        {
            if (Statement != null) Statement.Code();
            else Expr?.Code();
        }

        public int GetTokenLength()
        {
            return Expr.GetTokenLength();
        }

        #region operator

        public static bool operator ==(Parameter l, Type r)
        {
            return l.Type == r;
        }
        public static bool operator !=(Parameter l, Type r)
        {
            return !(l == r);
        }
        public static bool operator ==(Parameter l, Parameter r)
        {
            return l.Type == r.Type;
        }
        public static bool operator !=(Parameter l, Parameter r)
        {
            return !(l == r);
        }
        public static bool operator ==(Parameter l, Expr r)
        {
            return l.Type == r.Type;
        }
        public static bool operator !=(Parameter l, Expr r)
        {
            return !(l == r);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Parameter);
        }

        public bool Equals(Parameter other)
        {
            return EqualityComparer<Type>.Default.Equals(Type, other.Type);
        }

        public override int GetHashCode()
        {
            return 2049151605 + EqualityComparer<Type>.Default.GetHashCode(Type);
        }

        #endregion
    }
}
