using System.Collections.Generic;
using System.IO;
using FPL.Parse;

namespace FPL.Encoding
{
    public class Encoder
    {
        private static FileStream fileStream;
        private static StreamWriter streamWriter;
        public static List<CodingUnit> code = new List<CodingUnit>();
        public static int line;

        public static void Init(string file)
        {
            fileStream = new FileStream(file, FileMode.Create);
            streamWriter = new StreamWriter(fileStream);
        }

        public static CodingUnit Write(InstructionType type, int parameter = 0)
        {
            switch (type)
            {
                //case InstructionType.func:
                //    code.Add(new CodingUnit(0, type));
                //    return code[code.Count - 1];
                case InstructionType.define:
                    line = -1;
                    break;
            }

            code.Add(new CodingUnit(++line, type, parameter));
            return code[code.Count - 1];
        }

        public static CodingUnit Write(InstructionType type, string parameter)
        {
            switch (type)
            {
                case InstructionType.define:
                    line = -1;
                    break;
            }

            code.Add(new CodingUnit(line, type, parameter));
            return code[code.Count - 1];
        }

        public static CodingUnit Write(string name, int lineNum = 0)
        {
            code.Add(new CodingUnit(name, lineNum));
            return code[code.Count - 1];
        }

        public static CodingUnit Write(string type, string name)
        {
            code.Add(new CodingUnit(type, name));
            return code[code.Count - 1];
        }

        public static CodingUnit Write(int c)
        {
            code.Add(new CodingUnit(c));
            return code[code.Count - 1];
        }

        public static void Back()
        {
            code.RemoveAt(code.Count - 1);
            line--;
        }

        public static void WriteToFile()
        {
            streamWriter.WriteLine(0);
            streamWriter.WriteLine(0);
            streamWriter.WriteLine(code[2].parameter);
            int i;
            for (i = 3; i < code.Count; i++)
            {
                if (code[i].ins_type == InstructionType.define) break;
                if (code[i].name == "")
                    streamWriter.WriteLine(code[i].parameter);
                else if (code[i].type != "")
                    streamWriter.WriteLine(code[i].type + " " + code[i].name);
                else
                    streamWriter.WriteLine(code[i].name + " " + code[i].line_num);
                //if (code[i].ins_type == InstructionType.nop) streamWriter.WriteLine(/*(int)*/InstructionType.nop);
            }

            for (; i < code.Count; i++)
            {
                if (code[i].ins_type == InstructionType.endF)
                {
                    streamWriter.Write((int) code[i].ins_type + " " + code[i].parameter);
                    continue;
                }

                streamWriter.WriteLine((int) code[i].ins_type + " " + code[i].parameter);
            }

            streamWriter.Flush();
            streamWriter.Close();
            fileStream.Close();
        }
    }

    public class CodingUnit
    {
        public InstructionType ins_type = InstructionType.nop;
        public int line_num;
        public string name = "";
        public int parameter;
        public string type = "";

        public CodingUnit(int line, InstructionType type, int parm = 0)
        {
            line_num = line;
            ins_type = type;
            parameter = parm;
        }

        public CodingUnit(int line, InstructionType type, string parm)
        {
            line_num = line;
            ins_type = type;
            name = parm;
        }

        public CodingUnit(string name, int lineNum)
        {
            line_num = lineNum;
            this.name = name;
        }

        public CodingUnit(string type, string name)
        {
            this.type = type;
            this.name = name;
        }

        public CodingUnit(int c)
        {
            parameter = c;
        }

        public void Remove()
        {
            Encoder.code.Remove(this);
        }
    }
}