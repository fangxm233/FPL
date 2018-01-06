﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FPL.lexer
{
    [Serializable]
    class Tag
    {
        //目前最大tag：296
        //语句的tag
        public const int
            ASSIGN = 280, STATEMENT = 287, FUNCTION = 291, FUNCTIONCALL = 293, QUOTE = 295;

        //标识符的tag
        public const int
            DO    = 259, ELSE     = 260, BREAK = 258, FALSE = 262, TRUE   = 274,
            WHILE = 275, CONTINUE = 290, IF    = 265, FOR   = 284, RETURN = 292,
            USING = 294, VOID     = 296;

        //表达式的tag
        public const int
            BASIC = 257, ID   = 264, MINUS    = 268, NUM    = 270, REAL = 272,
            STR   = 276, PLUS = 277, MULTIPLY = 278, DIVIDE = 279;

        //布尔型表达式的tag
        public const int
            AND = 256, EQ   = 261, GE   = 263, LE = 267, NE = 269, 
            OR  = 271, MORE = 288, LESS = 289;

        //符号的tag
        public const int
            SEMICOLON = 281, LBRACKETS = 282, RBRACKETS = 283, LBRACE = 285, RBRACE = 286;

        //其他
        public const int 
            INDEX = 266, TEMP = 273, EOF = 65535, EOL = 65534;

    }
}