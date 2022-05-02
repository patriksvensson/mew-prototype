namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundStringLiteralExpression : BoundExpression
{
    public override TypeSymbol Type { get; } = PrimitiveTypeSymbol.String;
    public string Value { get; }

    public BoundStringLiteralExpression(StringLiteralExpression syntax)
        : base(syntax)
    {
        Value = syntax.Value;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitStringLiteralExpression(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitStringLiteralExpression(this, context);
    }
}
