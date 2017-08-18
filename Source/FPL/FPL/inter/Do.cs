﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    public class Do : Stmt
    {
        Rel rel;
        List<Stmt> stmts;

        public Do(int tag) : base(tag)
        {

        }

        public override Stmt Build(Lexer lex)
        {
            NewScope();
            lex.Scan();
            if (Lexer.Peek.tag != Tag.LBRACE) Error("应输入\"{\"");
            stmts = base.Builds(lex);
            if (Lexer.Peek.tag != Tag.RBRACE) Error("应输入\"}\"");
            lex.Scan();
            if (Lexer.Peek.tag != Tag.WHILE) Error("应输入\"while\"");
            lex.Scan();
            if (Lexer.Peek.tag != Tag.LPARENTHESIS) Error("应输入\"(\"");
            rel = new Rel();
            rel = rel.Build(lex);
            if (Lexer.Peek.tag != Tag.RPARENTHESIS) Error("应输入\")\"");
            lex.Scan();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            DestroyScope();
            return this;
        }
    }
}