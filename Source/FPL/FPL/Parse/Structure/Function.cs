using System.Collections.Generic;
using System.Linq;
using FPL.Classification;
using FPL.DataStorager;
using FPL.Generator;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Sentences;
using FPL.Parse.Sentences.ProcessControl;

namespace FPL.Parse.Structure
{
    public class Function : Sentence
    {
        public Class Class;
        public FuncType FuncType;
        public int HeadLine;
        public int ID;
        public string Name;
        public List<Object_s> objects_s = new List<Object_s>();
        public List<Parameter> Parameters = new List<Parameter>();
        public Type ReturnType;
        private List<Sentence> Sentences = new List<Sentence>();
        public List<Statement> Statements = new List<Statement>();
        public string TypeName;

        public Function(FuncType type, int tag) : base(tag)
        {
            FuncType = type;
            TypeName = Lexer.NextToken.ToString();
            Lexer.Next();
            if(type == FuncType.OperatorFunc)Lexer.Next();
            if (Lexer.NextToken.tag != Tag.ID && type != FuncType.OperatorFunc)
                ErrorSta(LogContent.SthUseless, Lexer.NextToken);
            Name = Lexer.NextToken.ToString();
        }

        public Function(FuncType type, int tag, string name) : base(tag)
        {
            FuncType = type;
            Name = name;
        }

        public Function(FuncType type, Type returnType, int tag) : base(tag)
        {
            FuncType = type;
            ReturnType = returnType;
            if (Lexer.NextToken.tag == Tag.ID)
                Name = Lexer.NextToken.ToString();
            else ErrorSta(LogContent.SthUseless, Lexer.NextToken);
        }

        public Function(FuncType type, Type returnType, string name, int tag) : base(tag)
        {
            FuncType = type;
            Class = GetClass(name);
            ReturnType = returnType;
            Name = name;
        }

        public override Sentence Build()
        {
            Class = Parser.AnalyzingClass;
            Parser.AnalyzingClass.AddFunction(Name, this);
            NewScope();
            Parser.AnalyzingFunction = this;
            Match("(");
            Lexer.Next();
            while (true)
            {
                if (Lexer.NextToken.tag == Tag.RBRACKETS) break;
                Parameters.Add(new Parameter((Statement) new Statement(VarType.Arg, Tag.STATEMENT).Build()));
                if (Lexer.NextToken.tag == Tag.COMMA)
                    Lexer.Next();
                else
                    break;
            }

            Match(")", false);
            Match("{");
            Sentences = BuildMethod();
            Match("}", false);
            DestroyScope();
            Parser.AnalyzingFunction = null;
            return this;
        }

        public override void Check()
        {
            if (tag == Tag.INIT_FUNCTION) return;
            if (tag == Tag.CONSTRUCTOR && Sentences.Count == 0)
            {
                ReturnType = Type.Void;
                AddSentence(new Return(Tag.RETURN));
                return;
            }

            Parser.AnalyzingFunction = this;
            if (tag == Tag.OPERATORFUNC) CheckOpeFunc();
            if (Sentences.Count == 0)
                if (ReturnType != Type.Void)
                {
                    Error(LogContent.NotAllPathHaveReturnValue);
                }
                else
                {
                    Sentences.Add(new Return(Tag.RETURN));
                    ((Return) Sentences[Sentences.Count - 1]).Class = Class;
                }

            if (Sentences.Last().tag != Tag.RETURN) //检查函数返回
            {
                if (ReturnType != Type.Void) Error(LogContent.NotAllPathHaveReturnValue);
                Sentences.Add(new Return(Tag.RETURN));
                ((Return) Sentences[Sentences.Count - 1]).Class = Class;
            }

            if (Parameters.Count != 0 && Name == "Main") Error(LogContent.HaveParmUnallowed);
            //foreach (Parameter item in Parameters) item.Check();
            foreach (Sentence item in Sentences) item.Check();
            Parser.AnalyzingFunction = null;
        }

        public void CheckOpeFunc()
        {
            if (Parameters.Count != Classifier.ClassificateIn(ClassificateMethod.ReloadOpeNeedParmCount, Name))
                Error(LogContent.OverloadSthNeedParmCount, Name,
                    Classifier.ClassificateIn(ClassificateMethod.ReloadOpeNeedParmCount, Name));
            bool s = false;
            foreach (Parameter parameter in Parameters)
            {
                if (parameter.Type.type_name == Class.Name) s = true;
            }

            if (s == false) Error(LogContent.MustBeInchudeType);
            switch (Name)
            {
                case "==":
                    if (!Class.ContainsFunction("!=", Parameters))
                        Error(LogContent.NeedMatchOverload, "==", "!=");
                    break;
                case "!=":
                    if (!Class.ContainsFunction("==", Parameters))
                        Error(LogContent.NeedMatchOverload, "!=", "==");
                    break;
                case ">=":
                    if (!Class.ContainsFunction(">=", Parameters))
                        Error(LogContent.NeedMatchOverload, ">=", "<=");
                    break;
                case "<=":
                    if (!Class.ContainsFunction("<=", Parameters))
                        Error(LogContent.NeedMatchOverload, "<=", ">=");
                    break;
                case "+":
                case "-":
                case "*":
                case "/":
                case "!":
                    break;
                default:
                    Error(LogContent.ExpectOverloadableOperator);
                    break;
            }
        }

        public override void Code()
        {
            Parser.AnalyzingFunction = this;
            if (tag == Tag.INIT_FUNCTION)
            {
                ReturnType = Type.Void;
                AddSentence(new Return(Tag.RETURN));
            }

            HeadLine = FILGenerator.Line + 1;
            foreach (Object_s item in objects_s) item.IsHead = false;

            for (int i = 1; i < Parameters.Count + (FuncType == FuncType.Static ? 0 : 1); i++)
                Parameters[i - (FuncType == FuncType.Static ? 0 : 1)].Statement.ID = i;

            for (int i = Parameters.Count + 1;
                i < Statements.Count + Parameters.Count + (FuncType == FuncType.Static ? 0 : 1);
                i++)
                Statements[i - Parameters.Count - (FuncType == FuncType.Static ? 0 : 1)].ID = i;

            for (int i = 0; i < Statements.Count - Parameters.Count; i++) FILGenerator.Write(InstructionType.pushval);

            foreach (Sentence item in Sentences)
            {
                if (item.tag == Tag.RETURN && tag == Tag.INIT_FUNCTION || tag == Tag.CONSTRUCTOR)
                {
                    FILGenerator.Write(InstructionType.ret);
                    continue;
                }

                item.Code();
            }

            Parser.AnalyzingFunction = null;
            //if (name == "Main") Encoder.Write(InstructionsType.endP);
        }

        public override void CodeSecond()
        {
            foreach (Sentence item in Sentences) item.CodeSecond();
        }

        public Statement GetStatement(string name)
        {
            foreach (Statement item in Statements)
                if (item.Name == name)
                    return item;
            return null;
        }

        public Type GetTypeByLocalName(string name)
        {
            foreach (Statement item in Statements)
                if (item.Name == name)
                    return item.Assign.Type;
            return null;
        }

        public void AddSentence(Sentence sentence)
        {
            Sentences.Add(sentence);
        }
    }
}