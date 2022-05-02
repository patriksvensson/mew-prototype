namespace Mew.CodeAnalysis;

public sealed class LetStatement : StatementSyntax
{
    public SyntaxToken Keyword { get; }
    public IdentifierExpression Name { get; }
    public Syntax EqualToken { get; }
    public Syntax Initializer { get; }
    public Syntax Semicolon { get; }

    public override TextSpan Span => TextSpan.Between(Keyword, Initializer ?? (IPositionable)Name);
    public override bool IsValid => Keyword.IsValid && Name.IsValid && (EqualToken?.IsValid ?? true) && (Initializer?.IsValid ?? true) && Semicolon.IsValid;

    public LetStatement(SyntaxTree syntaxTree, SyntaxToken letToken, IdentifierExpression name, Syntax equalToken, Syntax initializer, Syntax semicolon)
        : base(syntaxTree)
    {
        EnsureTokenType(letToken, SyntaxTokenKind.Let);
        EnsureSyntaxType(name, new[] { typeof(IdentifierExpression) });
        EnsureSyntaxType(equalToken, new[] { typeof(SyntaxToken), typeof(RecoverySyntax) });
        EnsureSyntaxType(initializer, new[] { typeof(ExpressionSyntax), typeof(RecoverySyntax) });
        EnsureSyntaxType(semicolon, new[] { typeof(SyntaxToken), typeof(RecoverySyntax) });

        Keyword = letToken ?? throw new ArgumentNullException(nameof(letToken));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        EqualToken = equalToken ?? throw new ArgumentNullException(nameof(equalToken));
        Initializer = initializer ?? throw new ArgumentNullException(nameof(initializer));
        Semicolon = semicolon ?? throw new ArgumentNullException(nameof(semicolon));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitLet(this, context);
    }
}