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
        public override Stmt Build()
        {
            switch (((symbols.Type)Lexer.Peek).lexeme)
            {
                case "int":
                    {
                        Lexer.Next();
                        assign = new Assign(Tag.ASSIGN, AddVar(((Word)Lexer.Peek).lexeme, symbols.Type.Int));
                        break;
                    }
                case "float":
                    {
                        Lexer.Next();
                        assign = new Assign(Tag.ASSIGN, AddVar(((Word)Lexer.Peek).lexeme, symbols.Type.Float));
                        break;
                    }
                case "bool":
                    {
                        Lexer.Next();
                        assign = new Assign(Tag.ASSIGN, AddVar(((Word)Lexer.Peek).lexeme, symbols.Type.Bool));
                        break;
                    }
                case "string":
                    {
                        Lexer.Next();
                        assign = new Assign(Tag.ASSIGN, AddVar(((Word)Lexer.Peek).lexeme, symbols.Type.String));
                        break;
                    }
                default:
                    {
                        Error("上下文中不存在名称" + ((symbols.Type)Lexer.Peek).lexeme);
                        break;
                    }
            }
            if (Lexer.Peek.tag != Tag.ID)
            {
                Error("应输入标识符");
            }
            assign = (Assign)assign.Build();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            assign.Check();
        }

        public override void Run()
        {
            //AddVar(assign.name, "define");
            assign.Run();
        }
    }
}
