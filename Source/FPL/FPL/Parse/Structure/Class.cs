using System.Collections.Generic;
using System.Linq;
using FPL.DataStorager;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Sentences;
using FPL.Parse.Sentences.ProcessControl;

namespace FPL.Parse.Structure
{
    public class Class : Sentence
    {
        public List<FunctionCall_e> FunctionCalls_e = new List<FunctionCall_e>();
        public List<FunctionCall_s> FunctionCalls_s = new List<FunctionCall_s>();
        public List<Function> Functions = new List<Function>();
        public int ID;
        public Function InitFunction;
        public string Name;
        public List<Object_s> Objects_s = new List<Object_s>();
        public List<Statement> Statement = new List<Statement>();
        public int Width;

        public Class(int tag) : base(tag)
        {
            Lexer.Next();
            if (Lexer.NextToken.tag == Tag.ID)
                Name = Lexer.NextToken.ToString();
            else Error(LogContent.SthUseless, Lexer.NextToken);
        }

        public override Sentence Build()
        {
            AddClass(Name, this);
            NewScope();
            Parser.AnalyzingClass = this;
            Match("{");
            BuildClass();
            Match("}", false);
            DestroyScope();
            if (ContainsFunction(Name, out bool h) == false && h == false)
                Functions.Add(new Function(FuncType.Constructor, Tag.CONSTRUCTOR, Name));
            InitFunction = new Function(FuncType.InitFunction, Tag.INIT_FUNCTION, ".init");
            Functions.Add(InitFunction);
            Parser.AnalyzingClass = null;
            return this;
        }

        public void AddFunction(string name, Function f)
        {
            Functions.Add(f);
        }

        public Function GetFunction(Node n, string name, params Parameter[] parameters)
        {
            bool hasSameName = false;
            foreach (Function item in Functions)
                if (item.Name == name)
                {
                    hasSameName = true;
                    if (parameters.Length != item.Parameters.Count) continue;
                    if (!parameters.Where((t, i) => t != item.Parameters[i]).Any())
                        return item;
                }

            Error(n, hasSameName ? LogContent.NotExistingMatchOverload : LogContent.NotExistingDefinitionInType, Name,
                name);
            return null;
        }

        public Function GetFunction(Node n, string name, List<Parameter> parameters)
        {
            bool hasSameName = false;
            foreach (Function item in Functions)
                if (item.Name == name)
                {
                    hasSameName = true;
                    if (parameters.Count != item.Parameters.Count) continue;
                    if (!parameters.Where((t, i) => t != item.Parameters[i]).Any())
                        return item;
                }

            Error(n, hasSameName ? LogContent.NotExistingMatchOverload : LogContent.NotExistingDefinitionInType, Name,
                name);
            return null;
        }

        public bool ContainsFunction(string name, out bool hasSameName, params Parameter[] parameters)
        {
            hasSameName = false;
            foreach (Function item in Functions)
                if (item.Name == name)
                {
                    hasSameName = true;
                    if (parameters.Length != item.Parameters.Count) continue;
                    if (!parameters.Where((t, i) => t != item.Parameters[i]).Any())
                        return true;
                }

            return false;
        }

        public bool ContainsFunction(string name, out bool hasSameName, List<Parameter> parameters)
        {
            hasSameName = false;
            foreach (Function item in Functions)
                if (item.Name == name)
                {
                    hasSameName = true;
                    if (parameters.Count != item.Parameters.Count) continue;
                    if (!parameters.Where((t, i) => t != item.Parameters[i]).Any())
                        return true;
                }

            return false;
        }

        public bool ContainsFunction(string name, params Parameter[] parameters)
        {
            foreach (Function item in Functions)
                if (item.Name == name)
                {
                    if (parameters.Length != item.Parameters.Count) continue;
                    if (!parameters.Where((t, i) => t != item.Parameters[i]).Any())
                        return true;
                }

            return false;
        }

        public bool ContainsFunction(string name, List<Parameter> parameters)
        {
            foreach (Function item in Functions)
                if (item.Name == name)
                {
                    if (parameters.Count != item.Parameters.Count) continue;
                    if (!parameters.Where((t, i) => t != item.Parameters[i]).Any())
                        return true;
                }

            return false;
        }

        public Statement GetStatement(string name)
        {
            foreach (Statement item in Statement)
                if (item.Name == name)
                    return item;
            return null;
        }

        public Type GetTypeByLocalName(string name)
        {
            foreach (Statement item in Statement)
                if (item.Name == name)
                    return item.Assign.Type;
            return null;
        }

        public Class GetClassByLocalName(string name)
        {
            foreach (Function item in Functions)
                if (item.Name == name)
                    return this;
            return GetClass(GetTypeByLocalName(name).type_name);
        }

        public int GetWidth()
        {
            return Statement.Count;
        }
    }
}