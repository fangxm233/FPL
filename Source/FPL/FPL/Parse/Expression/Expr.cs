using System.Collections.Generic;
using System.Linq;
using FPL.Encoding;
using FPL.LexicalAnalysis;

namespace FPL.Parse.Expression
{
    public class Expr : Node
    {
        public Expr left;
        public Expr right;
        public Token content;
        public symbols.Type type = symbols.Type.Int;
        public Class @class;
        public string name;
        public int tag;

        //public static LinkedList<Expr> expr;

        public virtual void Build() { }

        public Expr BuildStart() //表达式建立的开始调用的，不可重写
        {
            LinkedList<Expr> expr = MatchAll();
            if (expr.Count == 1)
                return null;
            BuildTree(expr.First, expr.Last);
            return expr.Last.Value;
        }

        protected void BuildTree(LinkedListNode<Expr> start, LinkedListNode<Expr> end)
        {
            foreach (Dictionary<int, int> dictionary in Parser.symbol_priority)
            {
                for (LinkedListNode<Expr> node = start.Next; node != end.Next; node = node.Next)
                {
                    if (dictionary.ContainsKey(node.Value.tag))
                        node.Value.Build();
                }
            }
        }

        private LinkedList<Expr> MatchAll()
        {
            LinkedList<Expr> expr = new LinkedList<Expr>();
            expr.AddFirst(new Expr());
            List<Factor> Brackets = new List<Factor>();
            while (true)
            {
                Lexer.Next();
                switch (Lexer.Peek.tag)
                {
                    case Tag.SEMICOLON:
                    case Tag.AND:
                    case Tag.OR:
                    case Tag.EQ:
                    case Tag.NE:
                    case Tag.LE:
                    case Tag.GE:
                    case Tag.MORE:
                    case Tag.LESS:
                    case Tag.COMMA:
                    case Tag.ASSIGN:
                        return expr;
                    case Tag.ID:
                        Lexer.Next();
                        if (Lexer.Peek.tag == Tag.LBRACKETS)
                        {
                            Lexer.Back();
                            expr.AddLast(new FunctionCall_e(Tag.FUNCTIONCALL));
                            expr.Last.Value.Build();
                            continue;
                        }
                        Lexer.Back();
                        break;
                    case Tag.NEW:
                        expr.AddLast(new New_e());
                        expr.Last.Value.Build();
                        continue;
                }
                if (!Parser.type_of_expr.ContainsKey(Lexer.Peek.tag))
                    Error("意外的字符" + Lexer.Peek);
                switch (Parser.type_of_expr[Lexer.Peek.tag])
                {
                    case Tag.FACTOR:
                        Factor f = new Factor(Lexer.Peek.tag, Lexer.Peek);
                        expr.AddLast(f);
                        f.Set_position(expr.Last);
                        if (Lexer.Peek.tag == Tag.LBRACKETS)Brackets.Add((Factor)expr.Last());
                        if (Lexer.Peek.tag == Tag.RBRACKETS)
                        {
                            if (Brackets.Count == 0)
                            {
                                expr.RemoveLast();
                                return expr;
                            }
                            Brackets.Last().end_position = expr.Last;
                            Brackets.RemoveAt(Brackets.Count - 1);
                        }
                        break;
                    case Tag.BINATY:
                        Binary b = new Binary(Lexer.Peek.tag);
                        expr.AddLast(b);
                        b.Set_position(expr.Last);
                        break;
                    case Tag.UNARY:
                        Unary u = new Unary(Lexer.Peek.tag);
                        expr.AddLast(u);
                        u.Set_position(expr.Last);
                        break;
                    case Tag.BOOL:
                        Bool bo = new Bool(Lexer.Peek.tag);
                        expr.AddLast(bo);
                        bo.Set_position(expr.Last);
                        break;
                }
            }
        }

        public virtual void Check() { }

        public virtual void Code() { }

        public virtual void CodeSecond()
        {
            left.CodeSecond();
            right.CodeSecond();
        }
    }

    public class Object_e : Expr
    {
        public bool is_head;
        public VarType varType;
    }
}