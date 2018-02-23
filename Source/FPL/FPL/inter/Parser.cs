using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;
using System.Collections;
using FPL.Encoding;

namespace FPL.inter
{
    public class Parser
    {
        [NonSerialized]
        public static List<Hashtable> symbols_list = new List<Hashtable>();

        [NonSerialized]
        public static List<Hashtable> var_id = new List<Hashtable>();

        public static Dictionary<string, Class> classes = new Dictionary<string, Class>();
        public static Function analyzing_function;
        public static Class analyzing_class;
        public static Sentence analyzing_loop;

        static List<CodingUnit> functions_unit = new List<CodingUnit>();
        static List<CodingUnit> classes_unit = new List<CodingUnit>();

        public void Compile()
        {
            new Sentence(1).BuildStsrt();
            Check();
        }

        public void Check()
        {
            {
                int i = 5;
                foreach (var item in classes)
                {
                    item.Value.ID = i++;
                }
            }
            int f = 0;
            foreach (var item in classes)
            {
                foreach (var item1 in item.Value.FunctionCalls_e)
                {
                    item1.@class = item.Value;
                }
                foreach (var item1 in item.Value.FunctionCalls_s)
                {
                    item1.@class = item.Value;
                }
                foreach (var item1 in item.Value.Objects_e)
                {
                    item1.@class = item.Value;
                }
                foreach (var item1 in item.Value.Objects_s)
                {
                    item1.@class = item.Value;
                }
                item.Value.width = item.Value.GetWidth();
                symbols.Type.AddType(new symbols.Type(item.Value.name, Tag.CLASS, item.Value.width));
                int s = 0;
                foreach (var item1 in item.Value.Statement)
                {
                    item1.ID = s++;
                }
                for (int i = 0; i < item.Value.Functions.Count; i++)
                {
                    item.Value.Functions[i].ID = f++;
                }
                foreach (var item1 in item.Value.Statement)
                {
                    item1.Check();
                    if (item1.assign.right != null)
                        item.Value.init_function.AddSentence(item1);
                }
            }
            foreach (var item in classes)
            {
                foreach (var item1 in item.Value.Functions)
                {
                    if (item1.tag == Tag.CONSTRUCTOR) continue;
                    if (item1.tag == Tag.INIT_FUNCTION) continue;
                    item1.return_type = symbols.Type.GetType(item1.type_name);
                }
                foreach (var item1 in item.Value.Functions)
                {
                    item1.Check();
                }
            }
        }

        public void Code()
        {
            Encoder.Write(0);
            Encoder.Write(0);
            CodingUnit entrance_line = Encoder.Write(0);
            entrance_line.parameter = -1;
            foreach (var item in classes)
            {
                classes_unit.Add(Encoder.Write(item.Value.name));
                Encoder.Write(item.Value.width);
                foreach (var var in item.Value.Statement)
                {
                    Encoder.Write(var.assign.left.type.type_name, var.name);
                }
                foreach (var func in item.Value.Functions)
                {
                    functions_unit.Add(Encoder.Write(func.name));
                }
                Encoder.Write(InstructionType.nop);
            }
            Encoder.Write(InstructionType.define);
            int class_i = 0;
            int func_i = 0;
            foreach (var item in classes)
            {
                Encoder.Write(InstructionType.@class);
                classes_unit[class_i++].lineNum = Encoder.line;
                foreach (var func in item.Value.Functions)
                {
                    Encoder.Write(InstructionType.func);
                    if (func.name == "Main")
                    {
                        entrance_line.parameter = Encoder.line + 1;
                        Encoder.Write(InstructionType.newobjc, item.Value.ID);
                        Encoder.Write(InstructionType.call, item.Value.GetFunction(".init").ID);
                        Encoder.Write(InstructionType.pushval);
                        Encoder.Write(InstructionType.pushval);
                        Encoder.Write(InstructionType.pushval);
                    }
                    functions_unit[func_i++].lineNum = Encoder.line + 1;
                    func.Code();
                    Encoder.Write(InstructionType.funcEnd);
                }
                Encoder.Write(InstructionType.classEnd);
            }
            Encoder.Write(InstructionType.endF);
        }

        public void CodeSecond()
        {
            foreach (var item in classes)
            {
                item.Value.CodeSecond();
            }
        }
    }

    public enum VarType
    {
        arg,
        field,
        local,
    }

    public class CompileException : Exception { }
    public class CodeException : Exception { }
}
