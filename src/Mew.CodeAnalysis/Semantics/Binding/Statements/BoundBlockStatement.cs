namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundBlockStatement : BoundStatement
{
    public ImmutableArray<BoundStatement> Statements { get; }

    public BoundBlockStatement(Syntax syntax, ImmutableArray<BoundStatement> statements)
        : base(syntax)
    {
        Statements = statements;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitBlockStatement(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitBlockStatement(this, context);
    }
}
