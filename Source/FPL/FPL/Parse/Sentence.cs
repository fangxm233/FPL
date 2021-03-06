﻿using System.Collections.Generic;
using FPL.DataStorager;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Expression;
using FPL.Parse.Sentences;
using FPL.Parse.Sentences.Loop;
using FPL.Parse.Sentences.ProcessControl;
using FPL.Parse.Structure;

namespace FPL.Parse
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
            var sentences = new List<Sentence>();
            while (true)
            {
                Lexer.Next();
                switch (Lexer.NextToken.tag)
                {
                    case Tag.RBRACE:
                    {
                        return sentences;
                    }
                    case Tag.BASIC:
                    {
                        sentences.Add(new Statement(VarType.Local, Tag.BASIC).Build());
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
                        if (Lexer.NextToken.tag == Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            sentences.Add(new FunctionCall_s(Tag.FUNCTIONCALL).Build());
                            Parser.AnalyzingClass.FunctionCalls_s.Add((FunctionCall_s) sentences[sentences.Count - 1]);
                            break;
                        }

                        if (Lexer.NextToken.tag == Tag.ID)
                        {
                            Lexer.Back();
                            sentences.Add(new Statement(VarType.Local, Tag.STATEMENT).Build());
                            break;
                        }

                        Lexer.Back();
                        Lexer.AddBackup();
                        sentences.Add(new Object_s(Tag.OBJECT).Build());
                        //Parser.analyzing_class.Objects_s.Add((Object_s)sentences[sentences.Count - 1]);
                        ((Object_s) sentences[sentences.Count - 1]).Statement =
                            GetStatement(((Object_s) sentences[sentences.Count - 1]).Name, VarType.Unknown);
                        if (Lexer.NextToken.tag == Tag.SEMICOLON) break;
                        Lexer.Next();
                        if (Lexer.NextToken.tag == Tag.ASSIGN)
                        {
                            Lexer.Recovery();
                            Lexer.Back();
                            sentences.RemoveAt(sentences.Count - 1);
                            sentences.Add(new Assign(new Expr().BuildStart(), Tag.ASSIGN).Build());
                            break;
                        }

                        ErrorSta(LogContent.GarmmarError);
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
                        ErrorSta(LogContent.SentenceErrorOrRBraceDoesNotMatch);
                        return sentences;
                    }
                }
            }
        }

        public Sentence BuildOne()
        {
            Lexer.Next();
            switch (Lexer.NextToken.tag)
            {
                case Tag.BASIC:
                {
                    return new Statement(VarType.Local, Tag.BASIC).Build();
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
                    if (Lexer.NextToken.tag == Tag.LBRACKETS)
                    {
                        Lexer.Back();
                        Sentence sentence = new FunctionCall_s(Tag.FUNCTIONCALL).Build();
                        Parser.AnalyzingClass.FunctionCalls_s.Add((FunctionCall_s) sentence);
                        return sentence;
                    }

                    if (Lexer.NextToken.tag == Tag.ID)
                    {
                        Lexer.Back();
                        return new Statement(VarType.Local, Tag.STATEMENT).Build();
                    }

                    {
                        Lexer.Back();
                        Lexer.AddBackup();
                        Sentence sentence = new Object_s(Tag.OBJECT).Build();
                        ((Object_s) sentence).Statement = GetStatement(((Object_s) sentence).Name, VarType.Unknown);
                        if (Lexer.NextToken.tag == Tag.SEMICOLON) return sentence;
                        Lexer.Next();
                        if (Lexer.NextToken.tag == Tag.ASSIGN)
                        {
                            Lexer.Recovery();
                            Lexer.Back();
                            sentence = new Assign(new Expr().BuildStart(), Tag.ASSIGN).Build();
                            return sentence;
                        }

                        ErrorSta(LogContent.GarmmarError);
                        return null;
                    }
                }
                case Tag.NEW:
                {
                    return new New_s(Tag.NEW).Build();
                }
                default:
                {
                    ErrorSta(LogContent.SentenceErrorOrRBraceDoesNotMatch);
                    return null;
                }
            }
        }

        public List<Sentence> BuildClass()
        {
            var sentences = new List<Sentence>();
            while (true)
            {
                Lexer.Next();
                switch (Lexer.NextToken.tag)
                {
                    case Tag.BASIC:
                    {
                        Lexer.Next();
                        Lexer.Next();
                        if (Lexer.NextToken.tag == Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            Lexer.Back();
                            sentences.Add(new Function(FuncType.Member, Tag.FUNCTION).Build());
                            break;
                        }

                        Lexer.Back();
                        Lexer.Back();
                        sentences.Add(new Statement(VarType.Field, Tag.BASIC).Build());
                        //Parser.analyzing_class.Statement.Add((Statement)sentences[sentences.Count - 1]);
                        break;
                    }
                    case Tag.STATIC:
                    {
                        Lexer.Next();
                        Lexer.Next();
                        if (Lexer.NextToken.tag == Tag.OPERATOR)
                        {
                            Lexer.Back();
                            sentences.Add(new Function(FuncType.OperatorFunc, Tag.OPERATORFUNC).Build());
                            break;
                        }
                        Lexer.Next();
                        if (Lexer.NextToken.tag == Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            Lexer.Back();
                            sentences.Add(new Function(FuncType.Static, Tag.FUNCTION).Build());
                            break;
                        }

                        Lexer.Back();
                        Lexer.Back();
                        sentences.Add(new Statement(VarType.Static, Tag.BASIC).Build());
                        //Parser.analyzing_class.Statement.Add((Statement)sentences[sentences.Count - 1]);
                        break;
                    }
                    case Tag.ID:
                    {
                        Lexer.Next();
                        if (Lexer.NextToken.tag == Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            if (Lexer.NextToken.ToString() != Parser.AnalyzingClass.Name) ErrorSta(LogContent.InPutType);
                            sentences.Add(new Function(FuncType.Constructor, Type.Void, Tag.CONSTRUCTOR).Build());
                            break;
                        }

                        Lexer.Next();
                        if (Lexer.NextToken.tag == Tag.LBRACKETS)
                            if (Lexer.NextToken.tag == Tag.LBRACKETS)
                            {
                                Lexer.Back();
                                Lexer.Back();
                                sentences.Add(new Function(FuncType.Member, Tag.FUNCTION).Build());
                                break;
                            }

                        Lexer.Back();
                        Lexer.Back();
                        sentences.Add(new Statement(VarType.Field, Tag.STATEMENT).Build());
                        //Parser.analyzing_class.Statement.Add((Statement)sentences[sentences.Count - 1]);
                        break;
                    }
                    case Tag.RBRACE:
                    {
                        return sentences;
                    }
                    default:
                    {
                        ErrorSta(LogContent.SentenceError);
                        return sentences;
                    }
                }
            }
        }

        public List<Sentence> BuildStsrt()
        {
            var classes = new List<Sentence>();
            while (true)
            {
                Lexer.Next();
                switch (Lexer.NextToken.tag)
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
                        ErrorSta(LogContent.InPutTypeOrFileEnd);
                        break;
                    }
                    default:
                    {
                        ErrorSta(LogContent.SentenceError);
                        return classes;
                    }
                }
            }
        }

        public virtual Sentence Build() //这是建立某一个语句
        {
            return this;
        }

        public virtual void Check()
        {
        }

        public virtual void Code()
        {
        }

        public virtual void CodeSecond()
        {
        }
    }
}