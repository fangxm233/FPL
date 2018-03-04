using FPL.LexicalAnalysis;
using System.Collections.Generic;
using FPL.Encoding;

namespace FPL.Parse.Expression
{
    public class Factor : Expr
    {
        public LinkedListNode<Expr> position;
        public LinkedListNode<Expr> end_position;
        public bool should_destory = true;

        public Statement statement;
        public bool is_head;
        public VarType varType;
        public int ID;

        public Factor(int tag, Token c)
        {
            this.tag = tag;
            content = c;
            if (tag == Tag.ID) name = content.ToString();
        }

        public void Set_position(LinkedListNode<Expr> pos)
        {
            position = pos;
        }

        public override void Build()
        {
            if (tag == Tag.LBRACKETS)
            {
                if (position.Previous != null && position.Previous.Value.tag == Tag.ID)
                {
                    should_destory = false;
                }
                BuildTree(position, end_position);
                if (should_destory)
                {
                    position.List.Remove(end_position);
                    position.List.Remove(position);
                }
            }
        }

        public override void Check()
        {
            if (tag != Tag.ID) return;
            //@class == null意味着是这一串对象中是第一个
            //获取class和statement
            if (@class == null)
            {
                @class = Parser.analyzing_class;
                statement = @class.GetStatement(name);
                if (Parser.analyzing_function != null)
                {
                    statement = Parser.analyzing_function.GetStatement(name);
                    type = Parser.analyzing_function.GetTypeByLocalName(name);
                    if (statement == null)
                    {
                        is_head = true;
                        statement = @class.GetStatement(name);
                        type = @class.GetTypeByLocalName(name);
                        if (statement == null)
                        {
                            @class = GetClass(name);
                            varType = VarType.Class;
                            return;
                        }
                    }
                }
            }
            else
                statement = @class.GetStatement(name);
            varType = statement.varType;
            if (type == null) Error(this, "类型\"" + @class.name + "\"中未包含\"" + name + "\"的定义");
            return;
        }

        public override void Code()
        {
            if(tag ==Tag.NUM) Encoder.Write(InstructionType.pushval, (int)content.GetValue());
            if(tag == Tag.ID) ObjectCode();
        }

        public void ObjectCode()
        {
            if (varType == VarType.Class) return;
            ID = statement.ID;
            if (varType == VarType.Static)
            {
                Encoder.Write(InstructionType.pushsta, ID);
                return;
            }
            if (is_head)
            {
                Encoder.Write(InstructionType.pusharg);//this
                Encoder.Write(InstructionType.pushfield, ID);
                return;
            }
            switch (varType)
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
