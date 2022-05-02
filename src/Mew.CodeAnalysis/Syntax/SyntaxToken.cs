namespace Mew.CodeAnalysis;

[DebuggerDisplay("{DebuggerString(),nq}")]
public sealed class SyntaxToken : Syntax
{
    public SyntaxTokenKind Kind { get; }
    public string Lexeme { get; }
    public string? Literal { get; }

    public ImmutableArray<SyntaxTrivia> LeadingTrivia { get; }
    public ImmutableArray<SyntaxTrivia> TrailingTrivia { get; }

    public override bool IsValid { get; } = true;
    public override TextSpan Span { get; }

    public SyntaxToken(
        SyntaxTree syntaxTree,
        TextSpan span, SyntaxTokenKind kind, string lexeme, string? literal,
        ImmutableArray<SyntaxTrivia> leadingTrivia, ImmutableArray<SyntaxTrivia> trailingTrivia)
            : base(syntaxTree)
    {
        Span = span;
        Kind = kind;
        Lexeme = lexeme ?? throw new ArgumentNullException(nameof(lexeme));
        Literal = literal;
        LeadingTrivia = leadingTrivia;
        TrailingTrivia = trailingTrivia;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitToken(this, context);
    }

    private string DebuggerString()
    {
        if (string.IsNullOrWhiteSpace(Lexeme))
        {
            return $"{Kind}";
        }

        return $"{Kind}: {Lexeme}";
    }
}
