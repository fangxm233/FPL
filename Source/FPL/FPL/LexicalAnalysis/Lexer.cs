using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FPL.Parse;
using Type = FPL.symbols.Type;

namespace FPL.LexicalAnalysis
{
    public class Lexer
    {
        public static Token Peek;
        public static int line = 1;
        public static string now_file_name;
        private static char peek = ' ';
        private static readonly Dictionary<string, Word> words = new Dictionary<string, Word>();

        private static readonly Dictionary<string, int> quote = new Dictionary<string, int>();
        private static StreamReader stream_reader;
        private static string[] files;
        private static int now_file_id;

        private static readonly List<Token> peeks = new List<Token>();
        private static int index;
        private static readonly List<int> backups = new List<int>();

        private static void Reserve(Word w)
        {
            words.Add(w.lexeme, w);
        }

        public static void AddQuote(string s)
        {
            quote.Add(s, Tag.QUOTE);
        }

        public static void Analysis(string[] args)
        {
            files = args;
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
            foreach (string item in files)
            {
                stream_reader = new StreamReader(item);
                now_file_name = item;
                peek = ' ';
                while (!stream_reader.EndOfStream) Scan();
                if (peeks.Last().tag != Tag.EOF)
                    Scan();
                line = 1;
                stream_reader.Close();
            }

            peeks.Add(new Token(Tag.EOF));
            now_file_name = files[0];
        }

        private static void Scan()
        {
            for (;; Readch()) //去掉所有空白
                if (peek == ' ' || peek == '\t' || peek == '\n')
                {
                }
                else if (peek == '\r')
                {
                    line++;
                    peeks.Add(new Token(Tag.EOL));
                }
                else if (peek == 65535)
                {
                    peeks.Add(new Token(Tag.EOF));
                    return;
                }
                else
                {
                    break;
                }

            if (peek == '/') //开始检测注释
            {
                Readch();
                if (peek == '/')
                    for (;; Readch())
                    {
                        if (peek == '\r')
                        {
                            line++;
                            peeks.Add(new Token(Tag.EOL));
                            Readch();
                            return;
                        }

                        if (peek == '\uffff') //如果是文件未就返回一个文件尾
                        {
                            peeks.Add(new Token(peek));
                            return;
                        }
                    }

                if (peek == '*')
                {
                    Readch();
                    for (;; Readch())
                    {
                        if (peek == '\r')
                        {
                            line++;
                            peeks.Add(new Token(Tag.EOL));
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
                    peeks.Add(Readch('&') ? Word.and : new Token('&'));
                    return;
                case '|':
                    peeks.Add(Readch('|') ? Word.or : new Token('|'));
                    return;
                case '=':
                    peeks.Add(Readch('=') ? Word.eq : Word.assign);
                    return;
                case '!':
                    peeks.Add(Readch('=') ? Word.ne : new Token('!'));
                    return;
                case '<':
                    peeks.Add(Readch('=') ? Word.le : Word.less);
                    return;
                case '>':
                    peeks.Add(Readch('=') ? Word.ge : Word.more);
                    return;
                case '+':
                    peeks.Add(Readch('+') ? Word.increase : Word.plus);
                    return;
                case '-':
                    peeks.Add(Readch('-') ? Word.decline : Word.minus);
                    return;
                case '*':
                    peeks.Add(Word.multiply);
                    Readch();
                    return;
                case '/':
                    break; //除号已在前面处理过了
                case ';':
                    peeks.Add(Word.semicolon);
                    Readch();
                    return;
                case '(':
                    peeks.Add(Word.Lparenthesis);
                    Readch();
                    return;
                case ')':
                    peeks.Add(Word.Rparenthesis);
                    Readch();
                    return;
                case '{':
                    peeks.Add(Word.LBrace);
                    Readch();
                    return;
                case '}':
                    peeks.Add(Word.RBrace);
                    Readch();
                    return;
                case '[':
                    peeks.Add(Word.LSquBrackets);
                    Readch();
                    return;
                case ']':
                    peeks.Add(Word.RSquBrackets);
                    Readch();
                    return;
                case '%':
                    peeks.Add(Word.modulo);
                    Readch();
                    return;
                case ',':
                    peeks.Add(Word.comma);
                    Readch();
                    return;
                case '.':
                    peeks.Add(Word.dot);
                    Readch();
                    return;
            }

            if (peek == '"')
            {
                string s = "";
                Readch();
                for (;; Readch())
                {
                    if (peek == '"')
                    {
                        Str str = new Str(s);
                        Readch();
                        peeks.Add(str);
                        return;
                    }

                    if (peek == '\n')
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

            if (char.IsLetter(peek) || peek == '_') //检测标识符
            {
                StringBuilder b = new StringBuilder();
                do
                {
                    b.Append(peek);
                    Readch();
                } while (char.IsLetterOrDigit(peek) || peek == '_');

                string s = b.ToString();
                Word w = null;
                if (words.ContainsKey(s))
                    w = words[s];
                if (w != null)
                {
                    peeks.Add(w);
                    return;
                }

                w = new Word(s, Tag.ID);
                words.Add(s, w);
                peeks.Add(w);
                return;
            }

            Node.Error("意外的字符" + "\"" + peek + "\"");
        }

        public static void AddBackup()
        {
            backups.Add(index);
        }

        public static void ThrowBackup()
        {
            backups.RemoveAt(backups.Count - 1);
        }

        public static void Recovery()
        {
            index = backups[backups.Count - 1];
            index--;
            Next();
            ThrowBackup();
        }

        public static void Next()
        {
            while (true)
            {
                Peek = peeks[index++];
                switch (Peek.tag)
                {
                    case Tag.EOL:
                        line++;
                        continue;
                    case Tag.ID:
                        if (quote.ContainsKey(Peek.ToString())) Peek.tag = Tag.QUOTE;
                        break;
                    case Tag.EOF:
                        if (now_file_id == files.Length - 1)
                            return;
                        else
                            line = 1;
                        now_file_name = files[++now_file_id];
                        continue;
                }

                break;
            }
        }

        public static void Back()
        {
            while (true)
            {
                index -= 2;
                Peek = peeks[index++];
                if (Peek.tag == Tag.EOL)
                {
                    line--;
                    continue;
                }

                break;
            }
        }

        private static void Readch()
        {
            peek = (char) stream_reader.Read();
        }

        private static bool Readch(char c)
        {
            Readch();
            if (peek != c) return false;
            peek = ' ';
            return true;
        }
    }
}