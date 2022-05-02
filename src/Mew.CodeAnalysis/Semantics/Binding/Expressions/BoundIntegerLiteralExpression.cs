namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundIntegerLiteralExpression : BoundExpression
{
    public override TypeSymbol Type { get; } = PrimitiveTypeSymbol.Integer;
    public long Value { get; }

    public BoundIntegerLiteralExpression(IntegerLiteralExpression syntax)
        : base(syntax)
    {
        Value = syntax.Value;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitIntegerLiteralExpression(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitIntegerLiteralExpression(this, context);
    }
}
