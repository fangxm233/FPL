using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;
using System.Collections;

namespace FPL.inter
{
    [Serializable]
    public class Node
    {
        public readonly int line;
        protected Node()
        {
            line = Lexer.line;
        }
        public static void Error(string s)
        {
            Console.WriteLine("行 " + Lexer.line + ": " + s);
            throw new CompileException();
        }
        public static void Error(Token c,string s)
        {
            Console.WriteLine("行 " + c.line + ": " + s);
            throw new CompileException();
        }
        public static void Error(Expr c, string s)
        {
            Console.WriteLine("行 " + c.line + ": " + s);
            throw new CompileException();
        }
        public static void Error(Node c, string s)
        {
            Console.WriteLine("行 " + c.line + ": " + s);
            throw new CompileException();
        }

        public int AddVar(string name, symbols.Type type)
        {
            if (Parser.functions.ContainsKey(name)) Error("当前上下文中已经包含\"" + name + "\"的定义");
            if (Parser.symbols_list[Parser.symbols_list.Count - 1][name] != null) Error("当前上下文中已经包含\"" + name + "\"的定义");
            Parser.var_id[Parser.var_id.Count - 1].Add(name, Parser.var_count);
            Parser.symbols_list[Parser.symbols_list.Count - 1].Add(name, type);
            return Parser.var_count++;
        }
        public void AddVar(string name, object value)
        {
            if (Parser.symbols_list[Parser.symbols_list.Count - 1][name] != null) Error("当前上下文中已经包含\"" + name + "\"的定义");
            Parser.symbols_list[Parser.symbols_list.Count - 1].Add(name, value);
        }

        public object GetName(string name)
        {
            for (int i = Parser.symbols_list.Count - 1; i > -1; i--)
            {
                if (Parser.symbols_list[i][name] != null)
                {
                    return Parser.symbols_list[i][name];
                }
            }
            Error("当前上下文中不存在名称\"" + name + "\"");
            return null;
        }
        public int GetID(string name)
        {
            for (int i = Parser.var_id.Count - 1; i > -1; i--)
            {
                if (Parser.var_id[i][name] != null)
                {
                    return (int)Parser.var_id[i][name];
                }
            }
            Error("当前上下文中不存在变量\"" + name + "\"");
            return 0;
        }
        //public object GetVar(string name)
        //{
        //    for (int i = Parser.symbols_list.Count - 1; i > -1; i--)
        //    {
        //        if (Parser.symbols_list[i][name] != null)
        //        {
        //            return Parser.symbols_list[i][name];
        //        }
        //    }
        //    Error("变量\"" + name + "\"未定义");
        //    return null;
        //}
        public object GetVar(int id)
        {
            return Parser.var_content[id];
        }

        public void NewScope()
        {
            if (Parser.is_runtime) return;
            Parser.symbols_list.Add(new Hashtable());
            Parser.var_id.Add(new Hashtable());
        }
        public void DestroyScope()
        {
            if (Parser.is_runtime) return;
            Parser.symbols_list.RemoveAt(Parser.symbols_list.Count - 1);
            Parser.var_id.RemoveAt(Parser.var_id.Count - 1);
        }

        public void AddFunction(string name,Function f)
        {
            if (Parser.functions.ContainsKey(name)) Error("当前上下文中已经包含\"" + name + "\"的定义");
            Parser.functions.Add(name, f);
        }
        public Function GetFunction(string name)
        {
            if (!Parser.functions.ContainsKey(name)) Error("当前上下文中不存在名称\"" + name + "\"");
            return Parser.functions[name];
        }
    }
}
