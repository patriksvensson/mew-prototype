namespace Mew.CodeAnalysis;

public sealed class LogicalExpression : ExpressionSyntax
{
    public ExpressionSyntax Left { get; }
    public ExpressionSyntax Right { get; }
    public SyntaxToken OperatorToken { get; }
    public LogicalOperator Operator { get; }

    public override TextSpan Span => TextSpan.Between(Left, Right);
    public override bool IsValid => Left.IsValid && OperatorToken.IsValid && Right.IsValid;

    public LogicalExpression(
        SyntaxTree syntaxTree,
        ExpressionSyntax left,
        SyntaxToken @operator,
        ExpressionSyntax right)
            : base(syntaxTree)
    {
        Left = left ?? throw new ArgumentNullException(nameof(left));
        OperatorToken = @operator ?? throw new ArgumentNullException(nameof(@operator));
        Right = right ?? throw new ArgumentNullException(nameof(right));

        Operator = Operators.Shared.GetLogicalOperator(OperatorToken);
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitLogical(this, context);
    }
}
