using System.Collections.Generic;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse;

namespace FPL.Parse
{
    public class Do : Sentence
    {
        Rel rel;
        public List<Sentence> sentences;
        public int end_line;
        int head_line;
        public int rel_line;

        public Do(int tag) : base(tag)
        {

        }

        public override Sentence Build()
        {
            NewScope();
            Lexer.Next();
            if (Lexer.Peek.tag == Tag.LBRACE)
            {
                sentences = BuildMethod();
                if (Lexer.Peek.tag != Tag.RBRACE) Error("应输入\"}\"");
            }
            else
            {
                Lexer.Back();
                sentences = new List<Sentence>
                {
                    BuildOne()
                };
            }
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.WHILE) Error("应输入\"while\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            rel = new Rel().BuildStart();
            if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.SEMICOLON) Error("应输入\";\"");
            DestroyScope();
            return this;
        }

        public override void Check()
        {
            rel.Check();
            foreach (Sentence item in sentences)
            {
                Parse.Parser.analyzing_loop = this;
                item.Check();
            }
            Parse.Parser.analyzing_loop = null;
        }

        public override void Code()
        {
            head_line = Encoder.line + 1;
            foreach (var item in sentences)
            {
                item.Code();
            }
            if (Encoder.line == head_line) return;
            rel_line = Encoder.line + 1;
            rel.Code(0);
            CodingUnit u = Encoder.code[Encoder.code.Count - 1];
            u.parameter = head_line;
            end_line = u.line_num;
        }
    }
}
