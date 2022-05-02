namespace Mew.CodeAnalysis.Semantics;

public abstract class BoundNode
{
    public Syntax Syntax { get; }

    protected BoundNode(Syntax syntax)
    {
        Syntax = syntax;
    }

    [DebuggerStepThrough]
    public abstract void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context);

    [DebuggerStepThrough]
    public abstract TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context);
}
