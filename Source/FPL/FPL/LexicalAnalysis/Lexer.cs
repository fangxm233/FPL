using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FPL.DataStorager;
using FPL.OutPut;
using FPL.Parse;
using Type = FPL.DataStorager.Type;

namespace FPL.LexicalAnalysis
{
    public class Lexer
    {
        public static Token NextToken;
        public static int Line = 1;
        public static string NowFileName;
        private static char Peek = ' ';
        private static readonly Dictionary<string, Word> Words = new Dictionary<string, Word>();

        private static readonly Dictionary<string, int> Quote = new Dictionary<string, int>();
        private static StreamReader StreamReader;
        private static string[] Files;
        private static int NowFileId;

        private static readonly List<Token> Peeks = new List<Token>();
        private static int Index;
        private static readonly List<int> Backups = new List<int>();

        private static void Reserve(Word w)
        {
            Words.Add(w.Lexeme, w);
        }

        public static void AddQuote(string s)
        {
            Quote.Add(s, Tag.QUOTE);
        }

        public static void Analysis(string[] args)
        {
            Files = args;
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
            Reserve(new Word("class", Tag.CLASS));
            Reserve(new Word("new", Tag.NEW));
            Reserve(new Word("static", Tag.STATIC));
            Reserve(Word.True);
            Reserve(Word.False);
            Reserve(Type.Int);
            Reserve(Type.Char);
            Reserve(Type.Bool);
            Reserve(Type.Float);
            Reserve(Type.String);
            Reserve(Type.Void);
            foreach (string item in Files)
            {
                StreamReader = new StreamReader(item);
                NowFileName = item;
                Peek = ' ';
                while (!StreamReader.EndOfStream) Scan();
                if (Peeks.Last().tag != Tag.EOF)
                    Scan();
                Line = 1;
                StreamReader.Close();
            }

            Peeks.Add(new Token(Tag.EOF));
            NowFileName = Files[0];
        }

