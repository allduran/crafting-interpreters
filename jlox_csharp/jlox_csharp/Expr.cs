namespace JLox
{
    public abstract class Expr
    {
        public abstract R Accept<R>(IVisitor<R> visitor);
    }

    // Literal: value (ex: 123 or "hi")
    public class Literal : Expr
    {
        public object? Value { get; }

        public Literal(object? value)
        {
            Value = value;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
    }

    // Unary expression: operator, subexpression right (ex: -5)
    public class Unary : Expr
    {
        public Token Operator { get; }
        public Expr Right { get; }

        public Unary(Token operatorToken, Expr right)
        {
            Operator = operatorToken;
            Right = right;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
    }

    // Binary expression: left, operator, right (ex: 1 + 2)
    public class Binary : Expr
    {
        public Expr Left { get; }
        public Token Operator { get; }
        public Expr Right { get; }

        public Binary(Expr left, Token operatorToken, Expr right)
        {
            Left = left;
            Operator = operatorToken;
            Right = right;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
    }

    public class Grouping : Expr
    {
        public Expr Expression { get; }

        public Grouping(Expr expression)
        {
            Expression = expression;
        }

        public override R Accept<R>(IVisitor<R> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
    }

}
