namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundUnaryOperator
{
    private static readonly BoundUnaryOperator[] _operators;

    public UnaryOperator Operator { get; }
    public TypeSymbol OperandType { get; }
    public TypeSymbol Type { get; }

    static BoundUnaryOperator()
    {
        _operators = new BoundUnaryOperator[]
        {
            // Boolean operations
            new BoundUnaryOperator(UnaryOperator.Negate, PrimitiveTypeSymbol.Boolean),

            // Integer operations
            new BoundUnaryOperator(UnaryOperator.Negative, PrimitiveTypeSymbol.Integer),
        };
    }

    private BoundUnaryOperator(UnaryOperator op, TypeSymbol type)
    {
        Operator = op;
        OperandType = type;
        Type = type;
    }

    public static BoundUnaryOperator? Bind(UnaryOperator kind, TypeSymbol operandType)
    {
        foreach (var op in _operators)
        {
            if (op.Operator == kind && op.OperandType == operandType)
            {
                return op;
            }
        }

        return null;
    }
}