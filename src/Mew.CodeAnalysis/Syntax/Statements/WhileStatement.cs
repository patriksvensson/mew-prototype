namespace Mew.CodeAnalysis;

public sealed class WhileStatement : StatementSyntax
{
    public SyntaxToken Keyword { get; }
    public Syntax Condition { get; }
    public Syntax Body { get; }

    public override TextSpan Span { get; }
    public override bool IsValid { get; }

    public WhileStatement(SyntaxTree syntaxTree, SyntaxToken keyword, Syntax condition, Syntax body)
        : base(syntaxTree)
    {
        EnsureTokenType(keyword, SyntaxTokenKind.While);
        EnsureSyntaxType(condition, new[] { typeof(ExpressionSyntax), typeof(RecoverySyntax) });
        EnsureSyntaxType(body, new[] { typeof(StatementSyntax), typeof(RecoverySyntax) });

        Keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        Body = body ?? throw new ArgumentNullException(nameof(body));

        Span = TextSpan.Between(Keyword, Body);
        IsValid = Keyword.IsValid && Condition.IsValid && Body.IsValid;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitWhile(this, context);
    }
}
