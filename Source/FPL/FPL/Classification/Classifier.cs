using System.Collections.Generic;
using FPL.DataStorager;

namespace FPL.Classification
{
    public enum ClassificateMethod
    {
        ExprEnd,
        ExprType,
        ReloadOpeNeedParmCount,
        VarType,

    }

    public class Classifier
    {
        private static readonly Dictionary<int, int> ExprTagTable = new Dictionary<int, int>();

        private static void FillTable()
        {
            ExprTagTable.Add(Tag.LSQUBRACKETS, Tag.FACTOR);
            ExprTagTable.Add(Tag.RSQUBRACKETS, Tag.FACTOR);
            ExprTagTable.Add(Tag.LBRACKETS, Tag.FACTOR);
            ExprTagTable.Add(Tag.RBRACKETS, Tag.FACTOR);
            ExprTagTable.Add(Tag.DOT, Tag.BINATY);
            ExprTagTable.Add(Tag.INCREASE, Tag.UNARY);
            ExprTagTable.Add(Tag.DECLINE, Tag.UNARY);
            ExprTagTable.Add(Tag.NOT, Tag.UNARY);
            ExprTagTable.Add(Tag.DIVIDE, Tag.BINATY);
            ExprTagTable.Add(Tag.MULTIPLY, Tag.BINATY);
            ExprTagTable.Add(Tag.MODULO, Tag.UNARY);
            ExprTagTable.Add(Tag.PLUS, Tag.BINATY);
            ExprTagTable.Add(Tag.MINUS, Tag.BINATY);
            ExprTagTable.Add(Tag.NUM, Tag.FACTOR);
            ExprTagTable.Add(Tag.REAL, Tag.FACTOR);
            ExprTagTable.Add(Tag.TRUE, Tag.FACTOR);
            ExprTagTable.Add(Tag.FALSE, Tag.FACTOR);
            ExprTagTable.Add(Tag.STR, Tag.FACTOR);
            ExprTagTable.Add(Tag.NEW, Tag.FACTOR);
            ExprTagTable.Add(Tag.ID, Tag.FACTOR);

            ExprTagTable.Add(Tag.MORE, Tag.BOOL);
            ExprTagTable.Add(Tag.LESS, Tag.BOOL);
            ExprTagTable.Add(Tag.LE, Tag.BOOL);
            ExprTagTable.Add(Tag.GE, Tag.BOOL);
            ExprTagTable.Add(Tag.EQ, Tag.BOOL);
            ExprTagTable.Add(Tag.NE, Tag.BOOL);
            ExprTagTable.Add(Tag.AND, Tag.BOOL);
            ExprTagTable.Add(Tag.OR, Tag.BOOL);
        }

        public static int ClassificateIn(ClassificateMethod method, int tag)
        {
            if (ExprTagTable.Count == 0)
                FillTable();

            switch (method)
            {
                case ClassificateMethod.ExprEnd:
                    switch (tag)
                    {
                        case Tag.SEMICOLON:
                        case Tag.ASSIGN:
                        case Tag.RBRACE:
                        case Tag.COMMA:
                            return 1;
                    }
                    break;
                case ClassificateMethod.ExprType:
                    if (ExprTagTable.ContainsKey(tag)) return ExprTagTable[tag];
                    break;
            }

            return -1;
        }

        public static int ClassificateIn(ClassificateMethod method, string s)
        {
            if (ExprTagTable.Count == 0)
                FillTable();

            switch (method)
            {
                case ClassificateMethod.ReloadOpeNeedParmCount:
                    switch (s)
                    {
                        case "==":
                        case "!=":
                        case ">=":
                        case "<=":
                        case "+":
                        case "-":
                        case "*":
                        case "/":
                            return 2;
                        case "!":
                            return 1;
                        default:
                            return -1;
                    }
                case ClassificateMethod.VarType:
                    switch (s)
                    {
                        case "int":
                        case "char":
                        case "float":
                        case "bool":
                        case "void":
                            return Tag.BASIC;
                        default:
                            return Tag.ID;
                    }
            }

            return -1;
        }
    }
}
