namespace Mew.CodeAnalysis;

public sealed class SyntaxTrivia : IPositionable
{
    public SyntaxTriviaKind Kind { get; }
    public string? Lexeme { get; }
    public TextSpan Span { get; }

    public SyntaxTrivia(TextSpan span, SyntaxTriviaKind kind, string? lexeme)
    {
        Span = span;
        Kind = kind;
        Lexeme = lexeme;
    }
}
