using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FPL.lexer
{
    [Serializable]
    class Tag
    {
        public const int
            AND = 256, BASIC = 257, BREAK = 258, DO = 259, ELSE = 260,
            EQ = 261, FALSE = 262, GE = 263, ID = 264, IF = 265,
            INDEX = 266, LE = 267, MINUS = 268, NE = 269, NUM = 270,
            OR = 271, REAL = 272, TEMP = 273, TRUE = 274, WHILE = 275,
            STR = 276, PLUS = 277, MULTIPLY = 278, DIVIDE = 279,
            ASSIGN = 280, SEMICOLON = 281, LPARENTHESIS = 282, RPARENTHESIS = 283, FOR = 284,
            LBRACE = 285, RBRACE = 286, STATEMENT = 287, MORE = 288, LESS = 289,
            CONTINUE = 290, EOF = 65535;
    }
}