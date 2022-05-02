namespace Mew.CodeAnalysis;

public abstract class LiteralExpression<T> : ExpressionSyntax
{
    public SyntaxToken Token { get; }
    public T Value { get; }

    public override TextSpan Span => Token.Span;
    public override bool IsValid => Token.IsValid;

    protected LiteralExpression(SyntaxTree syntaxTree, SyntaxToken token, T literal)
        : base(syntaxTree)
    {
        Token = token ?? throw new ArgumentNullException(nameof(token));
        Value = literal;
    }
}
