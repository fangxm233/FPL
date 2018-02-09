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
    [Serializable]
    public class Parser
    {
        [NonSerialized]
        public static List<Hashtable> symbols_list = new List<Hashtable>();

        [NonSerialized]
        public static List<Hashtable> var_id = new List<Hashtable>();

        public static Dictionary<string, Function> functions = new Dictionary<string, Function>();
        public static Function analyzing_function;
        public static Sentence analyzing_loop;
        //[NonSerialized]
        //public static bool is_runtime;

        public void Compile()
        {
            new Sentence(1).Buildsstart();
            Check();
        }

        public void Check()
        {
            foreach (var item in functions)
            {
                item.Value.Check();
            }
        }

        public void Code()
        {
            List<CodingUnit> functions_unit = new List<CodingUnit>();
            int i = 0;
            foreach (var item in functions)
            {
                item.Value.id = i;
                i++;
                functions_unit.Add(Encoder.Write(InstructionsType.func));
            }
            if(!functions.ContainsKey("Main")) Sentence.Error("未找到主函数");
            functions["Main"].Code();
            foreach (var item in functions)
            {
                if (item.Key != "Main")
                    item.Value.Code();
            }
            foreach (var item in functions)
            {
                item.Value.CodeSecond();
            }
            i = 0;
            foreach (var item in functions)
            {
                CodingUnit u = functions_unit[i];
                u.parameter = item.Value.head_line;
                i++;
            }
        }

        public void CodeSecond()
        {
            foreach (var item in functions)
            {
                item.Value.CodeSecond();
            }
        }
    }

    [Serializable]
    public class CompileException : Exception { }
    [Serializable]
    public class CodeException : Exception { }
}
