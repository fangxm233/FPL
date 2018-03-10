﻿using FPL.LexicalAnalysis;
using FPL.Parse.Expression;
using FPL.Parse.Structure;

namespace FPL.Parse.Sentences
{
    public class Statement : Sentence
    {
        public Assign assign;
        public Class @class;
        public int ID;
        public string name;
        private string type_name;
        public VarType varType;

        public Statement(VarType varType, int tag) : base(tag)
        {
            this.varType = varType;
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
            if (Lexer.Peek.tag != Tag.ID) Error("应输入标识符");
            name = Lexer.Peek.ToString();
            AddVar(this, varType);
            Lexer.Back();
            @class = Parser.AnalyzingClass;
            type_name = ((Word) Lexer.Peek).lexeme;
            assign = (Assign) new Assign(new Expr().BuildStart(), Tag.ASSIGN).Build();
            if (Parser.AnalyzingFunction != null)
                Parser.AnalyzingFunction.Statements.Add(this);
            else
                @class.Statement.Add(this);
            if (Lexer.Peek.tag == Tag.COMMA) return this;
            if (Lexer.Peek.tag == Tag.RBRACKETS) return this;
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            assign.TypeName = type_name;
            assign.Check();
        }

        public override void Code()
        {
            assign.Code();
        }
    }
}