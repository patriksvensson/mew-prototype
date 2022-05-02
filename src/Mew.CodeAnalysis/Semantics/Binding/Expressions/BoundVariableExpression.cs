namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundVariableExpression : BoundExpression
{
    public VariableSymbol Symbol { get; }
    public override TypeSymbol Type => Symbol.Type;

    public BoundVariableExpression(
        IdentifierExpression syntax,
        VariableSymbol symbol)
            : base(syntax)
    {
        Symbol = symbol ?? throw new ArgumentNullException(nameof(symbol));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitVariableExpression(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitVariableExpression(this, context);
    }
}
