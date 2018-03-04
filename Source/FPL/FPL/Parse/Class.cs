using System.Collections.Generic;
using FPL.LexicalAnalysis;
using FPL.Parse.Expression;
using FPL.Parse;

namespace FPL.Parse
{
    public class Class : Sentence
    {
        //List<Sentence> Sentences = new List<Sentence>();
        public List<Function> Functions = new List<Function>();
        public List<FunctionCall_s> FunctionCalls_s = new List<FunctionCall_s>();
        public List<FunctionCall_e> FunctionCalls_e = new List<FunctionCall_e>();
        public List<Object_s> Objects_s = new List<Object_s>();
        //public List<Object_e> Objects_e = new List<Object_e>();
        public List<Statement> Statement = new List<Statement>();
        public Function init_function;
        public string name;
        public int width;
        public int ID;

        public Class(int tag) : base(tag)
        {
            Lexer.Next();
            if (Lexer.Peek.tag == Tag.ID)
                name = ((Word)Lexer.Peek).lexeme;
            else Error("\"" + ((Word)Lexer.Peek).lexeme + "\"无效");
        }

        public override Sentence Build()
        {
            AddClass(name, this);
            NewScope();
            Parser.analyzing_class = this;
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACE) Error("应输入\"{\"");
            /*Sentences = */BuildClass();
            if (Lexer.Peek.tag != Tag.RBRACE) Error("应输入\"}\"");
            DestroyScope();
            if(GetFunction(name) == null)
            {
                Functions.Add(new Function(FuncType.Constructor, Tag.CONSTRUCTOR, name));
            }
            init_function = new Function(FuncType.InitFunction, Tag.INIT_FUNCTION, ".init");
            Functions.Add(init_function);
            Parser.analyzing_class = null;
            return this;
        }

        public void AddFunction(string name, Function f)
        {
            Functions.Add(f);
        }
        public Function GetFunction(string name)
        {
            foreach (var item in Functions)
            {
                if (item.name == name) return item;
            }
            return null;
        }

        public Statement GetStatement(string name)
        {
            foreach (var item in Statement)
            {
                if (item.name == name) return item;
            }
            return null;
        }
        public symbols.Type GetTypeByLocalName(string name)
        {
            foreach (var item in Statement)
            {
                if (item.name == name) return item.assign.type;
            }
            return null;
        }
        public Class GetClassByLocalName(string name)
        {
            foreach (var item in Functions)
            {
                if (item.name == name) return this;
            }
            return GetClass(GetTypeByLocalName(name).type_name);
        }

        public int GetWidth()
        {
            int w = 0;
            foreach (var item in Statement)
            {
                //if (item.assign.type.tag != Tag.BASIC) w += item.assign.type.width;
                /*else*/ w++;
            }
            return w;
        }
    }
}
