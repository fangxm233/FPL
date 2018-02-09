using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FPL_Interpreter;

namespace FPL.lexer
{
    public class Lexer
    {
        public static int line = 1;
        static char peek = ' ';

        static StreamReader stream_reader;

        public static List<InstructionsType> instructions = new List<InstructionsType>();
        public static List<int> parameters = new List<int>();
        public static List<int> methods = new List<int>();

        public static void Analysis(StreamReader stream_reader)
        {
            Lexer.stream_reader = stream_reader;
            while (!stream_reader.EndOfStream)
            {
                Scan();
            }
        }

        static void Scan()
        {
            for (; ; Readch()) //去掉所有空白
            {
                if (peek == ' ' || peek == '\t' || peek == '\r')
                {
                    continue;
                }
                else if (peek == '\n' || peek == '\uffff')
                {
                    line++;
                }
                else if (peek == '\uffff') return;
                else break;
            }
            {
                string n = "";
                do
                {
                    n = n + peek;
                    Readch();
                } while (char.IsDigit(peek));
                instructions.Add((InstructionsType)int.Parse(n));
            }
            Readch();
            for (; ; Readch()) //去掉所有空白
            {
                if (peek == ' ' || peek == '\t' || peek == '\r')
                {
                    continue;
                }
                else if (peek == '\n'|| peek == '\uffff')
                {
                    if (instructions.Count != parameters.Count) parameters.Add(0);
                    return;
                }
                else
                {
                    if (instructions.Count == parameters.Count) OutPut.FileError("暂不支持第二个参数");
                    if (char.IsDigit(peek)) //检测数字
                    {
                        string n = "";
                        do
                        {
                            n = n + peek;
                            Readch();
                        } while (char.IsDigit(peek));
                        if (instructions[instructions.Count - 1] == InstructionsType.func)
                        {
                            methods.Add(int.Parse(n));
                            instructions.RemoveAt(instructions.Count - 1);
                        }
                        else
                        {
                            parameters.Add(int.Parse(n));
                        }
                    }
                }
            }
        }

        static void Readch()
        {
            peek = (char)stream_reader.Read();
        }
    }
}
