namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundLabelStatement : BoundStatement
{
    public BoundLabel Label { get; }

    public BoundLabelStatement(Syntax syntax, BoundLabel label)
        : base(syntax)
    {
        Label = label ?? throw new ArgumentNullException(nameof(label));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitLabelStatement(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitLabelStatement(this, context);
    }
}