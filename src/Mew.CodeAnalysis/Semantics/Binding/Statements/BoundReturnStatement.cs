namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundReturnStatement : BoundStatement
{
    public BoundExpression? Expression { get; }

    public BoundReturnStatement(Syntax syntax, BoundExpression? expression)
        : base(syntax)
    {
        Expression = expression;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitReturnStatement(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitReturnStatement(this, context);
    }
}
