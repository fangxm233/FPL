using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{
    [Serializable]
    public class Sentence : Node
    {
        //运行时标记
        public static bool in_loop;
        public static bool is_break;
        public static bool is_continue;
        public static bool is_return;

        public static object function_return;

        public static string building_function;

        public int tag;
        public Sentence(int tag)
        {
            this.tag = tag;
        }
        public List<Sentence> Builds()//这是建立一个语句块
        {
            List<Sentence> sentences = new List<Sentence>();
            while (true)
            {
                Lexer.Next();
                switch (Lexer.Peek.tag)
                {
                    case Tag.RBRACE:
                        {
                            return sentences;
                        }
                    case Tag.BASIC:
                        {
                            sentences.Add(new Statement(Tag.BASIC).Build());
                            break;
                        }
                    case Tag.IF:
                        {
                            sentences.Add(new If(Tag.IF).Build());
                            break;
                        }
                    case Tag.FOR:
                        {
                            sentences.Add(new For(Tag.FOR).Build());
                            break;
                        }
                    case Tag.DO:
                        {
                            sentences.Add(new Do(Tag.DO).Build());
                            break;
                        }
                    case Tag.WHILE:
                        {
                            sentences.Add(new While(Tag.WHILE).Build());
                            break;
                        }
                    case Tag.BREAK:
                        {
                            sentences.Add(new Break(Tag.BREAK).Build());
                            break;
                        }
                    case Tag.CONTINUE:
                        {
                            sentences.Add(new Continue(Tag.CONTINUE).Build());
                            break;
                        }
                    case Tag.RETURN:
                        {
                            sentences.Add(new Return(Tag.RETURN).Build());
                            break;
                        }
                    case Tag.ID:
                        {
                            Lexer.Next();
                            if(Lexer.Peek.tag == Tag.LBRACKETS)
                            {
                                Lexer.Back();
                                sentences.Add(new FunctionCall_s(Tag.FUNCTIONCALL).Build());
                                break;
                            }
                            if (Lexer.Peek.tag == Tag.ASSIGN)
                            {
                                Lexer.Back();
                                sentences.Add(new Assign(Tag.ASSIGN).Build());
                                break;
                            }
                            Error("语法错误");
                            break;
                        }
                    case Tag.QUOTE:
                        {
                            sentences.Add(new Quote(Tag.QUOTE).Build());
                            break;
                        }
                    default:
                        {
                            Error("语句错误或大括号不匹配");
                            return sentences;
                        }
                }
            }
        }

        public List<Sentence> Buildsstart()
        {
            List<Sentence> sentences = new List<Sentence>();
            while (true)
            {
                Lexer.Next();
                switch (Lexer.Peek.tag)
                {
                    case Tag.EOF:
                        {
                            return sentences;
                        }
                    case Tag.RBRACE:
                        {
                            Error("意外的字符\"}\"");
                            break;
                        }
                    case Tag.BASIC:
                        {
                            sentences.Add(new Function(Tag.FUNCTION).Build());
                            break;
                        }
                    case Tag.USING:
                        {
                            new Using(Tag.USING).Build();
                            break;
                        }
                    case Tag.QUOTE:
                        {
                            Error("引用中已存在函数" + "\"" + ((Word)Lexer.Peek).lexeme + "\"");
                            break;
                        }
                    default:
                        {
                            Error("语句错误");
                            return sentences;
                        }
                }
            }
        }

        public virtual Sentence Build()//这是建立某一个语句
        {
            return this;
        }

        public virtual void Check()
        {
            return;
        }

        public virtual void Run()
        {

        }
    }
}
