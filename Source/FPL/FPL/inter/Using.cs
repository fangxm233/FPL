﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    [Serializable]
    public class Using : Stmt
    {
        public Using(int tag) : base(tag)
        {

        }

        public override Stmt Build(Lexer lex)
        {
            lex.Next();
            switch (((Word)Lexer.Peek).lexeme)
            {
                case "Console":
                    {
                        lex.AddQuote("cout");
                        lex.AddQuote("cin");
                        break;
                    }
                default:
                    {
                        Error("未知的引用");
                        break;
                    }
            }
            lex.Next();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }
    }
}