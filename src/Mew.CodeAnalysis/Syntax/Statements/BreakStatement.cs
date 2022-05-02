namespace Mew.CodeAnalysis;

public sealed class BreakStatement : StatementSyntax
{
    public SyntaxToken Keyword { get; }

    public override TextSpan Span => Keyword.Span;
    public override bool IsValid => Keyword.IsValid;

    public BreakStatement(SyntaxTree syntaxTree, SyntaxToken keyword)
        : base(syntaxTree)
    {
        EnsureTokenType(keyword, SyntaxTokenKind.Break);
        Keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitBreak(this, context);
    }
}
