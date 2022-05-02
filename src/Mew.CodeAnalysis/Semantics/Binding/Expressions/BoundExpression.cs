namespace Mew.CodeAnalysis.Semantics;

public abstract class BoundExpression : BoundNode
{
    public abstract TypeSymbol Type { get; }

    protected BoundExpression(Syntax syntax)
        : base(syntax)
    {
    }
}
