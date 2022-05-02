namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundBooleanLiteralExpression : BoundExpression
{
    public override TypeSymbol Type { get; } = PrimitiveTypeSymbol.Boolean;
    public bool Value { get; }

    public BoundBooleanLiteralExpression(BooleanLiteralExpression syntax)
        : base(syntax)
    {
        Value = syntax.Value;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitBooleanLiteralExpression(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitBooleanLiteralExpression(this, context);
    }
}