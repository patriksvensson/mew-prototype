namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundIfStatement : BoundStatement
{
    public BoundExpression Condition { get; }
    public BoundStatement ThenBranch { get; }
    public BoundStatement? ElseBranch { get; }

    public BoundIfStatement(
        Syntax syntax,
        BoundExpression condition,
        BoundStatement thenBranch,
        BoundStatement? elseBranch)
            : base(syntax)
    {
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        ThenBranch = thenBranch ?? throw new ArgumentNullException(nameof(thenBranch));
        ElseBranch = elseBranch;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitIfStatement(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitIfStatement(this, context);
    }
}