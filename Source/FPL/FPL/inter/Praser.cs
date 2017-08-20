﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;
using System.Collections;

namespace FPL.inter
{
    public class Praser
    {
        Lexer lex;
        public static List<Hashtable> symbols_list = new List<Hashtable>()
        {
            new Hashtable()
        };
        List<Stmt> stmts;

        public Praser(Lexer lex)
        {
            this.lex = lex;
        }
        public List<Stmt> Compile()
        {
            stmts = new Stmt(1).Buildsstart(lex);
            Check();
            return stmts;
        }
        public void Check()
        {
            foreach (Stmt item in stmts)
            {
                item.Check();
            }
        }
    }

    public class CompileException : Exception
    {
         
    }
}
