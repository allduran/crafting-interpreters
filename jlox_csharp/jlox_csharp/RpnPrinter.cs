namespace JLox
{
    public class RpnPrinter : IVisitor<string>
    {
        public string Print(Expr expr) => expr.Accept(this);

        public string VisitUnaryExpr(Unary expr) => $"{expr.Right.Accept(this)} {expr.Operator.Lexeme}";

        public string VisitBinaryExpr(Binary expr) => $"{expr.Left.Accept(this)} {expr.Right.Accept(this)} {expr.Operator.Lexeme}";

        public string VisitGroupingExpr(Grouping expr) => expr.Expression.Accept(this);

        public string VisitLiteralExpr(Literal expr)
        {
            if (expr.Value == null) return "nil";
            return expr.Value.ToString();
        }

    }
}
