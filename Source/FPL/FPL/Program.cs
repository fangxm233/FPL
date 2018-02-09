using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FPL.inter;
using FPL.Encoding;
using System.Diagnostics;

namespace FPL
{
    class Program
    {
        /* 目前支持
         * for
         * if
         * if else
         * while
         * do while
         * break
         * 强类型声明
         * 强类型运算
         * 函数
         */
        /* 目前有的判断语句标志
         * 各种变量名
         * 各种类型名
         * while
         * do
         * for
         * if 
         * else
         * break
         */
        /* 编译时变量存储
         * 一个类，存名字和type
         * 一个列表存储一个作用域的变量
         * 再一个列表存作用域们
         */

        static void Main(string[] args)
        {
            if (Compile("Program.fpl"))
            {
                //ProcessStartInfo processin = new ProcessStartInfo()
                //{
                //    FileName = "FPL_Interpreter.exe",
                //    RedirectStandardOutput = true,
                //    CreateNoWindow = false,
                //    UseShellExecute = false,
                //};
                //Process process = Process.Start(processin);
                //process.OutputDataReceived += OutPut;
                Process.Start("FPL_Interpreter.exe");
            }
            Console.ReadKey();
        }

        //private static void OutPut(object sender, DataReceivedEventArgs e)
        //{
        //    Console.WriteLine(e.Data);
        //}

        static bool Compile(string args)
        {
            try
            {
                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                Lexer.Analysis(new StreamReader(new FileStream(args, FileMode.Open)));
                Parser parser = new Parser();
                parser.Compile();
                Encoder.Init("Program.fplc");
                parser.Code();
                parser.CodeSecond();
                Encoder.WriteToFile();

                watch.Stop();
                TimeSpan timespan = watch.Elapsed;
                Console.WriteLine("编译完成");
                Console.WriteLine("执行时间：{0}(毫秒)", timespan.TotalMilliseconds);
                Console.WriteLine("");
                return true;
            }
            catch (CompileException) { return false; }
        }
    }
}
