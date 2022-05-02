namespace Mew.CodeAnalysis;

public sealed class ContinueStatement : StatementSyntax
{
    public SyntaxToken Keyword { get; }

    public override TextSpan Span => Keyword.Span;
    public override bool IsValid => Keyword.IsValid;

    public ContinueStatement(SyntaxTree syntaxTree, SyntaxToken keyword)
        : base(syntaxTree)
    {
        EnsureTokenType(keyword, SyntaxTokenKind.Continue);
        Keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitContinue(this, context);
    }
}
