namespace Mew.CodeAnalysis;

public sealed class UnaryExpression : ExpressionSyntax
{
    public SyntaxToken OperatorToken { get; }
    public UnaryOperator Operator { get; }
    public Syntax Expression { get; set; }

    public override TextSpan Span => TextSpan.Between(OperatorToken, Expression);
    public override bool IsValid => OperatorToken.IsValid && Expression.IsValid;

    public UnaryExpression(SyntaxTree syntaxTree, SyntaxToken @operator, Syntax expression)
        : base(syntaxTree)
    {
        EnsureAnyTokenType(@operator, new[] { SyntaxTokenKind.Bang, SyntaxTokenKind.Minus });
        EnsureSyntaxType(expression, new[] { typeof(ExpressionSyntax), typeof(RecoverySyntax) });

        OperatorToken = @operator ?? throw new ArgumentNullException(nameof(@operator));
        Operator = Operators.Shared.GetUnaryOperator(OperatorToken);
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitUnary(this, context);
    }
}
