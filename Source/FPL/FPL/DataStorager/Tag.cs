using System.Collections.Generic;
using System.Data;

namespace FPL.DataStorager
{
    public enum ClassificateMethod
    {
        ExprEnd,
        ExprType,

    }

    internal class Tag
    {
        //目前最大tag：318
        //语句的tag
        public const int
            SENTENCE = 316,
            ASSIGN = 280,
            STATEMENT = 287,
            FUNCTION = 291,
            FUNCTIONCALL = 293,
            QUOTE = 295,
            OBJECT = 301,
            CONSTRUCTOR = 302,
            INIT_FUNCTION = 303;

        //标识符的tag
        public const int
            IDENTIFIER = 315,
            DO = 259,
            ELSE = 260,
            BREAK = 258,
            FALSE = 262,
            TRUE = 274,
            WHILE = 275,
            CONTINUE = 290,
            IF = 265,
            FOR = 284,
            RETURN = 292,
            USING = 294,
            VOID = 296,
            CLASS = 298,
            NEW = 299,
            STATIC = 304;

        //表达式的tag
        public const int
            EXPR = 317,
            BOOL = 314,
            BASIC = 257,
            ID = 264,
            MINUS = 268,
            NUM = 270,
            REAL = 272,
            STR = 276,
            PLUS = 277,
            MULTIPLY = 278,
            DIVIDE = 279,
            UNARY = 310,
            BINATY = 311,
            FACTOR = 313;

        //布尔型表达式的tag
        public const int
            AND = 256,
            EQ = 261,
            GE = 263,
            LE = 267,
            NE = 269,
            OR = 271,
            MORE = 288,
            LESS = 289;

        //符号的tag
        public const int
            SYMBOL = 317,
            SEMICOLON = ';',
            LBRACKETS = '(',
            RBRACKETS = ')',
            LBRACE = '{',
            RBRACE = '}',
            COMMA = ',',
            DOT = ',',
            LSQUBRACKETS = '[',
            RSQUBRACKETS = ']',
            INCREASE = 307,
            DECLINE = 308,
            MODULO = '%',
            NOT = '!';

        //其他
        public const int
            INDEX = 266, TEMP = 273, EOF = 65535, EOL = 65534, EXPREND = 318;

        public int TagDetail { get; }

        private static readonly Dictionary<int, int> ExprTagTable = new Dictionary<int, int>();

        public Tag(int i)
        {
            TagDetail = i;

            if (ExprTagTable.Count == 0)
                FillTable();
        }

        private static void FillTable()
        {
            ExprTagTable.Add(LSQUBRACKETS, FACTOR);
            ExprTagTable.Add(RSQUBRACKETS, FACTOR);
            ExprTagTable.Add(LBRACKETS, FACTOR);
            ExprTagTable.Add(RBRACKETS, FACTOR);
            ExprTagTable.Add(DOT, BINATY);
            ExprTagTable.Add(INCREASE, UNARY);
            ExprTagTable.Add(DECLINE, UNARY);
            ExprTagTable.Add(NOT, UNARY);
            ExprTagTable.Add(DIVIDE, BINATY);
            ExprTagTable.Add(MULTIPLY, BINATY);
            ExprTagTable.Add(MODULO, UNARY);
            ExprTagTable.Add(PLUS, BINATY);
            ExprTagTable.Add(MINUS, BINATY);
            ExprTagTable.Add(NUM, FACTOR);
            ExprTagTable.Add(REAL, FACTOR);
            ExprTagTable.Add(TRUE, FACTOR);
            ExprTagTable.Add(FALSE, FACTOR);
            ExprTagTable.Add(STR, FACTOR);
            ExprTagTable.Add(NEW, FACTOR);
            ExprTagTable.Add(ID, FACTOR);

            ExprTagTable.Add(MORE, BOOL);
            ExprTagTable.Add(LESS, BOOL);
            ExprTagTable.Add(LE, BOOL);
            ExprTagTable.Add(GE, BOOL);
            ExprTagTable.Add(EQ, BOOL);
            ExprTagTable.Add(NE, BOOL);
            ExprTagTable.Add(AND, BOOL);
            ExprTagTable.Add(OR, BOOL);
        }

        public static explicit operator Tag(int i)
        {
            return new Tag(i);
        }

        public static int IsIncludedIn(ClassificateMethod method, int tag)
        {
            if (ExprTagTable.Count == 0)
                FillTable();
            
            switch (method)
            {
                case ClassificateMethod.ExprEnd:
                    switch (tag)
                    {
                        case SEMICOLON:
                        case ASSIGN:
                        case RBRACE:
                        case COMMA:
                            return 1;
                    }
                    break;
                case ClassificateMethod.ExprType:
                    if (ExprTagTable.ContainsKey(tag)) return ExprTagTable[tag];
                    break;
            }

            return -1;
        }
    }
}