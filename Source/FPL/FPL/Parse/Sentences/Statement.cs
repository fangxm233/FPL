using FPL.DataStorager;
using FPL.LexicalAnalysis;
using FPL.OutPut;
using FPL.Parse.Expression;
using FPL.Parse.Structure;

namespace FPL.Parse.Sentences
{
    public class Statement : Sentence
    {
        public Assign Assign;
        public Class Class;
        public int ID;
        public string Name;
        private string TypeName;
        public VarType VarType;

        public Statement(VarType varType, int tag) : base(tag)
        {
            VarType = varType;
        }

        public override Sentence Build()
        {
            //switch (((Word)Lexer.Peek).lexeme)
            //{
            //    case "int":
            //        {
            //            assign = new Assign(Tag.ASSIGN);
            //            Lexer.Next();
            //            AddVar(((Word)Lexer.Peek).lexeme, symbols.Type.Int);
            //            break;
            //        }
            //    case "float":
            //        {
            //            assign = new Assign(Tag.ASSIGN);
            //            Lexer.Next();
            //            AddVar(((Word)Lexer.Peek).lexeme, symbols.Type.Float);
            //            break;
            //        }
            //    case "bool":
            //        {
            //            assign = new Assign(Tag.ASSIGN);
            //            Lexer.Next();
            //            AddVar(((Word)Lexer.Peek).lexeme, symbols.Type.Bool);
            //            break;
            //        }
            //    case "string":
            //        {
            //            assign = new Assign(Tag.ASSIGN);
            //            Lexer.Next();
            //            AddVar(((Word)Lexer.Peek).lexeme, symbols.Type.String);
            //            break;
            //        }
            //    case ",":
            //        Error("应输入类型名");
            //        break;
            //    default:
            //        {
            //        }
            //}
            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.ID) Error(LogContent.IDExpect);
            Name = Lexer.NextToken.ToString();
            AddVar(this, VarType);
            Lexer.Back();
            Class = Parser.AnalyzingClass;
            TypeName = Lexer.NextToken.ToString();
            Assign = (Assign) new Assign(new Expr().BuildStart(), Tag.ASSIGN).Build();
            if (Parser.AnalyzingFunction != null)
                Parser.AnalyzingFunction.Statements.Add(this);
            else
                Class.Statement.Add(this);
            if (Lexer.NextToken.tag == Tag.COMMA) return this;
            if (Lexer.NextToken.tag == Tag.RBRACKETS) return this;
            Match(";", false);
            return this;
        }

        public override void Check()
        {
            Assign.TypeName = TypeName;
            Assign.Check();
        }

        public override void Code()
        {
            Assign.Code();
        }
    }
}