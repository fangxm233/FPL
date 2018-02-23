using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;
using System.Collections;

namespace FPL.inter
{

    public class Node
    {
        public readonly int line;
        protected Node()
        {
            line = Lexer.line;
        }
        public static void Error(string s)
        {
            Console.WriteLine("文件 " + Lexer.now_file_name + ": " + "行 " + Lexer.line + ": " + s);
            throw new CompileException();
        }
        public static void Error(Token c,string s)
        {
            Console.WriteLine("文件 " + Lexer.now_file_name + ": " + "行 " + c.line + ": " + s);
            throw new CompileException();
        }
        public static void Error(Expr c, string s)
        {
            Console.WriteLine("文件 " + Lexer.now_file_name + ": " + "行 " + c.line + ": " + s);
            throw new CompileException();
        }
        public static void Error(Node c, string s)
        {
            Console.WriteLine("文件 " + Lexer.now_file_name + ": " + "行 " + c.line + ": " + s);
            throw new CompileException();
        }

        //public int AddVar(string name, symbols.Type type)
        //{
        //    if (Parser.functions.ContainsKey(name)) Error("当前上下文中已经包含\"" + name + "\"的定义");
        //    if (Parser.symbols_list[Parser.symbols_list.Count - 1][name] != null) Error("当前上下文中已经包含\"" + name + "\"的定义");
        //    Parser.var_id[Parser.var_id.Count - 1].Add(name, Parser.var_count);
        //    Parser.symbols_list[Parser.symbols_list.Count - 1].Add(name, type);
        //    return Parser.var_count++;
        //}
        public void AddVar(Statement statement)
        {
            if (Parser.symbols_list[Parser.symbols_list.Count - 1][statement.name] != null) Error(this, "当前上下文中已经包含\"" + statement.name + "\"的定义");
            Parser.symbols_list[Parser.symbols_list.Count - 1].Add(statement.name, statement);
        }
        public Statement GetStatement(string name)
        {
            for (int i = Parser.symbols_list.Count - 1; i > -1; i--)
            {
                if (Parser.symbols_list[i][name] != null)
                {
                    return (Statement)Parser.symbols_list[i][name];
                }
            }
            Error("当前上下文中不存在名称\"" + name + "\"");
            return null;
        }

        public symbols.Type GetTypeByObjName(string name)
        {
            for (int i = Parser.symbols_list.Count - 1; i > -1; i--)
            {
                if (Parser.symbols_list[i][name] != null)
                {
                    return (symbols.Type)Parser.symbols_list[i][name];
                }
            }
            Error(this, "当前上下文中不存在名称\"" + name + "\"");
            return null;
        }

        public void NewScope()
        {
            Parser.symbols_list.Add(new Hashtable());
            Parser.var_id.Add(new Hashtable());
        }
        public void DestroyScope()
        {
            Parser.symbols_list.RemoveAt(Parser.symbols_list.Count - 1);
            Parser.var_id.RemoveAt(Parser.var_id.Count - 1);
        }

        //public void AddFunction(string name,Function f)
        //{
        //    if (Parser.functions.ContainsKey(name)) Error("当前上下文中已经包含\"" + name + "\"的定义");
        //    Parser.functions.Add(name, f);
        //}
        //public Function GetFunction(string name)
        //{
        //    if (!Parser.functions.ContainsKey(name)) Error("当前上下文中不存在名称\"" + name + "\"");
        //    return Parser.functions[name];
        //}

        public void AddClass(string name,Class @class)
        {
            if (Parser.classes.ContainsKey(name)) Error(this, "当前上下文中已经包含\"" + name + "\"的定义");
            Parser.classes.Add(name, @class);
        }
        public Class GetClass(string name)
        {
            if (!Parser.classes.ContainsKey(name))
            {
                switch (name)
                {
                    case "int": return Parser.classes["Int"];
                    case "float": return Parser.classes["Float"];
                    case "char": return Parser.classes["Char"];
                    case "bool": return Parser.classes["Bool"];
                    case "string": return Parser.classes["String"];
                    case "void": return Parser.classes["Void"];
                }
                Error(this, "当前上下文中不存在名称\"" + name + "\"");
            }
            return Parser.classes[name];
        }
    }
}
