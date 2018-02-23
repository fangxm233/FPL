using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.symbols;
using FPL.Encoding;

namespace FPL.inter
{
    public class Assign : Sentence
    {
        public Expr left;
        public Expr right;
        public int id;
        public string TypeName = null;
        public symbols.Type type;
        //public string name;

        //public Assign(int tag) : base(tag)
        //{
            
        //}
        //public Assign(string type_name, int tag) : base(tag)
        //{
        //    TypeName = type_name;
        //}
        public Assign(Expr left, int tag) : base(tag)
        {
            this.left = left;
        }

        public override Sentence Build()
        {
            //name = ((Word)Lexer.Peek).lexeme;
            //if (TypeName == null)
            //    type = GetTypeByName(name);
            //else
            //    type = symbols.Type.UnKnown;
            //if (left != null)
            //{
            //    left = new Object_e();
            //    left.Build();
            //    Lexer.Next();
            //}
            //Lexer.Next();
            switch (Lexer.Peek.tag)
            {
                case Tag.SEMICOLON:
                case Tag.RBRACKETS:
                case Tag.COMMA:
                    return this;
            }
            if (Lexer.Peek.tag != Tag.ASSIGN) Error("应输入\"=\"");
            right = new Expr().BuildStart();
            //switch (type.type)
            //{
            //    case "int":
            //        {
            //            return new Int(this);
            //        }
            //    case "float":
            //        {
            //            return new Float(this);
            //        }
            //    case "string":
            //        {
            //            return new String(this);
            //        }
            //    case "bool":
            //        {
            //            return new Bool(this);
            //        }
            //}
            return this;
        }

        public override void Check()
        {
            if (TypeName != null) type = symbols.Type.GetType(TypeName);
            if (type == null && TypeName != null) Error(this, "当前上下文中不存在名称" + TypeName);
            left = left.Check();
            type = left.type;
            if (right == null) return;
            right = right.Check();
            if (type != right.type) Error(this, "无法将类型\"" + right.type.type_name + "\"转换为类型\"" + type.type_name + "\"");
        }

        public override void Code()
        {
            if(right == null) return;
            int i = left.GetIndex();
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
            }
        }
    }

    //public class Int : Assign
    //{
    //    public Int(Assign a) : base(Tag.NUM)
    //    {
    //        left = a.left;
    //        right = a.right;
    //    }

    //    public override void Check()
    //    {
    //        if (right.Check()) right = right.ToStringPlus();
    //        if (right.type.type != "int") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"int\"");
    //        return;
    //    }
    //}
    //public class Float : Assign
    //{
    //    public Float(Assign a) : base(Tag.NUM)
    //    {
    //        left = a.left;
    //        right = a.right;
    //    }

    //    public override void Check()
    //    {
    //        if (right.Check()) right = right.ToStringPlus();
    //        if (right.type.type != "float" && right.type.type != "int") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"float\"");
    //        return;
    //    }
    //}
    //public class String : Assign
    //{
    //    public String(Assign a) : base(Tag.NUM)
    //    {
    //        left = a.left;
    //        right = a.right;
    //    }

    //    public override void Check()
    //    {
    //        if (right.Check()) right = right.ToStringPlus();
    //        if (right.type.type != "string") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"string\"");
    //        return;
    //    }
    //}
    //public class Bool : Assign
    //{
    //    public Bool(Assign a) : base(Tag.NUM)
    //    {
    //        left = a.left;
    //        right = a.right;
    //    }

    //    public override void Check()
    //    {
    //        if (right.Check()) right = right.ToStringPlus();
    //        if (right.type.type != "bool") Error(this, "无法将类型\"" + right.type.type + "\"转换为类型\"bool\"");
    //        return;
    //    }
    //}
}
