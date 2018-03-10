using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse.Sentences;

namespace FPL.Parse.Expression
{
    public class Factor : Expr
    {
        public LinkedListNode<Expr> EndPosition;
        public int ID;
        public bool IsHead;
        public LinkedListNode<Expr> Position;
        public bool ShouldDestory = true;

        public Statement Statement;
        public VarType VarType;

        public Factor(int tag, Token c)
        {
            this.Tag = tag;
            Content = c;
            if (tag == LexicalAnalysis.Tag.ID) Name = Content.ToString();
        }

        public void Set_position(LinkedListNode<Expr> pos)
        {
            Position = pos;
        }

        public override void Build()
        {
            if (Tag == LexicalAnalysis.Tag.LBRACKETS)
            {
                if (Position.Previous != null && Position.Previous.Value.Tag == LexicalAnalysis.Tag.ID) ShouldDestory = false;
                BuildTree(Position, EndPosition);
                if (ShouldDestory)
                {
                    Position.List.Remove(EndPosition);
                    Position.List.Remove(Position);
                }
            }
        }

        public override void Check()
        {
            if (Tag != LexicalAnalysis.Tag.ID) return;
            //@class == null意味着是这一串对象中是第一个
            //获取class和statement
            if (Class == null)
            {
                Class = Parser.AnalyzingClass;
                Statement = Class.GetStatement(Name);
                if (Parser.AnalyzingFunction != null)
                {
                    Statement = Parser.AnalyzingFunction.GetStatement(Name);
                    Type = Parser.AnalyzingFunction.GetTypeByLocalName(Name);
                    if (Statement == null)
                    {
                        IsHead = true;
                        Statement = Class.GetStatement(Name);
                        Type = Class.GetTypeByLocalName(Name);
                        if (Statement == null)
                        {
                            Class = GetClass(Name);
                            VarType = VarType.Class;
                            return;
                        }
                    }
                }
            }
            else
            {
                Statement = Class.GetStatement(Name);
            }

            VarType = Statement.VarType;
            if (Type == null) Error(this, "类型\"" + Class.Name + "\"中未包含\"" + Name + "\"的定义");
        }

        public override void Code()
        {
            if (Tag == LexicalAnalysis.Tag.NUM) Encoder.Write(InstructionType.pushval, (int) Content.GetValue());
            if (Tag == LexicalAnalysis.Tag.ID) ObjectCode();
        }

        public void ObjectCode()
        {
            if (VarType == VarType.Class) return;
            ID = Statement.ID;
            if (VarType == VarType.Static)
            {
                Encoder.Write(InstructionType.pushsta, ID);
                return;
            }

            if (IsHead && VarType == VarType.Field)
            {
                if (Parser.AnalyzingFunction.FuncType == FuncType.Static)
                    Error(this, "对象引用对于非静态的字段、方法或属性\"" + Name + "\"是必须的");
                Encoder.Write(InstructionType.pusharg); //this
                Encoder.Write(InstructionType.pushfield, ID);
                return;
            }

            switch (VarType)
            {
                case VarType.Arg:
                    Encoder.Write(InstructionType.pusharg, ID);
                    break;
                case VarType.Field:
                    Encoder.Write(InstructionType.pushfield, ID);
                    break;
                case VarType.Local:
                    Encoder.Write(InstructionType.pushloc, ID);
                    break;
            }
        }
    }
}