using System.Collections.Generic;
using System.IO;
using System.Linq;
using FPL.Parse;

namespace FPL.Generator
{
    public class FILGenerator
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

        #region Write

        public static CodingUnit Write(InstructionType type, int parameter = 0)
        {
            if (type == InstructionType.define) Line = -1;

            Code.Add(new CodingUnit(++Line, type, parameter));
            return Code.Last();
        }
        public static CodingUnit Write(string name, int lineNum = 0)
        {
            Code.Add(new CodingUnit(name, lineNum));
            return Code.Last();
        }
        public static CodingUnit Write(string type, string name)
        {
            Code.Add(new CodingUnit(type, name));
            return Code.Last();
        }
        public static CodingUnit Write(string name, int p1, int p2)
        {
            Code.Add(new CodingUnit(name, p1, p2));
            return Code.Last();
        }
        public static CodingUnit Write(int c)
        {
            Code.Add(new CodingUnit(c));
            return Code.Last();
        }


        #endregion

        public static void Back()
        {
            Code.RemoveAt(Code.Count - 1);
            Line--;
        }

        public static void WriteToFile(bool isDebug)
        {
            StreamWriter.WriteLine(0);
            StreamWriter.WriteLine(0);
            StreamWriter.WriteLine(Code[2].Parameter);
            int i;
            for (i = 3; i < Code.Count; i++)
            {
                if (Code[i].InsType == InstructionType.define) break;
                switch (Code[i].CodingUnitType)
                {
                    case CodingUnitType.Common:
                        StreamWriter.WriteLine((int)Code[i].InsType + " " + Code[i].Parameter);
                        break;
                    case CodingUnitType.NumOnly:
                        StreamWriter.WriteLine(Code[i].Parameter);
                        break;
                    case CodingUnitType.TwoString:
                        StreamWriter.WriteLine(Code[i].Type + " " + Code[i].Name);
                        break;
                    case CodingUnitType.StringANum:
                        StreamWriter.WriteLine(Code[i].Name + " " + Code[i].LineNum);
                        break;
                    case CodingUnitType.StringATwoNum:
                        StreamWriter.WriteLine(Code[i].Name + " " + Code[i].Parameter + " " + Code[i].Parameter2);
                        break;
                }
            }

            for (; i < Code.Count; i++)
            {
                if (Code[i].InsType == InstructionType.endF)
                {
                    StreamWriter.Write((int)Code[i].InsType + " " + Code[i].Parameter);
                    if (isDebug) StreamWriter.Write("    " + Code[i].InsType + " " + Code[i].Parameter);
                    break;
                }

                if (isDebug)
                {
                    StreamWriter.Write((int)Code[i].InsType + " " + Code[i].Parameter);
                    StreamWriter.WriteLine("    " + Code[i].InsType + " " + Code[i].Parameter);
                }
                else
                    StreamWriter.WriteLine((int)Code[i].InsType + " " + Code[i].Parameter);
            }

            StreamWriter.Flush();
            StreamWriter.Close();
            FileStream.Close();
        }

        public static int GetStackFrameSize()
        {
            if (Code.Last().InsType != InstructionType.funcEnd) return -1;
            int nowStack = 0;
            int maxStack = 0;
            for (int i = Code.Count - 1; i >= 0; i--)
            {
                if (Code[i].InsType == InstructionType.func) break;
                switch (Code[i].InsType)
                {
                    case InstructionType.pushloc:
                    case InstructionType.pusharg:
                    case InstructionType.pushfield:
                    case InstructionType.pushval:
                    case InstructionType.pushsta:
                    case InstructionType.newobjc:
                        nowStack++;
                        break;
                    case InstructionType.pop:
                    case InstructionType.storearg:
                    case InstructionType.storefield:
                    case InstructionType.storeloc:
                    case InstructionType.storesta:
                    case InstructionType.add_i:
                    case InstructionType.addf:
                    case InstructionType.add_c:
                    case InstructionType.sub_i:
                    case InstructionType.subf:
                    case InstructionType.sub_c:
                    case InstructionType.div_i:
                    case InstructionType.div_f:
                    case InstructionType.div_c:
                    case InstructionType.mul_i:
                    case InstructionType.mul_f:
                    case InstructionType.mul_c:
                    case InstructionType.eq:
                    case InstructionType.le:
                    case InstructionType.mo:
                    case InstructionType.jt:
                    case InstructionType.jf:
                        nowStack--;
                        if (nowStack < maxStack)
                            maxStack = nowStack;
                        break;
                }
            }

            return -maxStack + nowStack;
        }
    }

    internal enum CodingUnitType
    {
        Common,
        NumOnly,
        StringANum,
        TwoString,
        StringATwoNum,
    }

    public class CodingUnit
    {
        public InstructionType InsType = InstructionType.nop;
        public int LineNum;
        public string Name = "";
        public int Parameter;
        public int Parameter2;
        public string Type = "";
        internal CodingUnitType CodingUnitType;

        public CodingUnit(int line, InstructionType type, int parm = 0)
        {
            LineNum = line;
            InsType = type;
            Parameter = parm;
            CodingUnitType = CodingUnitType.Common;
        }

        public CodingUnit(string name, int lineNum)
        {
            LineNum = lineNum;
            Name = name;
            CodingUnitType = CodingUnitType.StringANum;
        }

        public CodingUnit(string type, string name)
        {
            Type = type;
            Name = name;
            CodingUnitType = CodingUnitType.TwoString;
        }

        public CodingUnit(int c)
        {
            Parameter = c;
            CodingUnitType = CodingUnitType.NumOnly;
        }

        public CodingUnit(string name, int p1, int p2)
        {
            Name = name;
            Parameter = p1;
            Parameter2 = p2;
            CodingUnitType = CodingUnitType.StringATwoNum;
        }

        public void Remove()
        {
            FILGenerator.Code.Remove(this);
        }
    }
}