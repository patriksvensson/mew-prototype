namespace Mew.CodeAnalysis.Semantics;

public sealed class BoundBinaryOperator
{
    private static readonly BoundBinaryOperator[] _operators;

    public BinaryOperator Operator { get; }
    public TypeSymbol Left { get; }
    public TypeSymbol Right { get; }
    public TypeSymbol Type { get; }

    static BoundBinaryOperator()
    {
        _operators = new BoundBinaryOperator[]
        {
            // Integer operations
            new BoundBinaryOperator(BinaryOperator.Add, PrimitiveTypeSymbol.Integer),
            new BoundBinaryOperator(BinaryOperator.Subtract, PrimitiveTypeSymbol.Integer),
            new BoundBinaryOperator(BinaryOperator.Multiply, PrimitiveTypeSymbol.Integer),
            new BoundBinaryOperator(BinaryOperator.Divide, PrimitiveTypeSymbol.Integer),
            new BoundBinaryOperator(BinaryOperator.Modolu, PrimitiveTypeSymbol.Integer),
            new BoundBinaryOperator(BinaryOperator.Equal, PrimitiveTypeSymbol.Integer, PrimitiveTypeSymbol.Boolean),
            new BoundBinaryOperator(BinaryOperator.NotEqual, PrimitiveTypeSymbol.Integer, PrimitiveTypeSymbol.Boolean),
            new BoundBinaryOperator(BinaryOperator.LessThan, PrimitiveTypeSymbol.Integer, PrimitiveTypeSymbol.Boolean),
            new BoundBinaryOperator(BinaryOperator.LessThanOrEqual, PrimitiveTypeSymbol.Integer, PrimitiveTypeSymbol.Boolean),
            new BoundBinaryOperator(BinaryOperator.GreaterThan, PrimitiveTypeSymbol.Integer, PrimitiveTypeSymbol.Boolean),
            new BoundBinaryOperator(BinaryOperator.GreaterThanOrEqual, PrimitiveTypeSymbol.Integer, PrimitiveTypeSymbol.Boolean),

            // Boolean operations
            new BoundBinaryOperator(BinaryOperator.Equal, PrimitiveTypeSymbol.Boolean),
            new BoundBinaryOperator(BinaryOperator.NotEqual, PrimitiveTypeSymbol.Boolean),

            // String operations
            new BoundBinaryOperator(BinaryOperator.Add, PrimitiveTypeSymbol.String),
            new BoundBinaryOperator(BinaryOperator.Equal, PrimitiveTypeSymbol.String, PrimitiveTypeSymbol.Boolean),
            new BoundBinaryOperator(BinaryOperator.NotEqual, PrimitiveTypeSymbol.String, PrimitiveTypeSymbol.Boolean),
        };
    }

    private BoundBinaryOperator(BinaryOperator op, TypeSymbol type)
        : this(op, type, type, type)
    {
    }

    private BoundBinaryOperator(BinaryOperator op, TypeSymbol type, TypeSymbol resultType)
        : this(op, type, type, resultType)
    {
    }

    private BoundBinaryOperator(BinaryOperator op, TypeSymbol left, TypeSymbol right, TypeSymbol resultType)
    {
        Operator = op;
        Left = left ?? throw new ArgumentNullException(nameof(left));
        Right = right ?? throw new ArgumentNullException(nameof(right));
        Type = resultType ?? throw new ArgumentNullException(nameof(resultType));
    }

    public static BoundBinaryOperator? Bind(BinaryOperator kind, TypeSymbol left, TypeSymbol right)
    {
        foreach (var op in _operators)
        {
            if (op.Operator == kind && op.Left == left && op.Right == right)
            {
                return op;
            }
        }

        return null;
    }
}