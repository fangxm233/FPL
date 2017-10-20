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
    public class Parser
    {
        [NonSerialized]
        public static List<Hashtable> symbols_list = new List<Hashtable>();
        
        public static int var_count;
        [NonSerialized]
        public static List<Hashtable> var_id = new List<Hashtable>();
        [NonSerialized]
        public static object[] var_content;

        public static Dictionary<string, Function> functions = new Dictionary<string, Function>();
        [NonSerialized]
        public static bool is_runtime;

        List<Stmt> stmts;

        public void Compile()
        {
            stmts = new Stmt(1).Buildsstart();
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
            if (!functions.ContainsKey("Main")) Error("未找到主函数");
            is_runtime = true;
            var_content = new object[var_count];
            functions["Main"].Run();
        }

        public static void Error(string s)
        {
            Console.WriteLine(s);
            throw new RunTimeException();
        }
    }

    [Serializable]
    public class CompileException : Exception { }
    [Serializable]
    public class RunTimeException : Exception { }
}
