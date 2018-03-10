using System;
using System.Collections;
using System.Linq;
using FPL.LexicalAnalysis;
using FPL.Parse.Expression;
using FPL.Parse.Sentences;
using FPL.Parse.Structure;

namespace FPL.Parse
{
    public class Node
    {
        public readonly string file;
        public readonly int line;

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

        public static void Error(Token c, string s)
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
                if (Parser.SymbolsList.Last()[".s" + statement.name] != null)
                    Error(this, "当前上下文中已经包含\"" + statement.name + "\"的定义");
                Parser.SymbolsList[Parser.SymbolsList.Count - 1].Add(".s" + statement.name, statement);
            }
            else
            {
                if (Parser.SymbolsList[Parser.SymbolsList.Count - 1][statement.name] != null)
                    Error(this, "当前上下文中已经包含\"" + statement.name + "\"的定义");
                Parser.SymbolsList[Parser.SymbolsList.Count - 1].Add(statement.name, statement);
            }
        }

        public Statement GetStatement(string name, VarType varType)
        {
            for (int i = Parser.SymbolsList.Count - 1; i > -1; i--)
                if (Parser.SymbolsList[i][name] != null)
                    return (Statement) Parser.SymbolsList[i][name];

            if (varType != VarType.Unknown)
                Error("当前上下文中不存在名称\"" + name + "\"");

            Statement statement = (Statement) Parser.SymbolsList[0][".s" + name];
            return statement;
        }

        public void NewScope()
        {
            Parser.SymbolsList.Add(new Hashtable());
            Parser.VarId.Add(new Hashtable());
        }

        public void DestroyScope()
        {
            Parser.SymbolsList.RemoveAt(Parser.SymbolsList.Count - 1);
            Parser.VarId.RemoveAt(Parser.VarId.Count - 1);
        }

        public void AddClass(string name, Class @class)
        {
            if (Parser.Classes.ContainsKey(name)) Error(this, "当前上下文中已经包含\"" + name + "\"的定义");
            Parser.Classes.Add(name, @class);
        }

        public Class GetClass(string name)
        {
            if (Parser.Classes.ContainsKey(name)) return Parser.Classes[name];
            switch (name)
            {
                case "int": return Parser.Classes["Int"];
                case "float": return Parser.Classes["Float"];
                case "char": return Parser.Classes["Char"];
                case "bool": return Parser.Classes["Bool"];
                case "string": return Parser.Classes["String"];
                case "void": return Parser.Classes["Void"];
            }

            Error(this, "当前上下文中不存在名称\"" + name + "\"");
            return Parser.Classes[name];
        }
    }
}