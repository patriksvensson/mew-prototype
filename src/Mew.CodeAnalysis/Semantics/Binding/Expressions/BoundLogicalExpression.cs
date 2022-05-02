namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundLogicalExpression : BoundExpression
{
    public BoundExpression Left { get; }
    public BoundLogicalOperator Op { get; }
    public BoundExpression Right { get; }

    public override TypeSymbol Type => Op.Type;

    public BoundLogicalExpression(
        Syntax syntax,
        BoundExpression left,
        BoundLogicalOperator op,
        BoundExpression right)
            : base(syntax)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Op = op;
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitLogicalExpression(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitLogicalExpression(this, context);
    }
}
