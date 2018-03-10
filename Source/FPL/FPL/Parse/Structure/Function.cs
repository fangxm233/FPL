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
        public Class @class;
        public FuncType func_type;
        public int head_line;
        public int ID;
        public string name;
        public List<Object_s> objects_s = new List<Object_s>();
        public List<Statement> par_statements = new List<Statement>();
        public Type return_type;
        private List<Sentence> Sentences = new List<Sentence>();
        public List<Statement> Statements = new List<Statement>();
        public string type_name;

        public Function(FuncType type, int tag) : base(tag)
        {
            func_type = type;
            type_name = ((Word) Lexer.Peek).lexeme;
            //return_type = (symbols.Type)Lexer.Peek;
            Lexer.Next();
            if (Lexer.Peek.tag == Tag.ID)
                name = ((Word) Lexer.Peek).lexeme;
            else Error("\"" + ((Word) Lexer.Peek).lexeme + "\"无效");
        }

        public Function(FuncType type, int tag, string name) : base(tag)
        {
            func_type = type;
            this.name = name;
        }

        public Function(FuncType type, Type return_type, int tag) : base(tag)
        {
            func_type = type;
            this.return_type = return_type;
            if (Lexer.Peek.tag == Tag.ID)
                name = ((Word) Lexer.Peek).lexeme;
            else Error("\"" + ((Word) Lexer.Peek).lexeme + "\"无效");
        }

        public Function(FuncType type, Type return_type, string name, int tag) : base(tag)
        {
            func_type = type;
            @class = GetClass(name);
            this.return_type = return_type;
            this.name = name;
        }

        public override Sentence Build()
        {
            @class = Parser.AnalyzingClass;
            Parser.AnalyzingClass.AddFunction(name, this);
            NewScope();
            Parser.AnalyzingFunction = this;
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            Lexer.Next();
            while (true)
            {
                if (Lexer.Peek.tag == Tag.RBRACKETS) break;
                par_statements.Add((Statement) new Statement(VarType.Arg, Tag.STATEMENT).Build());
                if (Lexer.Peek.tag == Tag.COMMA)
                    Lexer.Next();
                else
                    break;
            }

            if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACE) Error("应输入\"{\"");
            Sentences = BuildMethod();
            if (Lexer.Peek.tag != Tag.RBRACE) Error("应输入\"}\"");
            DestroyScope();
            Parser.AnalyzingFunction = null;
            return this;
        }

        public override void Check()
        {
            if (tag == Tag.INIT_FUNCTION) return;
            if (tag == Tag.CONSTRUCTOR && Sentences.Count == 0)
            {
                return_type = Type.Void;
                AddSentence(new Return(Tag.RETURN, name));
                return;
            }

            Parser.AnalyzingFunction = this;
            if (Sentences.Count == 0)
                if (return_type != Type.Void)
                {
                    Error(this, "不是所有路径都有返回值");
                }
                else
                {
                    Sentences.Add(new Return(Tag.RETURN, name));
                    ((Return) Sentences[Sentences.Count - 1]).@class = @class;
                }

            if (Sentences[Sentences.Count - 1].tag != Tag.RETURN) //检查函数返回
            {
                if (return_type != Type.Void) Error(this, "不是所有路径都有返回值");
                Sentences.Add(new Return(Tag.RETURN, name));
                ((Return) Sentences[Sentences.Count - 1]).@class = @class;
            }

            if (par_statements.Count != 0 && name == "Main") Error(this, "入口函数不允许有参数");
            foreach (Statement item in par_statements) item.Check();
            foreach (Sentence item in Sentences) item.Check();
            Parser.AnalyzingFunction = null;
        }

        public override void Code()
        {
            if (tag == Tag.INIT_FUNCTION)
            {
                return_type = Type.Void;
                AddSentence(new Return(Tag.RETURN, name));
            }

            head_line = Encoder.line + 1;
            foreach (Object_s item in objects_s) item.is_head = false;
            for (int i = 1; i < par_statements.Count + 1; i++) par_statements[i - 1].ID = i;
            for (int i = par_statements.Count + 2; i < Statements.Count; i++) Statements[i].ID = i;
            for (int i = 0; i < Statements.Count - par_statements.Count; i++) Encoder.Write(InstructionType.pushval);
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
                if (item.name == name)
                    return item;
            return null;
        }

        public Type GetTypeByLocalName(string name)
        {
            foreach (Statement item in Statements)
                if (item.name == name)
                    return item.assign.type;
            return null;
        }

        public void AddSentence(Sentence sentence)
        {
            Sentences.Add(sentence);
        }
    }
}