using System.Collections.Generic;
using System.Data;

namespace FPL.DataStorager
{
    internal class Tag
    {
        //目前最大tag：320
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
            INIT_FUNCTION = 303,
            OPERATORFUNC = 320;

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
            STATIC = 304,
            OPERATOR = 319;

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


        public Tag(int i)
        {
            TagDetail = i;
        }

        public static explicit operator Tag(int i)
        {
            return new Tag(i);
        }
    }
}