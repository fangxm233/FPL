using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    [Serializable]
    public class Function : Sentence
    {
        List<Sentence> stmts = new List<Sentence>();
        public string name;
        public symbols.Type return_type;
        
        public Function(int tag) : base(tag)
        {
            return_type = (symbols.Type)Lexer.Peek;
            Lexer.Next();
            if (Lexer.Peek.tag == Tag.ID)
                name = ((Word)Lexer.Peek).lexeme;
            else Error("\"" + ((Word)Lexer.Peek).lexeme + "\"无效");
            building_function = name;
        }

        public override Sentence Build()
        {
            AddFunction(name, this);
            NewScope();
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACE) Error("应输入\"{\"");
            stmts = Builds();
            if (Lexer.Peek.tag != Tag.RBRACE) Error("应输入\"}\"");
            DestroyScope();
            return this;
        }

        public override void Check()
        {
            foreach (Sentence item in stmts)
            {
                item.Check();
            }
        }

        public override void Run()
        {
            function_return = null;
            NewScope();
            foreach (Sentence item in stmts)
            {
                item.Run();
                if (is_return)
                {
                    is_return = false;
                    break;
                }
            }
            DestroyScope();
        }
    }
}
