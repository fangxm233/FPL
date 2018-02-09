using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.lexer;
using FPL.Encoding;

namespace FPL.inter
{
    [Serializable]
    public class FunctionCall_s : Sentence
    {
        public string name;
        public symbols.Type return_type;
        public List<Expr> parameters = new List<Expr>();
        CodingUnit unit;

        public FunctionCall_s(int tag) : base(tag)
        {
            name = ((Word)Lexer.Peek).lexeme;
        }

        public override Sentence Build()
        {
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            while (true)
            {
                parameters.Add(new Expr().BuildStart());
                if (parameters[parameters.Count - 1] == null)
                {
                    if (Lexer.Peek.tag == Tag.COMMA) Error("缺少参数");
                    if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
                    parameters.RemoveAt(parameters.Count - 1);
                    break;
                }
                if (Lexer.Peek.tag == Tag.RBRACKETS) break;
            }
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            return this;
        }

        public override void Check()
        {
            if (parameters.Count != GetFunction(name).statements.Count) Error("\"" + name + "\"方法没有采用" + parameters.Count + "个参数的重载");
            return_type = GetFunction(name).return_type;
        }

        public override void Code()
        {
            if (GetFunction(name).statements.Count != 0)
            {
                foreach (var item in parameters)
                {
                    item.Code();
                    Encoder.Write(InstructionsType.poparg);
                }
            }
            unit = Encoder.Write(InstructionsType.call);
            Encoder.Write(InstructionsType.pop);
        }

        public override void CodeSecond()
        {
            unit.parameter = GetFunction(name).id;//填写所指函数
        }
    }

    [Serializable]
    public class FunctionCall_e : Expr
    {
        public string name;
        public symbols.Type return_type;
        public List<Expr> parameters = new List<Expr>();
        CodingUnit unit;

        public FunctionCall_e(int tag)
        {
            name = ((Word)Lexer.Peek).lexeme;
        }

        public override Expr Build()
        {
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            while (true)
            {
                parameters.Add(new Expr().BuildStart());
                if (parameters[parameters.Count - 1] == null)
                {
                    if (Lexer.Peek.tag == Tag.COMMA) Error("缺少参数");
                    if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
                    parameters.RemoveAt(parameters.Count - 1);
                    break;
                }
                if (Lexer.Peek.tag == Tag.RBRACKETS) break;
            }
            return this;
        }

        public override bool Check()
        {
            if (parameters.Count != GetFunction(name).statements.Count) Error("\"" + name + "\"方法没有采用" + parameters.Count + "个参数的重载");
            return_type = GetFunction(name).return_type;
            return return_type == symbols.Type.String ? true : false;
        }

        public override Expr ToStringPlus()
        {
            return this;
        }

        public override void Code()
        {
            if (GetFunction(name).statements.Count != 0)
            {
                foreach (var item in parameters)
                {
                    item.Code();
                    Encoder.Write(InstructionsType.poparg);
                }
            }
            unit = Encoder.Write(InstructionsType.call);
        }

        public override void CodeSecond()
        {
            unit.parameter = GetFunction(name).id;//填写所指函数
        }
    }
}
