using System.Linq;
using FPL.DataStorager;
using FPL.Generator;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Expression;

namespace FPL.Parse.Sentences
{
    public class Assign : Sentence
    {
        public int Id;
        public Expr Left;
        public Expr Right;
        public Type Type;
        public string TypeName = null;

        public Assign(Expr left, int tag) : base(tag)
        {
            Left = left;
        }

        public override Sentence Build()
        {
            switch (Lexer.NextToken.tag)
            {
                case Tag.SEMICOLON:
                case Tag.RBRACKETS:
                case Tag.COMMA:
                    return this;
            }

            Match("=",false);
            Right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            if (TypeName != null) Type = Type.GetType(TypeName);
            if (Type == null && TypeName != null) Error(Left, LogContent.NotExistingDefinition, TypeName);
            Left.Check();
            if (Type == null)
                Type = Left.Type;
            if (Right == null) return;
            Right.Check();
            if (Type != Right.Type) Error(LogContent.UnableToConvertType, Right.Type.type_name, Type.type_name);
        }

        public override void Code()
        {
            if (Right == null) return;
            Right.Code();
            Left.Code();
            CodingUnit codingUnit = FILGenerator.Code.Last();
            FILGenerator.Back();
            switch (codingUnit.InsType)
            {
                case InstructionType.pusharg:
                    FILGenerator.Write(InstructionType.storearg, codingUnit.Parameter);
                    break;
                case InstructionType.pushfield:
                    FILGenerator.Write(InstructionType.storefield, codingUnit.Parameter);
                    break;
                case InstructionType.pushloc:
                    FILGenerator.Write(InstructionType.storeloc, codingUnit.Parameter);
                    break;
                case InstructionType.pushsta:
                    FILGenerator.Write(InstructionType.storesta, codingUnit.Parameter);
                    break;
                default:
                    Error(Left, LogContent.ExprError);
                    break;
            }
        }

        public override int GetTokenLength()
        {
            if (Right != null) return Left.GetTokenLength() + Right.GetTokenLength() + 1;
            return Left.GetTokenLength();
        }
    }
}