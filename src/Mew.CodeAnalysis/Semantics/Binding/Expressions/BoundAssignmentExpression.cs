namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundAssignmentExpression : BoundExpression
{
    public VariableSymbol Variable { get; }
    public BoundExpression Expression { get; }

    public override TypeSymbol Type => Expression.Type;

    public BoundAssignmentExpression(
        Syntax syntax,
        VariableSymbol variable,
        BoundExpression expression)
            : base(syntax)
    {
        Variable = variable ?? throw new ArgumentNullException(nameof(variable));
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitAssignment(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitAssignment(this, context);
    }
}
