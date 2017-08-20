using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;
using System.Collections;

namespace FPL.inter
{
    [Serializable]
    public class Praser
    {
        [NonSerialized]
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
        public void Compile()
        {
            stmts = new Stmt(1).Buildsstart(lex);
            Check();
        }
        public void Check()
        {
            foreach (Stmt item in stmts)
            {
                item.Check();
            }
        }
        public void Interprete()
        {
            symbols_list = new List<Hashtable>()
            {
                new Hashtable()
            };
            foreach (Stmt item in stmts)
            {
                item.Run();
            }
        }
    }

    [Serializable]
    public class CompileException : Exception
    {
         
    }
}
