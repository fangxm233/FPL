using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FPL.inter;
using System.IO;

namespace FPL.Encoding
{
    public class Encoder
    {
        static FileStream fileStream;
        static StreamWriter streamWriter;
        public static List<CodingUnit> code = new List<CodingUnit>();
        public static int line = 0;

        public static void Init(string file)
        {
            fileStream = new FileStream(file, FileMode.Create);
            streamWriter = new StreamWriter(fileStream);
        }

        public static CodingUnit Write(InstructionsType type, int parameter = 0)
        {
            switch (type)
            {
                case InstructionsType.func:
                    code.Add(new CodingUnit(0, type));
                    return code[code.Count - 1];
            }
            code.Add(new CodingUnit(++line, type, parameter));
            return code[code.Count - 1];
        }

        public static void WriteToFile()
        {
            foreach (var item in code)
            {
                streamWriter.WriteLine((int)item.type + " " + item.parameter);
            }
            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();
        }
    }

    public class CodingUnit
    {
        public int lineNum;
        public InstructionsType type;
        public int parameter;
        public CodingUnit(int line,InstructionsType type,int parm = 0)
        {
            lineNum = line;
            this.type = type;
            parameter = parm;
        }
        public void Remove()
        {
            Encoder.code.Remove(this);
        }
    }
}
