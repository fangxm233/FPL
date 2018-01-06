﻿using System;
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
            if (Compile("Program.fpl"))
                Interprete();
            Console.ReadKey();
        }

        static bool Compile(string args)
        {
            try
            {
                System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
                watch.Start();

                Lexer.Analysis(new StreamReader(new FileStream(args, FileMode.Open)));
                Parser praser = new Parser();
                praser.Compile();

                watch.Stop();

                //二进制存储
                FileStream fileStream = new FileStream("Program.fplc", FileMode.Create);
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, praser);
                fileStream.Close();

                TimeSpan timespan = watch.Elapsed;
                Console.WriteLine("编译完成");
                Console.WriteLine("执行时间：{0}(毫秒)", timespan.TotalMilliseconds);
                Console.WriteLine("");
                return true;
            }
            catch (CompileException) { return false; }
        }

        static void Interprete()
        {
            Parser parser;

            FileStream fileStream = new FileStream("Program.fplc", FileMode.Open);
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            parser = (Parser)binaryFormatter.Deserialize(fileStream);

            System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
            watch.Start();
            //try
            //{
                try
                {
                    parser.Interprete();
                }
                catch (RunTimeException){}
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
            watch.Stop();
            TimeSpan timespan = watch.Elapsed;
            Console.WriteLine("");
            Console.WriteLine("执行完成");
            Console.WriteLine("执行时间：{0}(毫秒)", timespan.TotalMilliseconds);
        }
    }
}
