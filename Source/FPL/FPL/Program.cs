using System;
using System.Diagnostics;
using FPL.Generator;
using FPL.LexicalAnalysis;
using FPL.Parse;

namespace FPL
{
    internal class Program
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
        //TODO:修改条件判断
        //TODO:修改解释器读取文件
        //TODO:修改编译器结构
        private static void Main(string[] args)
        {
            //args = new[]
            //{
            //    "Program.fpl", "System.fpl"
            //};
            if (args.Length == 0)
            {
                Console.WriteLine("未指明文件");
                Console.ReadKey();
                return;
            }

            if (Compile(args))
            {
                //Console.WriteLine("启动解释器");
                //Process.Start("FPL_Interpreter c++.exe");
            }

            Console.ReadKey();
        }

        private static bool Compile(string[] args)
        {
            try
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                Lexer.Analysis(args);
                Parser parser = new Parser();
                parser.Compile();
                FILGenerator.Init("Program.fplc");
                parser.Code();
                parser.CodeSecond();
                FILGenerator.WriteToFile(true);

                watch.Stop();
                TimeSpan timespan = watch.Elapsed;
                Console.WriteLine("编译完成");
                Console.WriteLine("执行时间：{0}(毫秒)", timespan.TotalMilliseconds);
                Console.WriteLine("");
                return true;
            }
            catch (CompileException)
            {
                return false;
            }
        }
    }
}