namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundConversionExpression : BoundExpression
{
    public override TypeSymbol Type { get; }
    public BoundExpression Expression { get; }

    public BoundConversionExpression(
        Syntax syntax,
        TypeSymbol type,
        BoundExpression expression)
            : base(syntax)
    {
        Type = type ?? throw new ArgumentNullException(nameof(type));
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitConversionExpression(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitConversionExpression(this, context);
    }
}