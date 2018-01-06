using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FPL.symbols;
using FPL.inter;

namespace FPL.lexer
{
    public class Lexer
    {
        public static Token Peek;
        public static int line = 1;
        static char peek = ' ';
        //Hashtable words = new Hashtable();
        static Dictionary<string, Word> words = new Dictionary<string, Word>();

        //Hashtable quote = new Hashtable();
        static Dictionary<string, int> quote = new Dictionary<string, int>();
        static StreamReader stream_reader;

        public static List<Token> peeks = new List<Token>();
        public static int index;

        static void Reserve(Word w)
        {
            words.Add(w.lexeme, w);
        }

        public static void AddQuote(string s)
        {
            quote.Add(s, Tag.QUOTE);
        }

        public static void Analysis(StreamReader stream_reader)
        {
            Lexer.stream_reader = stream_reader;
            //把各种保留字写在这
            Reserve(new Word("if", Tag.IF));
            Reserve(new Word("else", Tag.ELSE));
            Reserve(new Word("while", Tag.WHILE));
            Reserve(new Word("do", Tag.DO));
            Reserve(new Word("break", Tag.BREAK));
            Reserve(new Word("continue", Tag.CONTINUE));
            Reserve(new Word("for", Tag.FOR));
            Reserve(new Word("using", Tag.USING));
            Reserve(new Word("return", Tag.RETURN));
            Reserve(Word.True);
            Reserve(Word.False);
            Reserve(symbols.Type.Int);
            Reserve(symbols.Type.Char);
            Reserve(symbols.Type.Bool);
            Reserve(symbols.Type.Float);
            Reserve(symbols.Type.String);
            Reserve(symbols.Type.Void);
            while (!stream_reader.EndOfStream)
            {
                Scan();
            }
            peeks.Add(new Token(Tag.EOF));
            line = 1;
        }

        static void Scan()
        {
            for (; ; Readch()) //去掉所有空白
            {
                if (peek == ' ' || peek == '\t' || peek == '\r')
                {
                    continue;
                }
                else if (peek == '\n')
                {
                    line++;
                    peeks.Add(new Token(Tag.EOL));
                }
                else break;
            }
            if (peek == '/') //开始检测注释
            {
                Readch();
                if (peek == '/')
                {
                    for (; ; Readch())
                    {
                        if (peek == '\r') //如果是文件未就返回一个文件尾
                        {
                            line++;
                            Readch();
                            return;
                        }
                        if(peek == '\uffff')
                        {
                            peeks.Add(new Token(peek));
                            return;
                        }
                    }
                }
                if (peek == '*')
                {
                    Readch();
                    for (; ; Readch())
                    {
                        if (peek == '\r')
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
                                return;
                            }
                        }
                        if (peek == '\uffff')
                        {
                            peeks.Add(new Token(peek));
                            return;
                        }
                    }
                }
                peeks.Add(Word.divide);
                return;
            }
            switch (peek) //检测并返回各个符号以及组合符号
            {
                case '&':
                    {
                        if (Readch('&')) peeks.Add(Word.and); else peeks.Add(new Token('&'));
                        return;
                    }
                case '|':
                    {
                        if (Readch('|')) peeks.Add(Word.or); else peeks.Add(new Token('|'));
                        return;
                    }
                case '=':
                    {
                        if (Readch('=')) peeks.Add(Word.eq); else peeks.Add(Word.assign);
                        return;
                    }
                case '!':
                    {
                        if (Readch('=')) peeks.Add(Word.ne); else peeks.Add(new Token('!'));
                        return;
                    }
                case '<':
                    {
                        if (Readch('=')) peeks.Add(Word.le); else peeks.Add(Word.less);
                        return;
                    }
                case '>':
                    {
                        if (Readch('=')) peeks.Add(Word.ge); else peeks.Add(Word.more);
                        return;
                    }
                case '+':
                    {
                        peeks.Add(Word.plus);
                        Readch();
                        return;
                    }
                case '-':
                    {
                        peeks.Add(Word.minus);
                        Readch();
                        return;
                    }
                case '*':
                    {
                        peeks.Add(Word.multiply);
                        Readch();
                        return;
                    }
                case '/':
                    {
                        break;//除号已在前面处理过了
                    }
                case ';':
                    {
                        peeks.Add(Word.semicolon);
                        Readch();
                        return;
                    }
                case '(':
                    {
                        peeks.Add(Word.Lparenthesis);
                        Readch();
                        return;
                    }
                case ')':
                    {
                        peeks.Add(Word.Rparenthesis);
                        Readch();
                        return;
                    }
                case '{':
                    {
                        peeks.Add(Word.LBrace);
                        Readch();
                        return;
                    }
                case '}':
                    {
                        peeks.Add(Word.RBrace);
                        Readch();
                        return;
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
                        peeks.Add(str);
                        return;
                    }
                    else if (peek == '\n')
                    {
                        Node.Error("应输入\"\"\"");
                        Str str = new Str(s);
                        Readch();
                        peeks.Add(str);
                        return;
                    }
                    s = s + peek;
                }
            }
            if (char.IsDigit(peek)) //检测数字
            {
                string n = "";
                do
                {
                    n = n + peek;
                    Readch();
                } while (char.IsDigit(peek));
                if (peek != '.')
                {
                    try
                    {
                        peeks.Add(new Num(int.Parse(n)));
                    }
                    catch (Exception)
                    {
                        Node.Error("整数超出范围");
                    }
                    return;
                }
                string f = n;
                f = f + peek;
                for (;;)
                {
                    Readch();
                    if (!char.IsDigit(peek)) break;
                    f = f + peek;
                }
                try
                {
                    peeks.Add(new Real(float.Parse(f)));
                }
                catch (Exception)
                {
                    Node.Error("浮点数超出范围");
                }
                return;
            }
            if (char.IsLetter(peek)) //检测标识符
            {
                StringBuilder b = new StringBuilder();
                do
                {
                    b.Append(peek);
                    Readch();
                } while (char.IsLetterOrDigit(peek));
                string s = b.ToString();
                Word w = null;
                if (words.ContainsKey(s))
                w = words[s];
                if (w != null) {
                    peeks.Add(w);
                    return;
                }
                w = new Word(s, Tag.ID);
                words.Add(s, w);
                peeks.Add(w);
                return;
            }
            Token tok = new Token(peek);
            peek = ' ';
            peeks.Add(tok);
        }

        public static void Next()
        {
            Peek = peeks[index++];
            if(Peek.tag == Tag.EOL)
            {
                line++;
                Next();
            }
            if (Peek.tag == Tag.ID)
            {
                if (quote.ContainsKey(((Word)Peek).lexeme))
                {
                    Peek.tag = Tag.QUOTE;
                }
            }
        }
        public static void Back()
        {
            index -= 2;
            Next();
        }

        static void Readch()
        {
            peek = (char)stream_reader.Read();
        }

        static bool Readch(char c)
        {
            Readch();
            if (peek != c) return false;
            peek = ' ';
            return true;
        }
    }
}
