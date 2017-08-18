using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FPL.symbols;

namespace FPL.lexer
{
    public class Lexer
    {
        public static Token Peek;
        public static int line = 1;
        char peek = ' ';
        Hashtable words = new Hashtable();
        StreamReader stream_reader;
        void Reserve(Word w)
        {
            words.Add(w.lexeme, w);
        }

        public Lexer(StreamReader stream_reader)
        {
            this.stream_reader = stream_reader;
            Reserve(new Word("if", Tag.IF));
            Reserve(new Word("else", Tag.ELSE));
            Reserve(new Word("while", Tag.WHILE));
            Reserve(new Word("do", Tag.DO));
            Reserve(new Word("break", Tag.BREAK));
            Reserve(new Word("for", Tag.FOR));
            Reserve(Word.True);
            Reserve(Word.False);
            Reserve(symbols.Type.Int);
            Reserve(symbols.Type.Char);
            Reserve(symbols.Type.Bool);
            Reserve(symbols.Type.Float);
            Reserve(symbols.Type.String);
        }

        public void Scan()
        {
            start:
            for (; ; Readch())
            {
                if (peek == ' ' || peek == '\t' || peek == '\r')
                {
                    continue;
                }
                else if (peek == '\n')
                {
                    line++;
                }
                else break;
            }
            switch (peek)
            {
                case '&':
                    {
                        if (Readch('&')) Peek = Word.and; else Peek = new Token('&');
                        return;
                    }
                case '|':
                    {
                        if (Readch('|')) Peek = Word.or; else Peek = new Token('|');
                        return;
                    }
                case '=':
                    {
                        if (Readch('=')) Peek = Word.eq; else Peek = Word.assign;
                        return;
                    }
                case '!':
                    {
                        if (Readch('=')) Peek =  Word.ne; else Peek =  new Token('!');
                        return;
                    }
                case '<':
                    {
                        if (Readch('=')) Peek = Word.le; else Peek = Word.less;
                        return;
                    }
                case '>':
                    {
                        if (Readch('=')) Peek = Word.ge; else Peek = Word.more;
                        return;
                    }
                case '+':
                    {
                        Peek = Word.plus;
                        Readch();
                        return;
                    }
                case '-':
                    {
                        Peek = Word.minus;
                        Readch();
                        return;
                    }
                case '*':
                    {
                        Peek = Word.multiply;
                        Readch();
                        return;
                    }
                case '/':
                    {
                        Peek = Word.divide;
                        Readch();
                        return;
                    }
                case ';':
                    {
                        Peek = Word.semicolon;
                        Readch();
                        return;
                    }
                case '(':
                    {
                        Peek = Word.Lparenthesis;
                        Readch();
                        return;
                    }
                case ')':
                    {
                        Peek = Word.Rparenthesis;
                        Readch();
                        return;
                    }
                case '{':
                    {
                        Peek = Word.LBrace;
                        Readch();
                        return;
                    }
                case '}':
                    {
                        Peek = Word.RBrace;
                        Readch();
                        return;
                    }
            }
            if (peek == '/')
            {
                Readch();
                if (peek == '/')
                {
                    for (; ; Readch())
                    {
                        if (peek == '\n')
                        {
                            line++;
                            Readch();
                            goto start;
                        }
                    }
                }
                if(peek == '*')
                {
                    for (; ; Readch())
                    {
                        if (peek == '\n')
                        {
                            line++;
                            Readch();
                        }
                        if (peek == '*')
                        {
                            Readch();
                            if (peek == '/')
                            {
                                Readch();
                                goto start;
                            }
                        }
                    }
                }
            }
            if (peek == '"')
            {
                string s = "";
                Readch();
                for (; ; Readch())
                {
                    if (peek == '"')
                    {
                        Str str = new Str(s);
                        Readch();
                        Peek = str;
                        return;
                    }
                    else if (peek == '\n')
                    {
                        //报错string"缺失
                        Str str = new Str(s);
                        Readch();
                        Peek = str;
                        return;
                    }
                    s = s + peek;
                }
            }
            if (char.IsDigit(peek))
            {
                string n = "";
                do
                {
                    n = n + peek;
                    Readch();
                } while (char.IsDigit(peek));
                if (peek != '.')
                {
                    Peek = new Num(int.Parse(n));
                    return;
                }//此处应有报错，整数过长
                string f = n;
                f = f + peek;
                for (;;)
                {
                    Readch();
                    if (!char.IsDigit(peek)) break;
                    f = f + peek;
                }
                Peek = new Real(float.Parse(f));//此处应有报错，浮点过长
                return;
            }
            if (char.IsLetter(peek))
            {
                StringBuilder b = new StringBuilder();
                do
                {
                    b.Append(peek);
                    Readch();
                } while (char.IsLetterOrDigit(peek));
                string s = b.ToString();
                Word w = (Word)words[s];
                if (w != null) {
                    Peek = w;
                    return;
                }
                w = new Word(s, Tag.ID);
                words.Add(s, w);
                Peek = w;
                return;
            }
            Token tok = new Token(peek);
            peek = ' ';
            Peek = tok;
        }

        void Readch()
        {
            //if (stream_reader.EndOfStream)
            //{
            //    peek = '}';
            //    return;
            //}
            peek = (char)stream_reader.Read();
        }

        bool Readch(char c)
        {
            Readch();
            if (peek != c) return false;
            peek = ' ';
            return true;
        }
    }
}
