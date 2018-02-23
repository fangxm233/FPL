using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;

namespace FPL.inter
{

    public class Sentence : Node
    {
        public int tag;
        public Sentence(int tag)
        {
            this.tag = tag;
        }
        public List<Sentence> BuildMethod()
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
                            sentences.Add(new Statement(VarType.local, Tag.BASIC).Build());
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
                            if (Lexer.Peek.tag == Tag.LBRACKETS)
                            {
                                Lexer.Back();
                                sentences.Add(new FunctionCall_s(Tag.FUNCTIONCALL).Build());
                                Parser.analyzing_class.FunctionCalls_s.Add((FunctionCall_s)sentences[sentences.Count - 1]);
                                break;
                            }
                            if(Lexer.Peek.tag == Tag.ID)
                            {
                                Lexer.Back();
                                sentences.Add(new Statement(VarType.local, Tag.STATEMENT).Build());
                                break;
                            }
                            Lexer.Back();
                            Lexer.AddBackup();
                            sentences.Add(new Object_s(Tag.OBJECT).Build());
                            //Parser.analyzing_class.Objects_s.Add((Object_s)sentences[sentences.Count - 1]);
                            ((Object_s)sentences[sentences.Count - 1]).statement = GetStatement(((Object_s)sentences[sentences.Count - 1]).name);
                            if (Lexer.Peek.tag == Tag.SEMICOLON) break;
                            Lexer.Next();
                            if (Lexer.Peek.tag == Tag.ASSIGN)
                            {
                                Lexer.Recovery();
                                Lexer.Back();
                                sentences.RemoveAt(sentences.Count - 1);
                                sentences.Add(new Assign(new Expr().BuildStart(), Tag.ASSIGN).Build());
                                break;
                            }
                            Error("语法错误");
                            break;
                        }
                    case Tag.NEW:
                        {
                            sentences.Add(new New_s(Tag.NEW).Build());
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

        public Sentence BuildOne()
        {
            Lexer.Next();
            switch (Lexer.Peek.tag)
            {
                case Tag.BASIC:
                    {
                        return new Statement(VarType.local, Tag.BASIC).Build();
                    }
                case Tag.IF:
                    {
                        return new If(Tag.IF).Build();
                    }
                case Tag.FOR:
                    {
                        return new For(Tag.FOR).Build();
                    }
                case Tag.DO:
                    {
                        return new Do(Tag.DO).Build();
                    }
                case Tag.WHILE:
                    {
                        return new While(Tag.WHILE).Build();
                    }
                case Tag.BREAK:
                    {
                        return new Break(Tag.BREAK).Build();
                    }
                case Tag.CONTINUE:
                    {
                        return new Continue(Tag.CONTINUE).Build();
                    }
                case Tag.RETURN:
                    {
                        return new Return(Tag.RETURN).Build();
                    }
                case Tag.ID:
                    {
                        Lexer.Next();
                        if (Lexer.Peek.tag == Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            Sentence sentence = new FunctionCall_s(Tag.FUNCTIONCALL).Build();
                            Parser.analyzing_class.FunctionCalls_s.Add((FunctionCall_s)sentence);
                            return sentence;
                        }
                        if (Lexer.Peek.tag == Tag.ID)
                        {
                            Lexer.Back();
                            return new Statement(VarType.local, Tag.STATEMENT).Build();
                        }
                        else
                        {
                            Lexer.Back();
                            Lexer.AddBackup();
                            Sentence sentence = new Object_s(Tag.OBJECT).Build();
                            ((Object_s)sentence).statement = GetStatement(((Object_s)sentence).name);
                            if (Lexer.Peek.tag == Tag.SEMICOLON) return sentence;
                            Lexer.Next();
                            if (Lexer.Peek.tag == Tag.ASSIGN)
                            {
                                Lexer.Recovery();
                                Lexer.Back();
                                sentence = new Assign(new Expr().BuildStart(), Tag.ASSIGN).Build();
                                return sentence;
                            }
                            Error("语法错误");
                            return null;
                        }
                    }
                case Tag.NEW:
                    {
                        return new New_s(Tag.NEW).Build();
                    }
                default:
                    {
                        Error("语句错误或大括号不匹配");
                        return null;
                    }
            }
        }

        public List<Sentence> BuildClass()
        {
            List<Sentence> sentences = new List<Sentence>();
            while (true)
            {
                Lexer.Next();
                switch (Lexer.Peek.tag)
                {
                    case Tag.BASIC:
                        {
                            Lexer.Next();
                            Lexer.Next();
                            if (Lexer.Peek.tag == Tag.LBRACKETS)
                            {
                                Lexer.Back();
                                Lexer.Back();
                                sentences.Add(new Function(Tag.FUNCTION).Build());
                                break;
                            }
                            else
                            {
                                Lexer.Back();
                                Lexer.Back();
                                sentences.Add(new Statement(VarType.field, Tag.BASIC).Build());
                                //Parser.analyzing_class.Statement.Add((Statement)sentences[sentences.Count - 1]);
                                break;
                            }
                        }
                    case Tag.ID:
                        {
                            Lexer.Next();
                            if (Lexer.Peek.tag == Tag.LBRACKETS)
                            {
                                Lexer.Back();
                                if (Lexer.Peek.ToString() != Parser.analyzing_class.name) Error("应输入类型");
                                sentences.Add(new Function(symbols.Type.Void, Tag.CONSTRUCTOR).Build());
                                break;
                            }
                            Lexer.Next();
                            if(Lexer.Peek.tag == Tag.LBRACKETS)
                            {
                                if (Lexer.Peek.tag == Tag.LBRACKETS)
                                {
                                    Lexer.Back();
                                    Lexer.Back();
                                    sentences.Add(new Function(Tag.FUNCTION).Build());
                                    break;
                                }
                            }
                            Lexer.Back();
                            Lexer.Back();
                            sentences.Add(new Statement(VarType.field, Tag.STATEMENT).Build());
                            //Parser.analyzing_class.Statement.Add((Statement)sentences[sentences.Count - 1]);
                            break;
                        }
                    case Tag.RBRACE:
                        {
                            return sentences;
                        }
                    default:
                        {
                            Error("语句错误");
                            return sentences;
                        }
                }
            }
        }

        public List<Sentence> BuildStsrt()
        {
            List<Sentence> classes = new List<Sentence>();
            while (true)
            {
                Lexer.Next();
                switch (Lexer.Peek.tag)
                {
                    case Tag.EOF:
                        {
                            return classes;
                        }
                    case Tag.CLASS:
                        {
                            classes.Add(new Class(Tag.CLASS).Build());
                            break;
                        }
                    case Tag.RBRACE:
                        {
                            Error("应输入类型或文件尾");
                            break;
                        }
                    default:
                        {
                            Error("语句错误");
                            return classes;
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

        public virtual void Code()
        {
            return; 
        }

        public virtual void CodeSecond()
        {
            return;
        }
    }
}