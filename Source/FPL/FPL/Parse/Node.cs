using System;
using System.Collections;
using System.Linq;
using FPL.DataStorager;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Expression;
using FPL.Parse.Sentences;
using FPL.Parse.Structure;

namespace FPL.Parse
{
    public class Node
    {
        public readonly string File;
        public readonly int Line;

        protected Node()
        {
            Line = Lexer.Line;
            File = Lexer.NowFileName;
        }

        #region Error
        public static void ErrorSta(LogContent c, params object[] parm)
        {
            Debugger.LogError("文件 " + Lexer.NowFileName + ": " + "行 " + Lexer.Line + ": ", c, parm);
            throw new CompileException();
        }
        public void Error(LogContent c, params object[] parm)
        {
            Debugger.LogError("文件 " + Lexer.NowFileName + ": " + "行 " + Line + ": ", c, parm);
            throw new CompileException();
        }
        public static void Error(Token c, LogContent l, params object[] parm)
        {
            Debugger.LogError("文件 " + c.File + ": " + "行 " + c.Line + ": ", l, parm);
            throw new CompileException();
        }
        public static void Error(Expr c, LogContent l, params object[] parm)
        {
            Debugger.LogError("文件 " + c.File + ": " + "行 " + c.Line + ": ", l, parm);
            throw new CompileException();
        }
        public static void Error(Node c, LogContent l, params object[] parm)
        {
            Debugger.LogError("文件 " + c.File + ": " + "行 " + c.Line + ": ", l, parm);
            throw new CompileException();
        }

        #endregion

        public void AddVar(Statement statement, VarType varType)
        {
            if (varType == VarType.Static)
            {
                if (Parser.SymbolsList.Last()[".s" + statement.Name] != null)
                    Error(LogContent.ExistingDefinition, statement.Name);
                Parser.SymbolsList[Parser.SymbolsList.Count - 1].Add(".s" + statement.Name, statement);
            }
            else
            {
                if (Parser.SymbolsList[Parser.SymbolsList.Count - 1][statement.Name] != null)
                    Error(LogContent.ExistingDefinition, statement.Name);
                Parser.SymbolsList[Parser.SymbolsList.Count - 1].Add(statement.Name, statement);
            }
        }
        public Statement GetStatement(string name, VarType varType)
        {
            for (int i = Parser.SymbolsList.Count - 1; i > -1; i--)
                if (Parser.SymbolsList[i][name] != null)
                    return (Statement) Parser.SymbolsList[i][name];

            if (varType != VarType.Unknown)
                Error(LogContent.NotExistingDefinition, name);

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
            if (Parser.Classes.ContainsKey(name)) Error(LogContent.ExistingDefinition, name);

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

            Error(LogContent.NotExistingDefinition, name);
            return Parser.Classes[name];
        }

        public bool Match(string s, bool goNext = true, bool throwError = true)
        {
            if (goNext) Lexer.Next();
            if (Lexer.NextToken.ToString() != s)
            {
                if (throwError)
                {
                    Error(LogContent.SthExpect, s);
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}