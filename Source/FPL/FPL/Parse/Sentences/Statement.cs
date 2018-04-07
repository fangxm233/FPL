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
            Class = Parser.AnalyzingClass;
            TypeName = Lexer.NextToken.ToString();

            Lexer.Next();
            if (Lexer.NextToken.tag != Tag.ID) ErrorSta(LogContent.IDExpect);
            Name = Lexer.NextToken.ToString();
            AddVar(this, VarType);

            Lexer.Back();
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
            Assign.Type = Type.GetType(TypeName);
            Assign.Check();
        }

        public override void Code()
        {
            Assign.Code();
        }

        public override int GetTokenLength()
        {
            return 1 + Assign.GetTokenLength();
        }
    }
}