namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundUnaryExpression : BoundExpression
{
    public BoundExpression Operand { get; }
    public BoundUnaryOperator Op { get; }

    public override TypeSymbol Type => Op.Type;

    public BoundUnaryExpression(
        Syntax syntax,
        BoundExpression operand,
        BoundUnaryOperator op)
            : base(syntax)
    {
        Operand = operand ?? throw new ArgumentNullException(nameof(operand));
        Op = op ?? throw new ArgumentNullException(nameof(op));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitUnaryExpression(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitUnaryExpression(this, context);
    }
}
