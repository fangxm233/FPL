using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.Encoding;

namespace FPL.inter
{
    [Serializable]
    public class Function : Sentence
    {
        List<Sentence> Sentences = new List<Sentence>();
        public string name;
        public symbols.Type return_type;
        public List<Statement> statements = new List<Statement>();
        public List<Var> vars = new List<Var>();
        public List<string> stmts = new List<string>();
        public int head_line;
        public int id;

        public Function(int tag) : base(tag)
        {
            return_type = (symbols.Type)Lexer.Peek;
            Lexer.Next();
            if (Lexer.Peek.tag == Tag.ID)
                name = ((Word)Lexer.Peek).lexeme;
            else Error("\"" + ((Word)Lexer.Peek).lexeme + "\"无效");
            //building_function = name;
        }

        public override Sentence Build()
        {
            AddFunction(name, this);
            NewScope();
            Parser.analyzing_function = this;
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            Lexer.Next();
            while (true)
            {
                if (Lexer.Peek.tag == Tag.RBRACKETS) break;
                statements.Add((Statement)new Statement(Tag.STATEMENT).Build());
                if (Lexer.Peek.tag == Tag.COMMA)
                    Lexer.Next();
                else
                    break;
            }
            if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACE) Error("应输入\"{\"");
            Sentences = Builds();
            if (Lexer.Peek.tag != Tag.RBRACE) Error("应输入\"}\"");
            DestroyScope();
            return this;
        }

        public override void Check()
        {
            if (Sentences.Count == 0)
            {
                if (return_type != symbols.Type.Void) Error(this, "不是所有路径都有返回值");
                return;
            }
            if (Sentences[Sentences.Count - 1].tag != Tag.RETURN)//检查函数返回
            {
                if (return_type != symbols.Type.Void) Error(this, "不是所有路径都有返回值");
                Sentences.Add(new Return(Tag.RETURN, name));
            }
            foreach (var item in vars)//给所有变量读取分配所指变量
            {
                item.id = GetID(item);
            }
            foreach (var item in Sentences)
            {
                item.Check();
            }
        }

        public override void Code()
        {
            head_line = Encoder.line + 1;
            for (int i = 0; i < stmts.Count - statements.Count; i++)
            {
                Encoder.Write(InstructionsType.loadi);
            }
            foreach (var item in Sentences)
            {
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

        int GetID(Var var)
        {
            for (int i = 0; i < stmts.Count; i++)
            {
                if (stmts[i] == var.name) return stmts.Count - i - 1;
            }
            Error(var, "未找到变量名\"" + var.name + "\"");
            return 0;
        }
    }
}
