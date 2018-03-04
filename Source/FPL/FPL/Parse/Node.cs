using FPL.LexicalAnalysis;
using FPL.Parse.Expression;
using System;
using System.Collections;
using System.Linq;

namespace FPL.Parse
{
    public class Node
    {
        public readonly int line;
        public readonly string file;
        protected Node()
        {
            line = Lexer.line;
            file = Lexer.now_file_name;
        }
        public static void Error(string s)
        {
            Console.WriteLine("文件 " + Lexer.now_file_name + ": " + "行 " + Lexer.line + ": " + s);
            throw new CompileException();
        }
        public static void Error(Token c,string s)
        {
            Console.WriteLine("文件 " + c.file + ": " + "行 " + c.line + ": " + s);
            throw new CompileException();
        }
        public static void Error(Expr c, string s)
        {
            Console.WriteLine("文件 " + c.file + ": " + "行 " + c.line + ": " + s);
            throw new CompileException();
        }
        public static void Error(Node c, string s)
        {
            Console.WriteLine("文件 " + c.file + ": " + "行 " + c.line + ": " + s);
            throw new CompileException();
        }

        public void AddVar(Statement statement, VarType varType)
        {
            if (varType == VarType.Static)
            {
                if (Parser.symbols_list.Last()[".s" + statement.name] != null) Error(this, "当前上下文中已经包含\"" + statement.name + "\"的定义");
                Parser.symbols_list[Parser.symbols_list.Count - 1].Add(".s" + statement.name, statement);
            }
            else
            {
                if (Parser.symbols_list[Parser.symbols_list.Count - 1][statement.name] != null) Error(this, "当前上下文中已经包含\"" + statement.name + "\"的定义");
                Parser.symbols_list[Parser.symbols_list.Count - 1].Add(statement.name, statement);
            }
        }
        public Statement GetStatement(string name, VarType varType)
        {
            for (int i = Parser.symbols_list.Count - 1; i > -1; i--)
                if (Parser.symbols_list[i][name] != null) return (Statement)Parser.symbols_list[i][name];

            if (varType != VarType.Unknown)
                Error("当前上下文中不存在名称\"" + name + "\"");

            Statement statement = (Statement) Parser.symbols_list[0][".s" + name];
            return statement;
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

        public void AddClass(string name,Class @class)
        {
            if (Parser.classes.ContainsKey(name)) Error(this, "当前上下文中已经包含\"" + name + "\"的定义");
            Parser.classes.Add(name, @class);
        }
        public Class GetClass(string name)
        {
            if (Parser.classes.ContainsKey(name)) return Parser.classes[name];
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
            return Parser.classes[name];
        }
    }
}
