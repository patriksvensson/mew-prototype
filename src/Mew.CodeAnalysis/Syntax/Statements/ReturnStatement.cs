namespace Mew.CodeAnalysis;

public sealed class ReturnStatement : StatementSyntax
{
    public SyntaxToken Keyword { get; }
    public Syntax? Expression { get; }

    public override TextSpan Span => TextSpan.Between(Keyword, Expression);
    public override bool IsValid => Keyword.IsValid && (Expression?.IsValid ?? true);

    public ReturnStatement(SyntaxTree syntaxTree, SyntaxToken keyword, Syntax? value)
        : base(syntaxTree)
    {
        EnsureTokenType(keyword, SyntaxTokenKind.Return);

        Keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
        Expression = value;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitReturn(this, context);
    }
}