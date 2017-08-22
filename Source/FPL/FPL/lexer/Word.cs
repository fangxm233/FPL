using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FPL.lexer
{
    [Serializable]
    public class Word : Token
    {
        public string lexeme = "";
        public Word(string s, int i) : base(i)
        {
            lexeme = s;
        }
        public static readonly Word
            and  = new Word("&&", Tag.AND),  or   = new Word("||", Tag.OR),
            eq   = new Word("==", Tag.EQ),   ne   = new Word("!=", Tag.NE),
            le   = new Word("<=", Tag.LE),   ge   = new Word(">=", Tag.GE),
            more = new Word(">",  Tag.MORE), less = new Word("<",  Tag.LESS),

            plus     = new Word("plus",     Tag.PLUS),
            minus    = new Word("minus",    Tag.MINUS),
            multiply = new Word("multiply", Tag.MULTIPLY),
            divide   = new Word("divide",   Tag.DIVIDE),

            semicolon    = new Word("semicolon",    Tag.SEMICOLON),
            Lparenthesis = new Word("Lparenthesis", Tag.LPARENTHESIS),
            Rparenthesis = new Word("Rparenthesis", Tag.RPARENTHESIS),
            LBrace       = new Word("LBrace",       Tag.LBRACE),
            RBrace       = new Word("RBrace",       Tag.RBRACE),

            True   = new Word("true",   Tag.TRUE),
            False  = new Word("false",  Tag.FALSE),
            assign = new Word("assign", Tag.ASSIGN),
            temp   = new Word("temp",   Tag.TEMP);
    }
}
