using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Sentences;
using FPL.Parse.Sentences.ProcessControl;
using FPL.symbols;

namespace FPL.Parse.Structure
{
    public class Function : Sentence
    {
        public Class Class;
        public FuncType FuncType;
        public int HeadLine;
        public int ID;
        public string Name;
        public List<Object_s> objects_s = new List<Object_s>();
        public List<Statement> ParStatements = new List<Statement>();
        public Type ReturnType;
        private List<Sentence> Sentences = new List<Sentence>();
        public List<Statement> Statements = new List<Statement>();
        public string TypeName;

        public Function(FuncType type, int tag) : base(tag)
        {
            FuncType = type;
            TypeName = ((Word) Lexer.NextToken).Lexeme;
            //return_type = (symbols.Type)Lexer.Peek;
            Lexer.Next();
            if (Lexer.NextToken.tag == Tag.ID)
                Name = ((Word) Lexer.NextToken).Lexeme;
            else Error("\"" + ((Word) Lexer.NextToken).Lexeme + "\"无效");
        }

        public Function(FuncType type, int tag, string name) : base(tag)
        {
            FuncType = type;
            this.Name = name;
        }

        public Function(FuncType type, Type return_type, int tag) : base(tag)
        {
            FuncType = type;
            this.ReturnType = return_type;
            if (Lexer.NextToken.tag == Tag.ID)
                Name = ((Word) Lexer.NextToken).Lexeme;
            else Error("\"" + ((Word) Lexer.NextToken).Lexeme + "\"无效");
        }

        public Function(FuncType type, Type return_type, string name, int tag) : base(tag)
        {
            FuncType = type;
            Class = GetClass(name);
            this.ReturnType = return_type;
            this.Name = name;
        }

        public override Sentence Build()
        {
            Class = Parser.AnalyzingClass;
            Parser.AnalyzingClass.AddFunction(Name, this);
            NewScope();
            Parser.AnalyzingFunction = this;
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            Lexer.Next();
            while (true)
            {
                if (Lexer.NextToken.tag == Tag.RBRACKETS) break;
                ParStatements.Add((Statement) new Statement(VarType.Arg, Tag.STATEMENT).Build());
                if (Lexer.NextToken.tag == Tag.COMMA)
                    Lexer.Next();
                else
                    break;
            }

            if (Lexer.NextToken.tag != Tag.RBRACKETS) Error("应输入\")\"");
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.LBRACE) Error("应输入\"{\"");
            Sentences = BuildMethod();
            if (Lexer.NextToken.tag != Tag.RBRACE) Error("应输入\"}\"");
            DestroyScope();
            Parser.AnalyzingFunction = null;
            return this;
        }

        public override void Check()
        {
            if (tag == Tag.INIT_FUNCTION) return;
            if (tag == Tag.CONSTRUCTOR && Sentences.Count == 0)
            {
                ReturnType = Type.Void;
                AddSentence(new Return(Tag.RETURN, Name));
                return;
            }

            Parser.AnalyzingFunction = this;
            if (Sentences.Count == 0)
                if (ReturnType != Type.Void)
                {
                    Error(this, "不是所有路径都有返回值");
                }
                else
                {
                    Sentences.Add(new Return(Tag.RETURN, Name));
                    ((Return) Sentences[Sentences.Count - 1]).Class = Class;
                }

            if (Sentences[Sentences.Count - 1].tag != Tag.RETURN) //检查函数返回
            {
                if (ReturnType != Type.Void) Error(this, "不是所有路径都有返回值");
                Sentences.Add(new Return(Tag.RETURN, Name));
                ((Return) Sentences[Sentences.Count - 1]).Class = Class;
            }

            if (ParStatements.Count != 0 && Name == "Main") Error(this, "入口函数不允许有参数");
            foreach (Statement item in ParStatements) item.Check();
            foreach (Sentence item in Sentences) item.Check();
            Parser.AnalyzingFunction = null;
        }

        public override void Code()
        {
            if (tag == Tag.INIT_FUNCTION)
            {
                ReturnType = Type.Void;
                AddSentence(new Return(Tag.RETURN, Name));
            }

            HeadLine = Encoder.Line + 1;
            foreach (Object_s item in objects_s) item.IsHead = false;
            for (int i = 1; i < ParStatements.Count + 1; i++) ParStatements[i - 1].ID = i;
            for (int i = ParStatements.Count + 2; i < Statements.Count; i++) Statements[i].ID = i;
            for (int i = 0; i < Statements.Count - ParStatements.Count; i++) Encoder.Write(InstructionType.pushval);
            foreach (Sentence item in Sentences)
            {
                if (item.tag == Tag.RETURN && tag == Tag.INIT_FUNCTION || tag == Tag.CONSTRUCTOR)
                {
                    Encoder.Write(InstructionType.ret);
                    continue;
                }

                item.Code();
            }

            //if (name == "Main") Encoder.Write(InstructionsType.endP);
        }

        public override void CodeSecond()
        {
            foreach (Sentence item in Sentences) item.CodeSecond();
        }

        public Statement GetStatement(string name)
        {
            foreach (Statement item in Statements)
                if (item.Name == name)
                    return item;
            return null;
        }

        public Type GetTypeByLocalName(string name)
        {
            foreach (Statement item in Statements)
                if (item.Name == name)
                    return item.Assign.Type;
            return null;
        }

        public void AddSentence(Sentence sentence)
        {
            Sentences.Add(sentence);
        }
    }
}