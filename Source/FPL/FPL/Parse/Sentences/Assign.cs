using System.Linq;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Expression;
using FPL.symbols;

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

            if (Lexer.NextToken.tag != Tag.ASSIGN) Error("应输入\"=\"");
            Right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            if (TypeName != null) Type = Type.GetType(TypeName);
            if (Type == null && TypeName != null) Error(this, "当前上下文中不存在名称" + TypeName);
            Left.Check();
            Type = Left.Type;
            if (Right == null) return;
            Right.Check();
            if (Type != Right.Type) Error(this, "无法将类型\"" + Right.Type.type_name + "\"转换为类型\"" + Type.type_name + "\"");
        }

        public override void Code()
        {
            if (Right == null) return;
            Right.Code();
            Left.Code();
            CodingUnit codingUnit = Encoder.Code.Last();
            Encoder.Back();
            switch (codingUnit.ins_type)
            {
                case InstructionType.pusharg:
                    Encoder.Write(InstructionType.storearg, codingUnit.parameter);
                    break;
                case InstructionType.pushfield:
                    Encoder.Write(InstructionType.storefield, codingUnit.parameter);
                    break;
                case InstructionType.pushloc:
                    Encoder.Write(InstructionType.storeloc, codingUnit.parameter);
                    break;
                case InstructionType.pushsta:
                    Encoder.Write(InstructionType.storesta, codingUnit.parameter);
                    break;
            }
        }
    }
}