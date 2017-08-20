using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.lexer;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using FPL.inter;

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
       /* 运行时变量存储
        * 遇到每个作用域新建一个列表，用完销毁
        */
        static void Main(string[] args)
        {
            Compile("Program.fpl");
            //Console.ReadKey();
            Interprete();
            Console.ReadKey();
        }

        static void Compile(string args)
        {
            try
            {
                Lexer lex = new Lexer(new StreamReader(new FileStream(args, FileMode.Open)));
                Praser praser = new Praser(lex);

                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();
                praser.Compile();
                watch.Stop();

                FileStream fileStream = new FileStream("Program.fplc", FileMode.Create);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, praser);
                fileStream.Close();

                TimeSpan timespan = watch.Elapsed;
                Console.WriteLine("编译完成");
                Console.WriteLine("执行时间：{0}(毫秒)", timespan.TotalMilliseconds);
            }
            catch (CompileException) { }
        }

        static void Interprete()
        {
            Praser praser;

            FileStream fileStream = new FileStream("Program.fplc", FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            praser = (Praser)binaryFormatter.Deserialize(fileStream);

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            praser.Interprete();
            watch.Stop();
            TimeSpan timespan = watch.Elapsed;
            Console.WriteLine("执行完成");
            Console.WriteLine("执行时间：{0}(毫秒)", timespan.TotalMilliseconds);
        }
    }
}
