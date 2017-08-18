using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    public class Stmt : Node
    {
        public int tag;
        public Stmt(int tag)
        {
            this.tag = tag;
        }
        public virtual List<Stmt> Builds(Lexer lex)//这是建立一个语句块
        {
            List<Stmt> stmts = new List<Stmt>();
            while (true)
            {
                lex.Scan();
                switch (Lexer.Peek.tag)
                {
                    case Tag.RBRACE:
                        {
                            return stmts;
                        }
                    case Tag.BASIC:
                        {
                            stmts.Add(new Statement(Tag.BASIC));
                            stmts[stmts.Count - 1].Build(lex);
                            break;
                        }
                    case Tag.IF:
                        {
                            stmts.Add(new If(Tag.IF));
                            stmts[stmts.Count - 1].Build(lex);
                            break;
                        }
                    case Tag.FOR:
                        {
                            stmts.Add(new For(Tag.FOR));
                            stmts[stmts.Count - 1].Build(lex);
                            break;
                        }
                    case Tag.DO:
                        {
                            stmts.Add(new Do(Tag.DO));
                            stmts[stmts.Count - 1].Build(lex);
                            break;
                        }
                    case Tag.WHILE:
                        {
                            stmts.Add(new While(Tag.WHILE));
                            stmts[stmts.Count - 1].Build(lex);
                            break;
                        }
                    case Tag.BREAK:
                        {
                            stmts.Add(new Break(Tag.BREAK));
                            stmts[stmts.Count - 1].Build(lex);
                            break;
                        }
                    case Tag.ID:
                        {
                            stmts.Add(new Assign(Tag.ASSIGN));
                            stmts[stmts.Count - 1].Build(lex);
                            break;
                        }
                    default:
                        {
                            Console.WriteLine(Lexer.Peek.tag);
                            Error("语句错误或大括号不匹配");
                            return stmts;
                        }
                }
            }
        }

        public virtual List<Stmt> Buildsstart(Lexer lex)//这是建立一个语句块
        {
            List<Stmt> stmts = new List<Stmt>();
            while (true)
            {
                lex.Scan();
                switch (Lexer.Peek.tag)
                {
                    case Tag.EOF:
                        {
                            return stmts;
                        }
                    case Tag.RBRACE:
                        {
                            Error("意外的字符\"}\"");
                            break;
                        }
                    case Tag.BASIC:
                        {
                            stmts.Add(new Statement(Tag.BASIC));
                            stmts[stmts.Count - 1].Build(lex);
                            break;
                        }
                    case Tag.IF:
                        {
                            stmts.Add(new If(Tag.IF));
                            stmts[stmts.Count - 1].Build(lex);
                            break;
                        }
                    case Tag.FOR:
                        {
                            stmts.Add(new For(Tag.FOR));
                            stmts[stmts.Count - 1].Build(lex);
                            break;
                        }
                    case Tag.DO:
                        {
                            stmts.Add(new Do(Tag.DO));
                            stmts[stmts.Count - 1].Build(lex);
                            break;
                        }
                    case Tag.WHILE:
                        {
                            stmts.Add(new While(Tag.WHILE));
                            stmts[stmts.Count - 1].Build(lex);
                            break;
                        }
                    case Tag.BREAK:
                        {
                            stmts.Add(new Break(Tag.BREAK));
                            stmts[stmts.Count - 1].Build(lex);
                            break;
                        }
                    case Tag.ID:
                        {
                            stmts.Add(new Assign(Tag.ASSIGN));
                            stmts[stmts.Count - 1].Build(lex);
                            break;
                        }
                    default:
                        {
                            Error("语句错误或大括号不匹配");
                            return stmts;
                        }
                }
            }
        }

        public virtual Stmt Build(Lexer lex)//这是建立某一个语句
        {
            return this;
        }
        public virtual void Check() { }
        public virtual void Run() { }
    }
}
