namespace Mew.CodeAnalysis;

internal sealed class Operators
{
    public static Operators Shared { get; } = new Operators();

    public BinaryOperator GetBinaryOperator(SyntaxToken operatorToken)
    {
        return operatorToken.Kind switch
        {
            SyntaxTokenKind.Plus => BinaryOperator.Add,
            SyntaxTokenKind.Minus => BinaryOperator.Subtract,
            SyntaxTokenKind.Star => BinaryOperator.Multiply,
            SyntaxTokenKind.Slash => BinaryOperator.Divide,
            SyntaxTokenKind.Percent => BinaryOperator.Modolu,
            SyntaxTokenKind.BangEqual => BinaryOperator.NotEqual,
            SyntaxTokenKind.EqualEqual => BinaryOperator.Equal,
            SyntaxTokenKind.Greater => BinaryOperator.GreaterThan,
            SyntaxTokenKind.GreaterEqual => BinaryOperator.GreaterThanOrEqual,
            SyntaxTokenKind.Less => BinaryOperator.LessThan,
            SyntaxTokenKind.LessEqual => BinaryOperator.LessThanOrEqual,
            _ => throw new InvalidOperationException("Unknown binary operator"),
        };
    }

    public UnaryOperator GetUnaryOperator(SyntaxToken operatorToken)
    {
        return operatorToken.Kind switch
        {
            SyntaxTokenKind.Bang => UnaryOperator.Negate,
            SyntaxTokenKind.Minus => UnaryOperator.Negative,
            _ => throw new InvalidOperationException("Unknown logical operator"),
        };
    }

    public LogicalOperator GetLogicalOperator(SyntaxToken operatorToken)
    {
        return operatorToken.Kind switch
        {
            SyntaxTokenKind.And => LogicalOperator.And,
            SyntaxTokenKind.Or => LogicalOperator.Or,
            _ => throw new InvalidOperationException("Unknown logical operator"),
        };
    }
}
