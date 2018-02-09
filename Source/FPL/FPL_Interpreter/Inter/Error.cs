using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPL_Interpreter
{
    class OutPut
    {
        public static void FileError(string s)
        {
            Console.WriteLine("FileError： {0}", s);
            throw new FileException();
        }

        public static void RunTimeError(string s)
        {
            Console.WriteLine("RunTimeError： {0}", s);
            throw new RunTimeException();
        }

        public static void RunTimeWarning(string s)
        {
            Console.WriteLine("RunTimeWarning： {0}", s);
        }
    }
    [Serializable]
    public class FileException : Exception { }
    [Serializable]
    public class RunTimeException : Exception { }
}
