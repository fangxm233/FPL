using System;
using System.Collections;
using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse;
using Type = FPL.symbols.Type;

namespace FPL.Parse
{
    public enum VarType
    {
        Arg,
        Field,
        Local,
        Static,
        Class,
        Unknown,
    }

    public enum FuncType
    {
        Member,
        Static,
        Constructor,
        InitFunction,
    }

    public class Parser
    {
        public static List<Hashtable> symbols_list = new List<Hashtable>();

        public static List<Hashtable> var_id = new List<Hashtable>();

        public static Dictionary<string, Class> classes = new Dictionary<string, Class>();
        public static Function analyzing_function;
        public static Class analyzing_class;
        public static Sentence analyzing_loop;

        static List<CodingUnit> functions_unit = new List<CodingUnit>();
        static List<CodingUnit> classes_unit = new List<CodingUnit>();

        public static Dictionary<int, int>[] symbol_priority = new Dictionary<int, int>[8];
        public static Dictionary<int, int> type_of_expr = new Dictionary<int, int>();
        public static Dictionary<int, Dictionary<string, InstructionType>> ins_table;

        public void Compile()
        {
            Fill_type_of_expr();
            Fill_symbol_priority();
            Fill_ins_table();
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
            foreach (var item in classes)
            {
                analyzing_class = item.Value;
                //foreach (var item1 in item.Value.FunctionCalls_e)
                //{
                //    item1.@class = item.Value;
                //}
                //foreach (var item1 in item.Value.FunctionCalls_s)
                //{
                //    item1.@class = item.Value;
                //}
                //foreach (var item1 in item.Value.Objects_e)
                //{
                //    item1.@class = item.Value;
                //}
                //foreach (var item1 in item.Value.Objects_s)
                //{
                //    item1.@class = item.Value;
                //}
                item.Value.width = item.Value.GetWidth();
                Type.AddType(new Type(item.Value.name, Tag.CLASS, item.Value.width));
                int s = 0;
                int static_count = 0;
                foreach (var item1 in item.Value.Statement)
                {
                    if(item1.varType == VarType.Static)
                    {
                        item1.ID = static_count++;
                        continue;
                    }
                    item1.ID = s++;
                }
                int f = 0;
                foreach (Function fun in item.Value.Functions)
                {
                    fun.ID = f++;
                }
                foreach (var item1 in item.Value.Statement)
                {
                    item1.Check();
                    if (item1.assign.right != null)
                        item.Value.init_function.AddSentence(item1);
                }

                analyzing_class = null;
            }
            foreach (var item in classes)
            {
                analyzing_class = item.Value;
                foreach (var item1 in item.Value.Functions)
                {
                    if (item1.tag == Tag.CONSTRUCTOR) continue;
                    if (item1.tag == Tag.INIT_FUNCTION) continue;
                    item1.return_type = Type.GetType(item1.type_name);
                }
                foreach (var item1 in item.Value.Functions)
                {
                    item1.Check();
                }
                analyzing_class = null;
            }
        }

        public void Code()
        {
            Encoder.Write(0);
            Encoder.Write(0);
            CodingUnit entrance_line = Encoder.Write(-1);
            CodingUnit static_count = Encoder.Write(0);
            foreach (var @class in classes)
            {
                foreach (Statement statement in @class.Value.Statement)
                {
                    if (statement.varType != VarType.Static) continue;
                    Encoder.Write(statement.assign.left.type.type_name, statement.name);
                    static_count.parameter++;
                }
            }
            foreach (var item in classes)
            {
                classes_unit.Add(Encoder.Write(item.Value.name));
                Encoder.Write(item.Value.width);
                foreach (Statement var in item.Value.Statement)
                {
                    Encoder.Write(var.assign.left.type.type_name, var.name);
                }
                foreach (Function func in item.Value.Functions)
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
                classes_unit[class_i++].line_num = Encoder.line;
                foreach (Function func in item.Value.Functions)
                {
                    Encoder.Write(InstructionType.func);
                    if (func.name == "Main"&& func.func_type == FuncType.Static)
                    {
                        entrance_line.parameter = Encoder.line + 1;
                        Encoder.Write(InstructionType.call, func.ID);
                    }
                    functions_unit[func_i++].line_num = Encoder.line + 1;
                    func.Code();
                    Encoder.Write(InstructionType.funcEnd);
                }
                Encoder.Write(InstructionType.classEnd);
            }
            Encoder.Write(InstructionType.endF);
            if (entrance_line.parameter == -1) Node.Error("未找到程序入口点");
        }

        public void CodeSecond()
        {
            foreach (var item in classes)
            {
                item.Value.CodeSecond();
            }
        }

