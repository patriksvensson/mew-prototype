namespace Mew.CodeAnalysis;

public sealed class IfStatement : StatementSyntax
{
    public SyntaxToken Keyword { get; }
    public Syntax Condition { get; }
    public Syntax ThenBranch { get; }
    public SyntaxToken? ElseToken { get; }
    public Syntax? ElseBranch { get; }

    public override TextSpan Span => TextSpan.Between(Keyword, ElseBranch ?? ThenBranch);
    public override bool IsValid => Keyword.IsValid && Condition.IsValid && ThenBranch.IsValid && (ElseToken?.IsValid ?? true) && (ElseBranch?.IsValid ?? true);

    public IfStatement(SyntaxTree syntaxTree, SyntaxToken keyword, Syntax condition, Syntax thenBranch, SyntaxToken? elseToken, Syntax? elseBranch)
        : base(syntaxTree)
    {
        EnsureTokenType(keyword, SyntaxTokenKind.If);
        EnsureSyntaxType(condition, new[] { typeof(ExpressionSyntax), typeof(RecoverySyntax) });
        EnsureSyntaxType(thenBranch, new[] { typeof(StatementSyntax), typeof(RecoverySyntax) });
        EnsureTokenType(elseToken, SyntaxTokenKind.Else);
        EnsureSyntaxType(elseBranch, new[] { typeof(StatementSyntax), typeof(RecoverySyntax) });

        Keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        ThenBranch = thenBranch ?? throw new ArgumentNullException(nameof(thenBranch));
        ElseToken = elseToken;
        ElseBranch = elseBranch;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitIf(this, context);
    }
}
