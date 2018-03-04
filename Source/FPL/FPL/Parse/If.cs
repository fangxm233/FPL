using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FPL.symbols;
using FPL.Encoding;
using FPL.LexicalAnalysis;
using FPL.Parse;

namespace FPL.Parse
{
    public class If : Sentence
    {
        Rel rel;
        List<Sentence> sentences;
        List<Sentence> sentences_else;
        CodingUnit to_end;

        public If(int tag) : base(tag)
        {
            
        }

        public override Sentence Build()
        {
            NewScope();
            Lexer.Next();
            if (Lexer.Peek.tag != Tag.LBRACKETS) Error("应输入\"(\"");
            rel = new Rel().BuildStart();
            if (Lexer.Peek.tag != Tag.RBRACKETS) Error("应输入\")\"");
            Lexer.Next();
            if(Lexer.Peek.tag == Tag.LBRACE)
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
            DestroyScope();
            Lexer.Next();
            if (Lexer.Peek.tag == Tag.ELSE)
            {
                NewScope();
                Lexer.Next();
                if (Lexer.Peek.tag == Tag.LBRACKETS)
                {
                    sentences_else = BuildMethod();
                    if (Lexer.Peek.tag != Tag.RBRACE) Error("应输入\"}\"");
                }
                else
                {
                    Lexer.Back();
                    sentences_else = new List<Sentence>
                    {
                        BuildOne()
                    };
                }
                DestroyScope();
            }
            else
                Lexer.Back();
            return this;
        }

        public override void Check()
        {
            rel.Check();
            foreach (Sentence item in sentences)
            {
                item.Check();
            }
            if (sentences_else == null) return;
            foreach (var item in sentences_else)
            {
                item.Check();
            }
        }

        public override void Code()
        {
            rel.Code(0);
            CodingUnit u = Encoder.code[Encoder.code.Count - 1];
            u.parameter = Encoder.line + 2;
            to_end = Encoder.Write(InstructionType.jmp);
            foreach (var item in sentences)
            {
                item.Code();
            }
            to_end.parameter = Encoder.line + 1;
            if (sentences_else == null) return;
            foreach (var item in sentences_else)
            {
                item.Code();
            }
        }

        public override void CodeSecond()
        {
            foreach (var item in sentences)
            {
                item.CodeSecond();
            }
        }
    }
}
