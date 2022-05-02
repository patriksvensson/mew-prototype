namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundBinaryExpression : BoundExpression
{
    public BoundExpression Left { get; }
    public BoundBinaryOperator Op { get; }
    public BoundExpression Right { get; }

    public override TypeSymbol Type => Op.Type;

    public BoundBinaryExpression(
        Syntax syntax,
        BoundExpression left,
        BoundBinaryOperator op,
        BoundExpression right)
            : base(syntax)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Op = op ?? throw new ArgumentNullException(nameof(op));
        Right = right ?? throw new ArgumentNullException(nameof(right));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitBinaryExpression(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitBinaryExpression(this, context);
    }
}
