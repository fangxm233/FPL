using System;
using System.Collections;
using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Sentences;
using FPL.Parse.Structure;
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
        Unknown
    }

    public enum FuncType
    {
        Member,
        Static,
        Constructor,
        InitFunction
    }

    public class Parser
    {
        public static List<Hashtable> SymbolsList = new List<Hashtable>();

        public static List<Hashtable> VarId = new List<Hashtable>();

        public static Dictionary<string, Class> Classes = new Dictionary<string, Class>();
        public static Function AnalyzingFunction;
        public static Class AnalyzingClass;
        public static Sentence AnalyzingLoop;

        private static readonly List<CodingUnit> FunctionsUnit = new List<CodingUnit>();
        private static readonly List<CodingUnit> ClassesUnit = new List<CodingUnit>();

        public static Dictionary<int, int>[] SymbolPriority = new Dictionary<int, int>[8];
        public static Dictionary<int, int> TypeOfExpr = new Dictionary<int, int>();
        public static Dictionary<int, Dictionary<string, InstructionType>> InsTable;

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
                foreach (var item in Classes) item.Value.ID = i++;
            }
            foreach (var item in Classes)
            {
                AnalyzingClass = item.Value;
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
                item.Value.Width = item.Value.GetWidth();
                Type.AddType(new Type(item.Value.Name, Tag.CLASS, item.Value.Width));
                int s = 0;
                int static_count = 0;
                foreach (Statement item1 in item.Value.Statement)
                {
                    if (item1.VarType == VarType.Static)
                    {
                        item1.ID = static_count++;
                        continue;
                    }

                    item1.ID = s++;
                }

                int f = 0;
                foreach (Function fun in item.Value.Functions) fun.ID = f++;
                foreach (Statement item1 in item.Value.Statement)
                {
                    item1.Check();
                    if (item1.Assign.Right != null)
                        item.Value.InitFunction.AddSentence(item1);
                }

                AnalyzingClass = null;
            }

            foreach (var item in Classes)
            {
                AnalyzingClass = item.Value;
                foreach (Function item1 in item.Value.Functions)
                {
                    if (item1.tag == Tag.CONSTRUCTOR) continue;
                    if (item1.tag == Tag.INIT_FUNCTION) continue;
                    item1.ReturnType = Type.GetType(item1.TypeName);
                }

                foreach (Function item1 in item.Value.Functions) item1.Check();
                AnalyzingClass = null;
            }
        }

        public void Code()
        {
            Encoder.Write(0);
            Encoder.Write(0);
            CodingUnit entrance_line = Encoder.Write(-1);
            CodingUnit static_count = Encoder.Write(0);
            foreach (var @class in Classes)
            foreach (Statement statement in @class.Value.Statement)
            {
                if (statement.VarType != VarType.Static) continue;
                Encoder.Write(statement.Assign.Left.Type.type_name, statement.Name);
                static_count.parameter++;
            }

            foreach (var item in Classes)
            {
                ClassesUnit.Add(Encoder.Write(item.Value.Name));
                Encoder.Write(item.Value.Width);
                foreach (Statement var in item.Value.Statement) Encoder.Write(var.Assign.Left.Type.type_name, var.Name);
                foreach (Function func in item.Value.Functions) FunctionsUnit.Add(Encoder.Write(func.Name));
                Encoder.Write(InstructionType.nop);
            }

            Encoder.Write(InstructionType.define);
            int class_i = 0;
            int func_i = 0;
            foreach (var item in Classes)
            {
                Encoder.Write(InstructionType.@class);
                ClassesUnit[class_i++].line_num = Encoder.Line;
                foreach (Function func in item.Value.Functions)
                {
                    Encoder.Write(InstructionType.func);
                    if (func.Name == "Main" && func.FuncType == FuncType.Static)
                    {
                        entrance_line.parameter = Encoder.Line + 1;
                        Encoder.Write(InstructionType.call, func.ID);
                    }

                    FunctionsUnit[func_i++].line_num = Encoder.Line + 1;
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
            foreach (var item in Classes) item.Value.CodeSecond();
        }

        public void Fill_symbol_priority()
        {
            for (int i = 0; i < SymbolPriority.Length; i++) SymbolPriority[i] = new Dictionary<int, int>();
            SymbolPriority[0].Add(Tag.LSQUBRACKETS, Tag.FACTOR);
            SymbolPriority[0].Add(Tag.RSQUBRACKETS, Tag.FACTOR);
            SymbolPriority[0].Add(Tag.LBRACKETS, Tag.FACTOR);
            SymbolPriority[0].Add(Tag.RBRACKETS, Tag.FACTOR);
            SymbolPriority[0].Add(Tag.DOT, Tag.BINATY);
            SymbolPriority[1].Add(Tag.INCREASE, Tag.UNARY);
            SymbolPriority[1].Add(Tag.DECLINE, Tag.UNARY);
            SymbolPriority[1].Add(Tag.NOT, Tag.UNARY);
            SymbolPriority[2].Add(Tag.DIVIDE, Tag.BINATY);
            SymbolPriority[2].Add(Tag.MULTIPLY, Tag.BINATY);
            SymbolPriority[2].Add(Tag.MODULO, Tag.UNARY);
            SymbolPriority[3].Add(Tag.PLUS, Tag.BINATY);
            SymbolPriority[3].Add(Tag.MINUS, Tag.BINATY);
            SymbolPriority[4].Add(Tag.MORE, Tag.BOOL);
            SymbolPriority[4].Add(Tag.LESS, Tag.BOOL);
            SymbolPriority[4].Add(Tag.LE, Tag.BOOL);
            SymbolPriority[4].Add(Tag.GE, Tag.BOOL);
            SymbolPriority[5].Add(Tag.EQ, Tag.BOOL);
            SymbolPriority[5].Add(Tag.NE, Tag.BOOL);
            SymbolPriority[6].Add(Tag.AND, Tag.BOOL);
            SymbolPriority[6].Add(Tag.OR, Tag.BOOL);
            SymbolPriority[7].Add(Tag.NUM, Tag.FACTOR);
            SymbolPriority[7].Add(Tag.REAL, Tag.FACTOR);
            SymbolPriority[7].Add(Tag.TRUE, Tag.FACTOR);
            SymbolPriority[7].Add(Tag.FALSE, Tag.FACTOR);
            SymbolPriority[7].Add(Tag.STR, Tag.FACTOR);
            SymbolPriority[7].Add(Tag.NEW, Tag.FACTOR);
            SymbolPriority[7].Add(Tag.ID, Tag.FACTOR);
        }

        public void Fill_type_of_expr()
        {
            TypeOfExpr.Add(Tag.LSQUBRACKETS, Tag.FACTOR);
            TypeOfExpr.Add(Tag.RSQUBRACKETS, Tag.FACTOR);
            TypeOfExpr.Add(Tag.LBRACKETS, Tag.FACTOR);
            TypeOfExpr.Add(Tag.RBRACKETS, Tag.FACTOR);
            TypeOfExpr.Add(Tag.DOT, Tag.BINATY);
            TypeOfExpr.Add(Tag.INCREASE, Tag.UNARY);
            TypeOfExpr.Add(Tag.DECLINE, Tag.UNARY);
            TypeOfExpr.Add(Tag.NOT, Tag.UNARY);
            TypeOfExpr.Add(Tag.DIVIDE, Tag.BINATY);
            TypeOfExpr.Add(Tag.MULTIPLY, Tag.BINATY);
            TypeOfExpr.Add(Tag.MODULO, Tag.UNARY);
            TypeOfExpr.Add(Tag.PLUS, Tag.BINATY);
            TypeOfExpr.Add(Tag.MINUS, Tag.BINATY);
            TypeOfExpr.Add(Tag.MORE, Tag.BOOL);
            TypeOfExpr.Add(Tag.LESS, Tag.BOOL);
            TypeOfExpr.Add(Tag.LE, Tag.BOOL);
            TypeOfExpr.Add(Tag.GE, Tag.BOOL);
            TypeOfExpr.Add(Tag.EQ, Tag.BOOL);
            TypeOfExpr.Add(Tag.NE, Tag.BOOL);
            TypeOfExpr.Add(Tag.AND, Tag.BOOL);
            TypeOfExpr.Add(Tag.OR, Tag.BOOL);
            TypeOfExpr.Add(Tag.NUM, Tag.FACTOR);
            TypeOfExpr.Add(Tag.REAL, Tag.FACTOR);
            TypeOfExpr.Add(Tag.TRUE, Tag.FACTOR);
            TypeOfExpr.Add(Tag.FALSE, Tag.FACTOR);
            TypeOfExpr.Add(Tag.STR, Tag.FACTOR);
            TypeOfExpr.Add(Tag.NEW, Tag.FACTOR);
            TypeOfExpr.Add(Tag.ID, Tag.FACTOR);
        }

        public void Fill_ins_table()
        {
            InsTable = new Dictionary<int, Dictionary<string, InstructionType>>();
            var dictionary = new Dictionary<string, InstructionType>
            {
                {"int", InstructionType.add_i},
                {"float", InstructionType.add_f},
                {"char", InstructionType.add_c}
            };
            InsTable.Add(Tag.PLUS, dictionary);
            dictionary = new Dictionary<string, InstructionType>
            {
                {"int", InstructionType.sub_i},
                {"float", InstructionType.sub_f},
                {"char", InstructionType.sub_c}
            };
            InsTable.Add(Tag.MINUS, dictionary);
            dictionary = new Dictionary<string, InstructionType>
            {
                {"int", InstructionType.mul_i},
                {"float", InstructionType.mul_f},
                {"char", InstructionType.mul_c}
            };
            InsTable.Add(Tag.MULTIPLY, dictionary);
            dictionary = new Dictionary<string, InstructionType>
            {
                {"int", InstructionType.div_i},
                {"float", InstructionType.div_f},
                {"char", InstructionType.div_c}
            };
            InsTable.Add(Tag.DIVIDE, dictionary);
            //TODO:添加取余
        }
    }

    public class CompileException : Exception
    {
    }

    public class CodeException : Exception
    {
    }
}