namespace Mew.CodeAnalysis;

public sealed class BinaryExpression : ExpressionSyntax
{
    public ExpressionSyntax Left { get; }
    public ExpressionSyntax Right { get; }

    public override TextSpan Span => TextSpan.Between(Left, Right);

    public SyntaxToken OperatorToken { get; }
    public BinaryOperator Operator { get; }
    public override bool IsValid => Left.IsValid && OperatorToken.IsValid && Right.IsValid;

    public BinaryExpression(
        SyntaxTree syntaxTree,
        ExpressionSyntax left,
        SyntaxToken @operator,
        ExpressionSyntax right)
            : base(syntaxTree)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        OperatorToken = @operator ?? throw new ArgumentNullException(nameof(@operator));
        Right = right ?? throw new ArgumentNullException(nameof(right));

        Operator = Operators.Shared.GetBinaryOperator(OperatorToken);
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitBinary(this, context);
    }
}
