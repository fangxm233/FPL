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
        [NonSerialized]
        public static List<Hashtable> symbols_list = new List<Hashtable>()
        {
            new Hashtable()
        };
        public static Hashtable functions = new Hashtable();
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
            if (functions["Main"] == null) Error("未找到主函数");
            ((Function)functions["Main"]).Run();
        }

        public static void Error(string s)
        {
            Console.WriteLine(s);
            throw new RunTimeException();
        }
    }

    [Serializable]
    public class CompileException : Exception { }
    public class RunTimeException : Exception { }
}
