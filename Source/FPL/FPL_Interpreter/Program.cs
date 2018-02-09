using FPL.lexer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPL_Interpreter
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            try
            {
                Lexer.Analysis(new StreamReader(new FileStream("Program.fplc", FileMode.Open)));
            }
            catch (FileException)
            {
            }

            watch.Stop();
            TimeSpan timespan = watch.Elapsed;
            Console.WriteLine("文件解析完成，耗时 {0} 毫秒", timespan.TotalMilliseconds);
            Console.WriteLine();

            watch.Reset();
            watch.Start();
            try
            {
                Runner.RunStart();
            }
            catch (RunTimeException)
            {
            }

            watch.Stop();
            timespan = watch.Elapsed;
            Console.WriteLine("程序运行完成，耗时 {0} 毫秒", timespan.TotalMilliseconds);
            Console.ReadKey();
        }
    }
}
