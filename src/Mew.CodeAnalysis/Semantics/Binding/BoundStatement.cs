namespace Mew.CodeAnalysis.Semantics;

public abstract class BoundStatement : BoundNode
{
    protected BoundStatement(Syntax syntax)
        : base(syntax)
    {
    }
}
