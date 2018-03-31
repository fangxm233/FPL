using FPL.Generator;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Sentences.ProcessControl;
using FPL.Parse.Structure;
using FPL.symbols;

namespace FPL.Parse.Sentences
{
    public class Object_s : Sentence
    {
        public Class Class;
        private int ID;
        public bool IsHead = true;
        public string Name;
        private Sentence Next;
        public Statement Statement;
        public Type Type;
        public VarType VarType;

        public Object_s(int tag) : base(tag)
        {
            Name = ((Word) Lexer.NextToken).Lexeme;
        }

        public override Sentence Build()
        {
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.DOT)
            {
                //Error("只有赋值，函数调用，new 对象表达式可作为语句");
                Lexer.Back();
                return this;
            }

            Next = BuildNext();
            return this;
        }

        public override void Check()
        {
            if (Class == null && Statement != null)
            {
                Class = Statement.Class;
                IsHead = true;
            }
            else if (Statement != null)
            {
                Statement = Class.GetStatement(Name);
            }
            else
            {
                if (Type.GetType(Name) == null)
                {
                    if (Class == null)
                        Class = Parser.AnalyzingClass;
                    else if (Parser.AnalyzingFunction.FuncType != FuncType.Static)
                        Statement = Class.GetStatement(Name);
                }
                else
                {
                    Class = GetClass(Name);
                    VarType = VarType.Class;
                    return;
                }
            }

            VarType = Statement.VarType;
            if (Parser.AnalyzingFunction != null)
            {
                Type = Parser.AnalyzingFunction.GetTypeByLocalName(Name);
                if (Type != null)
                {
                    Parser.AnalyzingFunction.objects_s.Add(this);
                    return;
                }
            }

            Type = Class.GetTypeByLocalName(Name);
            if (Type == null) Error(LogContent.NotExistingDefinitionInType, Class.Name, Name);
            if (Next == null) Error(LogContent.NotSentence);
            if (Next.tag == Tag.FUNCTIONCALL) ((FunctionCall_s) Next).Class = GetClass(Type.type_name);
            if (Next.tag == Tag.OBJECT) ((Object_s) Next).Class = GetClass(Type.type_name);
            Class = GetClass(Type.type_name);
            Class.Objects_s.Add(this);
            Next.Check();
        }

        public override void Code()
        {
            ID = Statement.ID;

            if (VarType == VarType.Static)
            {
                FILGenerator.Write(InstructionType.pushsta, ID);
                return;
            }

            if (IsHead && VarType == VarType.Field)
            {
                if (Parser.AnalyzingFunction.FuncType == FuncType.Static)
                    Error(LogContent.ShouldBeingInstanced);
                FILGenerator.Write(InstructionType.pusharg); //this
                FILGenerator.Write(InstructionType.pushfield, ID);
                return;
            }

            switch (VarType)
            {
                case VarType.Arg:
                    FILGenerator.Write(InstructionType.pusharg, ID);
                    break;
                case VarType.Field:
                    FILGenerator.Write(InstructionType.pushfield, ID);
                    break;
                case VarType.Local:
                    FILGenerator.Write(InstructionType.pushloc, ID);
                    break;
            }
        }

        public Sentence BuildNext()
        {
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.ID) Error(LogContent.SthUseless, Lexer.NextToken);
            Lexer.Next();
            if (Lexer.NextToken.tag == Tag.LBRACKETS)
            {
                Lexer.Back();
                return new FunctionCall_s(Tag.FUNCTIONCALL).Build();
            }

            Lexer.Back();
            return new Object_s(Tag.OBJECT).Build();
        }
    }
}