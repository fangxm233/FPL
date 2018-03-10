using System.Collections.Generic;
using FPL.LexicalAnalysis;

namespace FPL.symbols
{
    public class Type : Word
    {
        public static Dictionary<string, Type> Types = new Dictionary<string, Type>();

        public static readonly Type
            Int = new Type("int", LexicalAnalysis.Tag.BASIC, 4),
            Float = new Type("float", LexicalAnalysis.Tag.BASIC, 8),
            Char = new Type("char", LexicalAnalysis.Tag.BASIC, 1),
            Bool = new Type("bool", LexicalAnalysis.Tag.BASIC, 1),
            String = new Type("string", LexicalAnalysis.Tag.CLASS, 1),
            Void = new Type("void", LexicalAnalysis.Tag.BASIC, 0),
            UnKnown = new Type("UnKnown", LexicalAnalysis.Tag.BASIC, 0);

        public readonly string type_name;
        public int width;

        public Type(string s, int tag, int w) : base(s, tag)
        {
            width = w;
            type_name = s;
        }

        public override string ToString()
        {
            return type_name;
        }

        public static Type GetType(string name)
        {
            if (Types.ContainsKey(name)) return Types[name];
            switch (name)
            {
                case "int": return Int;
                case "float": return Float;
                case "char": return Char;
                case "bool": return Bool;
                case "string": return String;
                case "void": return Void;
            }

            return null;
        }

        public static void AddType(Type type)
        {
            Types.Add(type.type_name, type);
        }

        public static bool Numeric(Type p)
        {
            if (p == Char || p == Int || p == Float) return true;
            return false;
        }

        public static Type Max(Type p1, Type p2)
        {
            if (!Numeric(p1) || !Numeric(p2)) return null;
            if (p1 == Float || p2 == Float) return Float;
            if (p1 == Int || p2 == Int) return Int;
            return Char;
        }
    }
}