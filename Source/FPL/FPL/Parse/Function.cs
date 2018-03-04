using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Expression;
using FPL.Parse;

namespace FPL.Parse
{
    public class Function : Sentence
    {
        List<Sentence> Sentences = new List<Sentence>();
        public string name;
        public symbols.Type return_type;
        public string type_name;
        public List<Statement> par_statements = new List<Statement>();
        public List<Statement> Statements = new List<Statement>();
        public List<Object_e> objects_e = new List<Object_e>();
        public List<Object_s> objects_s = new List<Object_s>();
        public int head_line;
        public int ID;
        public Class @class;
        public FuncType func_type;

        public Function(FuncType type, int tag) : base(tag)
        {
            func_type = type;
            type_name = ((Word)Lexer.Peek).lexeme;
            //return_type = (symbols.Type)Lexer.Peek;
            Lexer.Next();
            if (Lexer.Peek.tag == Tag.ID)
                name = ((Word)Lexer.Peek).lexeme;
            else Error("\"" + ((Word)Lexer.Peek).lexeme + "\"无效");
        }
        public Function(FuncType type, int tag, string name) : base(tag)
        {
            func_type = type;
            this.name = name;
        }

        public Function(FuncType type, symbols.Type return_type, int tag) : base(tag)
        {
            func_type = type;
            this.return_type = return_type;
            if (Lexer.Peek.tag == Tag.ID)
                name = ((Word)Lexer.Peek).lexeme;
            else Error("\"" + ((Word)Lexer.Peek).lexeme + "\"无效");
        }

        public Function(FuncType type, symbols.Type return_type, string name, int tag) : base(tag)
        {
            func_type = type;
            @class = GetClass(name);
            this.return_type = return_type;
            this.name = name;
        }

        public override Sentence Build()
        {
            @class = Parser.analyzing_class;
            Parser.analyzing_class.AddFunction(name, this);
            NewScope();
            Parser.analyzing_function = this;
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            Lexer.Next();
            while (true)
            {
                if (Lexer.Peek.tag == Tag.RBRACKETS) break;
                par_statements.Add((Statement)new Statement(VarType.Arg, Tag.STATEMENT).Build());
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
            Parse.Parser.analyzing_function = null;
            return this;
        }

        public override void Check()
        {
            if (tag == Tag.INIT_FUNCTION) return;
            if (tag == Tag.CONSTRUCTOR && Sentences.Count == 0)
            {
                return_type = symbols.Type.Void;
                AddSentence(new Return(Tag.RETURN, name));
                return;
            }
            Parse.Parser.analyzing_function = this;
            if (Sentences.Count == 0)
                if (return_type != symbols.Type.Void) Error(this, "不是所有路径都有返回值");
                else
                {
                    Sentences.Add(new Return(Tag.RETURN, name));
                    ((Return)Sentences[Sentences.Count - 1]).@class = @class;
                }
            if (Sentences[Sentences.Count - 1].tag != Tag.RETURN)//检查函数返回
            {
                if (return_type != symbols.Type.Void) Error(this, "不是所有路径都有返回值");
                Sentences.Add(new Return(Tag.RETURN, name));
                ((Return)Sentences[Sentences.Count - 1]).@class = @class;
            }
            if (par_statements.Count != 0 && name == "Main") Error(this, "入口函数不允许有参数");
            foreach (Statement item in par_statements)
            {
                item.Check();
            }
            foreach (Sentence item in Sentences)
            {
                item.Check();
            }
            if (func_type == FuncType.Static)
            {
                foreach (Object_e object_e in objects_e)
                {
                    if (object_e.varType == VarType.Field && object_e.is_head) Error(object_e, "对象引用对于非静态的字段、方法或属性\"" + name + "\"是必须的");
                }

                foreach (Object_s object_s in objects_s)
                {
                    if (object_s.varType == VarType.Field && object_s.is_head) Error(object_s, "对象引用对于非静态的字段、方法或属性\"" + name + "\"是必须的");
                }
            }
            Parse.Parser.analyzing_function = null;
        }

        public override void Code()
        {
            if (tag == Tag.INIT_FUNCTION)
            {
                return_type = symbols.Type.Void;
                AddSentence(new Return(Tag.RETURN, name));
            }
            head_line = Encoder.line + 1;
            foreach (var item in objects_e)
            {
                item.is_head = false;
            }
            foreach (var item in objects_s)
            {
                item.is_head = false;
            }
            for (int i = 1; i < par_statements.Count + 1; i++)
            {
                par_statements[i - 1].ID = i;
            }
            for (int i = par_statements.Count + 2; i < Statements.Count; i++)
            {
                Statements[i].ID = i;
            }
            for (int i = 0; i < Statements.Count - par_statements.Count; i++)
            {
                Encoder.Write(InstructionType.pushval);
            }
            foreach (var item in Sentences)
            {
                if(item.tag == Tag.RETURN && tag == Tag.INIT_FUNCTION || tag == Tag.CONSTRUCTOR)
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
            foreach (var item in Sentences)
            {
                item.CodeSecond();
            }
        }

        public int GetID(Object_e object_e)
        {
            for (int i = 0; i < Statements.Count; i++)
            {
                if (Statements[i].name == object_e.name) return Statements.Count - i - 1;
            }
            Error(object_e, "未找到变量名\"" + object_e.name + "\"");
            return 0;
        }

        public int GetID(Object_s object_s)
        {
            for (int i = 0; i < Statements.Count; i++)
            {
                if (Statements[i].name == object_s.name) return Statements.Count - i - 1;
            }
            Error(object_s, "未找到变量名\"" + object_s.name + "\"");
            return 0;
        }

        public Statement GetStatement(string name)
        {
            foreach (var item in Statements)
            {
                if (item.name == name) return item;
            }
            return null;
        }

        public symbols.Type GetTypeByLocalName(string name)
        {
            foreach (var item in Statements)
            {
                if (item.name == name) return item.assign.type;
            }
            return null;
        }

        public void AddSentence(Sentence sentence)
        {
            Sentences.Add(sentence);
        }
    }
}
