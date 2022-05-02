namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundLogicalOperator
{
    public LogicalOperator Operator { get; }
    public TypeSymbol Type { get; }

    public BoundLogicalOperator(LogicalOperator op)
    {
        Operator = op;
        Type = PrimitiveTypeSymbol.Boolean;
    }

    public static BoundLogicalOperator? Bind(LogicalOperator kind, TypeSymbol left, TypeSymbol right)
    {
        if (left != PrimitiveTypeSymbol.Boolean || right != PrimitiveTypeSymbol.Boolean)
        {
            return null;
        }

        return new BoundLogicalOperator(kind);
    }
}