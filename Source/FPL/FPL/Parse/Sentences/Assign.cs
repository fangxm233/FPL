using System.Linq;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Expression;
using FPL.symbols;

namespace FPL.Parse.Sentences
{
    public class Assign : Sentence
    {
        public int id;
        public Expr left;
        public Expr right;
        public Type type;
        public string TypeName = null;

        public Assign(Expr left, int tag) : base(tag)
        {
            this.left = left;
        }

        public override Sentence Build()
        {
            switch (Lexer.Peek.tag)
            {
                case Tag.SEMICOLON:
                case Tag.RBRACKETS:
                case Tag.COMMA:
                    return this;
            }

            if (Lexer.Peek.tag != Tag.ASSIGN) Error("应输入\"=\"");
            right = new Expr().BuildStart();
            return this;
        }

        public override void Check()
        {
            if (TypeName != null) type = Type.GetType(TypeName);
            if (type == null && TypeName != null) Error(this, "当前上下文中不存在名称" + TypeName);
            left.Check();
            type = left.type;
            if (right == null) return;
            right.Check();
            if (type != right.type) Error(this, "无法将类型\"" + right.type.type_name + "\"转换为类型\"" + type.type_name + "\"");
        }

        public override void Code()
        {
            if (right == null) return;
            right.Code();
            left.Code();
            CodingUnit codingUnit = Encoder.code.Last();
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