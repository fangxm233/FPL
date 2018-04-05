using System.Collections.Generic;
using FPL.DataStorager;
using FPL.Generator;
using FPL.LexicalAnalysis;
using FPL.OutPut;
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
        private bool isBuilt;

        public Statement Statement;
        public VarType VarType;

        public Factor(int tag, Token c)
        {
            this.tag = tag;
            Content = c;
            if (tag == Tag.ID) Name = Content.ToString();
        }

        public void Set_position(LinkedListNode<Expr> pos)
        {
            Position = pos;
        }

        public override void Build()
        {
            if (isBuilt) return;
            if (tag == Tag.LBRACKETS)
            {
                if (Position.Previous != null && Position.Previous.Value.tag == Tag.ID) ShouldDestory = false;
                BuildTree(Position, EndPosition);
                if (ShouldDestory)
                {
                    Position.List.Remove(EndPosition);
                    Position.List.Remove(Position);
                }
            }

            isBuilt = true;
        }

        public override void Check()
        {
            if (tag != Tag.ID)
            {
                switch (tag)
                {
                    case Tag.TRUE:
                    case Tag.FALSE:
                        Type = Type.Bool;
                        break;
                }
                return;
            }
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
            if (Type == null) Error(LogContent.NotExistingDefinitionInType, Class.Name, Name);
        }

        public override void Code()
        {
            if (tag == Tag.NUM) FILGenerator.Write(InstructionType.pushval, (int) Content.GetValue());
            if (tag == Tag.ID) ObjectCode();
        }

        public void ObjectCode()
        {
            if (VarType == VarType.Class) return;
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
    }
}