        private static void Scan()
        {
            for (;; Readch()) //去掉所有空白
                if (Peek == ' ' || Peek == '\t' || Peek == '\n')
                {
                }
                else if (Peek == '\r')
                {
                    Line++;
                    Peeks.Add(new Token(Tag.EOL));
                }
                else if (Peek == 65535)
                {
                    Peeks.Add(new Token(Tag.EOF));
                    return;
                }
                else
                {
                    break;
                }

            if (Peek == '/') //开始检测注释
            {
                Readch();
                if (Peek == '/')
                    for (;; Readch())
                    {
                        if (Peek == '\r')
                        {
                            Line++;
                            Peeks.Add(new Token(Tag.EOL));
                            Readch();
                            return;
                        }

                        if (Peek == '\uffff') //如果是文件未就返回一个文件尾
                        {
                            Peeks.Add(new Token(Peek));
                            return;
                        }
                    }

                if (Peek == '*')
                {
                    Readch();
                    for (;; Readch())
                    {
                        if (Peek == '\r')
                        {
                            Line++;
                            Peeks.Add(new Token(Tag.EOL));
                            Readch();
                        }

                        if (Peek == '*')
                        {
                            Readch();
                            if (Peek == '/')
                            {
                                Readch();
                                return;
                            }
                        }

                        if (Peek == '\uffff')
                        {
                            Peeks.Add(new Token(Peek));
                            return;
                        }
                    }
                }

                Peeks.Add(Word.Divide);
                return;
            }

            switch (Peek) //检测并返回各个符号以及组合符号
            {
                case '&':
                    Peeks.Add(Readch('&') ? Word.And : new Token('&'));
                    return;
                case '|':
                    Peeks.Add(Readch('|') ? Word.Or : new Token('|'));
                    return;
                case '=':
                    Peeks.Add(Readch('=') ? Word.Eq : Word.Assign);
                    return;
                case '!':
                    Peeks.Add(Readch('=') ? Word.Ne : new Token('!'));
                    return;
                case '<':
                    Peeks.Add(Readch('=') ? Word.Le : Word.Less);
                    return;
                case '>':
                    Peeks.Add(Readch('=') ? Word.Ge : Word.More);
                    return;
                case '+':
                    Peeks.Add(Readch('+') ? Word.Increase : Word.Plus);
                    return;
                case '-':
                    Peeks.Add(Readch('-') ? Word.Decline : Word.Minus);
                    return;
                case '*':
                    Peeks.Add(Word.Multiply);
                    Readch();
                    return;
                case '/':
                    break; //除号已在前面处理过了
                case ';':
                    Peeks.Add(Word.Semicolon);
                    Readch();
                    return;
                case '(':
                    Peeks.Add(Word.Lparenthesis);
                    Readch();
                    return;
                case ')':
                    Peeks.Add(Word.Rparenthesis);
                    Readch();
                    return;
                case '{':
                    Peeks.Add(Word.LBrace);
                    Readch();
                    return;
                case '}':
                    Peeks.Add(Word.RBrace);
                    Readch();
                    return;
                case '[':
                    Peeks.Add(Word.LSquBrackets);
                    Readch();
                    return;
                case ']':
                    Peeks.Add(Word.RSquBrackets);
                    Readch();
                    return;
                case '%':
                    Peeks.Add(Word.Modulo);
                    Readch();
                    return;
                case ',':
                    Peeks.Add(Word.Comma);
                    Readch();
                    return;
                case '.':
                    Peeks.Add(Word.Dot);
                    Readch();
                    return;
            }

            if (Peek == '"')
            {
                string s = "";
                Readch();
                for (;; Readch())
                {
                    if (Peek == '"')
                    {
                        Str str = new Str(s);
                        Readch();
                        Peeks.Add(str);
                        return;
                    }

                    if (Peek == '\n')
                    {
                        Node.Error(LogContent.SthExpect, "\"");
                        Str str = new Str(s);
                        Readch();
                        Peeks.Add(str);
                        return;
                    }

                    s = s + Peek;
                }
            }

            if (char.IsDigit(Peek)) //检测数字
            {
                string n = "";
                do
                {
                    n = n + Peek;
                    Readch();
                } while (char.IsDigit(Peek));

                if (Peek != '.')
                {
                    try
                    {
                        Peeks.Add(new Num(int.Parse(n)));
                    }
                    catch (Exception)
                    {
                        Node.Error(LogContent.NumberOutOfRange);
                    }

                    return;
                }

                string f = n;
                f = f + Peek;
                for (;;)
                {
                    Readch();
                    if (!char.IsDigit(Peek)) break;
                    f = f + Peek;
                }

                try
                {
                    Peeks.Add(new Real(float.Parse(f)));
                }
                catch (Exception)
                {
                    Node.Error(LogContent.FloatOutOfRange);
                }

                return;
            }

            if (char.IsLetter(Peek) || Peek == '_') //检测标识符
            {
                StringBuilder b = new StringBuilder();
                do
                {
                    b.Append(Peek);
                    Readch();
                } while (char.IsLetterOrDigit(Peek) || Peek == '_');

                string s = b.ToString();
                Word w = null;
                if (Words.ContainsKey(s))
                    w = Words[s];
                if (w != null)
                {
                    Peeks.Add(w);
                    return;
                }

                w = new Word(s, Tag.ID);
                Words.Add(s, w);
                Peeks.Add(w);
                return;
            }

            Node.Error(LogContent.SthUnexpect, Peek);
        }

        public static void AddBackup()
        {
            Backups.Add(Index);
        }

        public static void ThrowBackup()
        {
            Backups.RemoveAt(Backups.Count - 1);
        }

        public static void Recovery()
        {
            Index = Backups[Backups.Count - 1];
            Index--;
            Next();
            ThrowBackup();
        }

        public static void Next()
        {
            while (true)
            {
                NextToken = Peeks[Index++];
                switch (NextToken.tag)
                {
                    case Tag.EOL:
                        Line++;
                        continue;
                    case Tag.ID:
                        if (Quote.ContainsKey(NextToken.ToString())) NextToken.tag = Tag.QUOTE;
                        break;
                    case Tag.EOF:
                        if (NowFileId == Files.Length - 1)
                            return;
                        else
                            Line = 1;
                        NowFileName = Files[++NowFileId];
                        continue;
                }

                break;
            }
        }

        public static void Back()
        {
            while (true)
            {
                Index -= 2;
                NextToken = Peeks[Index++];
                if (NextToken.tag == Tag.EOL)
                {
                    Line--;
                    continue;
                }

                break;
            }
        }

        private static void Readch()
        {
            Peek = (char) StreamReader.Read();
        }

        private static bool Readch(char c)
        {
            Readch();
            if (Peek != c) return false;
            Peek = ' ';
            return true;
        }
    }
}