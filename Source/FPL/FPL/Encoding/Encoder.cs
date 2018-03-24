using System.Collections.Generic;
using System.IO;
using FPL.Parse;

namespace FPL.Encoding
{
    public class Encoder
    {
        private static FileStream FileStream;
        private static StreamWriter StreamWriter;
        public static List<CodingUnit> Code = new List<CodingUnit>();
        public static int Line;

        public static void Init(string file)
        {
            FileStream = new FileStream(file, FileMode.Create);
            StreamWriter = new StreamWriter(FileStream);
        }

        public static CodingUnit Write(InstructionType type, int parameter = 0)
        {
            switch (type)
            {
                //case InstructionType.func:
                //    code.Add(new CodingUnit(0, type));
                //    return code[code.Count - 1];
                case InstructionType.define:
                    Line = -1;
                    break;
            }

            Code.Add(new CodingUnit(++Line, type, parameter));
            return Code[Code.Count - 1];
        }

        public static CodingUnit Write(InstructionType type, string parameter)
        {
            switch (type)
            {
                case InstructionType.define:
                    Line = -1;
                    break;
            }

            Code.Add(new CodingUnit(Line, type, parameter));
            return Code[Code.Count - 1];
        }

        public static CodingUnit Write(string name, int lineNum = 0)
        {
            Code.Add(new CodingUnit(name, lineNum));
            return Code[Code.Count - 1];
        }

        public static CodingUnit Write(string type, string name)
        {
            Code.Add(new CodingUnit(type, name));
            return Code[Code.Count - 1];
        }

        public static CodingUnit Write(int c)
        {
            Code.Add(new CodingUnit(c));
            return Code[Code.Count - 1];
        }

        public static void Back()
        {
            Code.RemoveAt(Code.Count - 1);
            Line--;
        }

        public static void WriteToFile()
        {
            StreamWriter.WriteLine(0);
            StreamWriter.WriteLine(0);
            StreamWriter.WriteLine(Code[2].parameter);
            int i;
            for (i = 3; i < Code.Count; i++)
            {
                if (Code[i].ins_type == InstructionType.define) break;
                if (Code[i].name == "")
                    StreamWriter.WriteLine(Code[i].parameter);
                else if (Code[i].type != "")
                    StreamWriter.WriteLine(Code[i].type + " " + Code[i].name);
                else
                    StreamWriter.WriteLine(Code[i].name + " " + Code[i].line_num);
                //if (code[i].ins_type == InstructionType.nop) streamWriter.WriteLine(/*(int)*/InstructionType.nop);
            }

            for (; i < Code.Count; i++)
            {
                if (Code[i].ins_type == InstructionType.endF)
                {
                    StreamWriter.Write(/*(int)*/ Code[i].ins_type + " " + Code[i].parameter);
                    continue;
                }

                StreamWriter.WriteLine(/*(int)*/ Code[i].ins_type + " " + Code[i].parameter);
            }

            StreamWriter.Flush();
            StreamWriter.Close();
            FileStream.Close();
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
            Encoder.Code.Remove(this);
        }
    }
}