        public void Fill_symbol_priority()
        {
            for (int i = 0; i < symbol_priority.Length; i++)
            {
                symbol_priority[i] = new Dictionary<int, int>();
            }
            symbol_priority[0].Add(Tag.LSQUBRACKETS, Tag.FACTOR);
            symbol_priority[0].Add(Tag.RSQUBRACKETS, Tag.FACTOR);
            symbol_priority[0].Add(Tag.LBRACKETS, Tag.FACTOR);
            symbol_priority[0].Add(Tag.RBRACKETS, Tag.FACTOR);
            symbol_priority[0].Add(Tag.DOT, Tag.BINATY);
            symbol_priority[1].Add(Tag.INCREASE, Tag.UNARY);
            symbol_priority[1].Add(Tag.DECLINE, Tag.UNARY);
            symbol_priority[1].Add(Tag.NOT, Tag.UNARY);
            symbol_priority[2].Add(Tag.DIVIDE, Tag.BINATY);
            symbol_priority[2].Add(Tag.MULTIPLY, Tag.BINATY);
            symbol_priority[2].Add(Tag.MODULO, Tag.UNARY);
            symbol_priority[3].Add(Tag.PLUS, Tag.BINATY);
            symbol_priority[3].Add(Tag.MINUS, Tag.BINATY);
            symbol_priority[4].Add(Tag.MORE, Tag.BOOL);
            symbol_priority[4].Add(Tag.LESS, Tag.BOOL);
            symbol_priority[4].Add(Tag.LE, Tag.BOOL);
            symbol_priority[4].Add(Tag.GE, Tag.BOOL);
            symbol_priority[5].Add(Tag.EQ, Tag.BOOL);
            symbol_priority[5].Add(Tag.NE, Tag.BOOL);
            symbol_priority[6].Add(Tag.AND, Tag.BOOL);
            symbol_priority[6].Add(Tag.OR, Tag.BOOL);
            symbol_priority[7].Add(Tag.NUM, Tag.FACTOR);
            symbol_priority[7].Add(Tag.REAL, Tag.FACTOR);
            symbol_priority[7].Add(Tag.TRUE, Tag.FACTOR);
            symbol_priority[7].Add(Tag.FALSE, Tag.FACTOR);
            symbol_priority[7].Add(Tag.STR, Tag.FACTOR);
            symbol_priority[7].Add(Tag.NEW, Tag.FACTOR);
            symbol_priority[7].Add(Tag.ID, Tag.FACTOR);
        }
        public void Fill_type_of_expr()
        {
            type_of_expr.Add(Tag.LSQUBRACKETS, Tag.FACTOR);
            type_of_expr.Add(Tag.RSQUBRACKETS, Tag.FACTOR);
            type_of_expr.Add(Tag.LBRACKETS, Tag.FACTOR);
            type_of_expr.Add(Tag.RBRACKETS, Tag.FACTOR);
            type_of_expr.Add(Tag.DOT, Tag.BINATY);
            type_of_expr.Add(Tag.INCREASE, Tag.UNARY);
            type_of_expr.Add(Tag.DECLINE, Tag.UNARY);
            type_of_expr.Add(Tag.NOT, Tag.UNARY);
            type_of_expr.Add(Tag.DIVIDE, Tag.BINATY);
            type_of_expr.Add(Tag.MULTIPLY, Tag.BINATY);
            type_of_expr.Add(Tag.MODULO, Tag.UNARY);
            type_of_expr.Add(Tag.PLUS, Tag.BINATY);
            type_of_expr.Add(Tag.MINUS, Tag.BINATY);
            type_of_expr.Add(Tag.MORE, Tag.BOOL);
            type_of_expr.Add(Tag.LESS, Tag.BOOL);
            type_of_expr.Add(Tag.LE, Tag.BOOL);
            type_of_expr.Add(Tag.GE, Tag.BOOL);
            type_of_expr.Add(Tag.EQ, Tag.BOOL);
            type_of_expr.Add(Tag.NE, Tag.BOOL);
            type_of_expr.Add(Tag.AND, Tag.BOOL);
            type_of_expr.Add(Tag.OR, Tag.BOOL);
            type_of_expr.Add(Tag.NUM, Tag.FACTOR);
            type_of_expr.Add(Tag.REAL, Tag.FACTOR);
            type_of_expr.Add(Tag.TRUE, Tag.FACTOR);
            type_of_expr.Add(Tag.FALSE, Tag.FACTOR);
            type_of_expr.Add(Tag.STR, Tag.FACTOR);
            type_of_expr.Add(Tag.NEW, Tag.FACTOR);
            type_of_expr.Add(Tag.ID, Tag.FACTOR);
        }
        public void Fill_ins_table()
        {
            ins_table = new Dictionary<int, Dictionary<string, InstructionType>>();
            Dictionary<string, InstructionType> dictionary = new Dictionary<string, InstructionType>
            {
                { "int", InstructionType.add_i },
                { "float", InstructionType.add_f },
                { "char", InstructionType.add_c }
            };
            ins_table.Add(Tag.PLUS, dictionary);
            dictionary = new Dictionary<string, InstructionType>
            {
                { "int", InstructionType.sub_i },
                { "float", InstructionType.sub_f },
                { "char", InstructionType.sub_c }
            };
            ins_table.Add(Tag.MINUS, dictionary);
            dictionary = new Dictionary<string, InstructionType>
            {
                { "int", InstructionType.mul_i },
                { "float", InstructionType.mul_f },
                { "char", InstructionType.mul_c }
            };
            ins_table.Add(Tag.MULTIPLY, dictionary);
            dictionary = new Dictionary<string, InstructionType>
            {
                { "int", InstructionType.div_i },
                { "float", InstructionType.div_f },
                { "char", InstructionType.div_c }
            };
            ins_table.Add(Tag.DIVIDE, dictionary);
            //TODO:添加取余
        }
    }

    public class CompileException : Exception { }
    public class CodeException : Exception { }
}
