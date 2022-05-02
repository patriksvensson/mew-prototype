namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundErrorExpression : BoundExpression
{
    public override TypeSymbol Type => TypeSymbol.Error;

    public BoundErrorExpression(Syntax syntax)
        : base(syntax)
    {
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitErrorExpression(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitErrorExpression(this, context);
    }
}