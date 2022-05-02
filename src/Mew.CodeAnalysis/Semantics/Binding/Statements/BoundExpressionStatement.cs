namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundExpressionStatement : BoundStatement
{
    public BoundExpression Expression { get; }

    public BoundExpressionStatement(Syntax syntax, BoundExpression expression)
        : base(syntax)
    {
        Expression = expression;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitExpressionStatement(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitExpressionStatement(this, context);
    }
}