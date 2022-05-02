namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundLoopStatement : BoundStatement
{
    public BoundStatement Body { get; }
    public BoundLabel BreakLabel { get; }
    public BoundLabel ContinueLabel { get; }

    public BoundLoopStatement(
        Syntax syntax,
        BoundStatement body,
        BoundLabel breakLabel,
        BoundLabel continueLabel)
            : base(syntax)
    {
        Body = body ?? throw new ArgumentNullException(nameof(body));
        BreakLabel = breakLabel ?? throw new ArgumentNullException(nameof(breakLabel));
        ContinueLabel = continueLabel ?? throw new ArgumentNullException(nameof(continueLabel));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitLoopStatement(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitLoopStatement(this, context);
    }
}