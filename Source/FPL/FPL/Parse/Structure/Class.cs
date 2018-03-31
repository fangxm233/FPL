using System.Collections.Generic;
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
            if (GetFunction(Name) == null) Functions.Add(new Function(FuncType.Constructor, Tag.CONSTRUCTOR, Name));
            InitFunction = new Function(FuncType.InitFunction, Tag.INIT_FUNCTION, ".init");
            Functions.Add(InitFunction);
            Parser.AnalyzingClass = null;
            return this;
        }

        public void AddFunction(string name, Function f)
        {
            Functions.Add(f);
        }

        public Function GetFunction(string name)
        {
            foreach (Function item in Functions)
                if (item.Name == name)
                    return item;
            return null;
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