﻿using System;
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
        public void Error(Token c,string s)
        {
            Console.WriteLine("行 " + c.line + ": " + s);
            throw new CompileException();
        }
        public void Error(Expr c, string s)
        {
            Console.WriteLine("行 " + c.line + ": " + s);
            throw new CompileException();
        }
        public void Error(Node c, string s)
        {
            Console.WriteLine("行 " + c.line + ": " + s);
            throw new CompileException();
        }

        public void AddVar(string name, symbols.Type type)
        {
            if (Praser.symbols_list[Praser.symbols_list.Count - 1][name] != null) Error("已在此范围定义了名为\"" + name + "\"的变量");
            Praser.symbols_list[Praser.symbols_list.Count - 1].Add(name, type);
        }
        public void AddVar(string name, object value)
        {
            if (Praser.symbols_list[Praser.symbols_list.Count - 1][name] != null) Error("已在此范围定义了名为\"" + name + "\"的变量");
            Praser.symbols_list[Praser.symbols_list.Count - 1].Add(name, value);
        }

        public object GetName(string name)
        {
            for (int i = Praser.symbols_list.Count - 1; i > -1; i--)
            {
                if (Praser.symbols_list[i][name] != null)
                {
                    return Praser.symbols_list[i][name];
                }
            }
            Error("当前上下文中不存在名称\"" + name + "\"");
            return null;
        }

        public object GetVar(string name)
        {
            for (int i = Praser.symbols_list.Count - 1; i > -1; i--)
            {
                if (Praser.symbols_list[i][name] != null)
                {
                    return Praser.symbols_list[i][name];
                }
            }
            Error("变量\"" + name + "\"未定义");
            return null;
        }

        public void NewScope()
        {
            Praser.symbols_list.Add(new Hashtable());
        }

        public void DestroyScope()
        {
            Praser.symbols_list.RemoveAt(Praser.symbols_list.Count - 1);
        }
    }
}
