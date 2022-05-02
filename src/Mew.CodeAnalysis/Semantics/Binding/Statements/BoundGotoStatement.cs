namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundGotoStatement : BoundStatement
{
    public BoundLabel Label { get; }

    public BoundGotoStatement(Syntax syntax, BoundLabel label)
        : base(syntax)
    {
        Label = label ?? throw new ArgumentNullException(nameof(label));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitGotoStatement(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitGotoStatement(this, context);
    }
}
