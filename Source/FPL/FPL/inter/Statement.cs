using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;

namespace FPL.inter
{
    [Serializable]
    class Statement : Stmt
    {
        Assign assign;
        public Statement(int tag) : base(tag)
        {

        }
        public override Stmt Build(Lexer lex)
        {
            switch (((symbols.Type)Lexer.Peek).lexeme)
            {
                case "int":
                    {
                        lex.Next();
                        AddVar(((Word)Lexer.Peek).lexeme, symbols.Type.Int);
                        break;
                    }
                case "float":
                    {
                        lex.Next();
                        AddVar(((Word)Lexer.Peek).lexeme, symbols.Type.Float);
                        break;
                    }
                case "bool":
                    {
                        lex.Next();
                        AddVar(((Word)Lexer.Peek).lexeme, symbols.Type.Bool);
                        break;
                    }
                case "string":
                    {
                        lex.Next();
                        AddVar(((Word)Lexer.Peek).lexeme, symbols.Type.String);
                        break;
                    }
                default:
                    {
                        Error("上下文中不存在名称" + ((symbols.Type)Lexer.Peek).lexeme);
                        break;
                    }
            }
            if (Lexer.Peek.tag == Tag.ID)
            {
                assign = new Assign(Tag.ASSIGN);
                assign = (Assign)assign.Build(lex);
            }
            else
            {
                Error("应输入标识符");
            }
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            assign.Check();
        }

        public override void Run()
        {
            AddVar(assign.name, "define");
            assign.Run();
        }
    }
}
