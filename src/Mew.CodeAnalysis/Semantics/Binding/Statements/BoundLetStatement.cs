namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundLetStatement : BoundStatement
{
    public VariableSymbol Variable { get; }
    public BoundExpression Initializer { get; }

    public BoundLetStatement(
        Syntax syntax,
        VariableSymbol variable,
        BoundExpression initializer)
            : base(syntax)
    {
        Variable = variable ?? throw new ArgumentNullException(nameof(variable));
        Initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitLetStatement(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitLetStatement(this, context);
    }
}