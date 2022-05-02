namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundWhileStatement : BoundStatement
{
    public BoundExpression Condition { get; }
    public BoundStatement Body { get; }
    public BoundLabel BreakLabel { get; }
    public BoundLabel ContinueLabel { get; }

    public BoundWhileStatement(
        Syntax syntax,
        BoundExpression condition,
        BoundStatement body,
        BoundLabel breakLabel,
        BoundLabel continueLabel)
            : base(syntax)
    {
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        Body = body ?? throw new ArgumentNullException(nameof(body));
        BreakLabel = breakLabel ?? throw new ArgumentNullException(nameof(breakLabel));
        ContinueLabel = continueLabel ?? throw new ArgumentNullException(nameof(continueLabel));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitWhileStatement(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitWhileStatement(this, context);
    }
}
