namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundFunctionCallExpression : BoundExpression
{
    public FunctionSymbol Function { get; }
    public ImmutableArray<BoundExpression> Arguments { get; }

    public override TypeSymbol Type => Function.ReturnType;

    public BoundFunctionCallExpression(
        Syntax syntax,
        FunctionSymbol function,
        ImmutableArray<BoundExpression> arguments)
            : base(syntax)
    {
        Function = function ?? throw new ArgumentNullException(nameof(function));
        Arguments = arguments;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(IBoundNodeVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitFunctionCallExpression(this, context);
    }

    [DebuggerStepThrough]
    public override TResult Accept<TContext, TResult>(IBoundNodeVisitor<TContext, TResult> visitor, TContext context)
    {
        return visitor.VisitFunctionCallExpression(this, context);
    }
}
