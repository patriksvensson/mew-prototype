namespace Mew.CodeAnalysis;

public sealed class ExternalFunctionDeclarationStatement : StatementSyntax, IFunctionDeclarationSyntax
{
    public SyntaxToken ExternalKeyword { get; }
    public SyntaxToken FunctionKeyword { get; }
    public IdentifierExpression Name { get; }
    public Syntax LeftParen { get; }
    public ImmutableArray<Syntax> Parameters { get; }
    public Syntax RightParen { get; }
    public SyntaxToken? Arrow { get; }
    public Syntax? ReturnType { get; }

    public override TextSpan Span { get; }
    public override bool IsValid { get; }

    public ExternalFunctionDeclarationStatement(
        SyntaxTree syntaxTree,
        SyntaxToken externalKeyword,
        SyntaxToken functionKeyword,
        IdentifierExpression name,
        Syntax leftParen,
        ImmutableArray<Syntax> parameters,
        Syntax rightParen,
        SyntaxToken? arrowToken,
        Syntax? returnType)
            : base(syntaxTree)
    {
        EnsureTokenType(ExternalKeyword, SyntaxTokenKind.Extern);
        EnsureTokenType(FunctionKeyword, SyntaxTokenKind.Fn);
        EnsureSyntaxType(name, new[] { typeof(IdentifierExpression) });
        EnsureSyntaxType(leftParen, new[] { typeof(SyntaxToken), typeof(RecoverySyntax) });
        EnsureSyntaxType(parameters, new[] { typeof(ParameterSyntax), typeof(RecoverySyntax) });
        EnsureSyntaxType(returnType, new[] { typeof(IdentifierExpression), typeof(RecoverySyntax) });
        EnsureSyntaxType(rightParen, new[] { typeof(SyntaxToken), typeof(RecoverySyntax) });

        ExternalKeyword = externalKeyword ?? throw new ArgumentNullException(nameof(externalKeyword));
        FunctionKeyword = functionKeyword ?? throw new ArgumentNullException(nameof(functionKeyword));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        LeftParen = leftParen ?? throw new ArgumentNullException(nameof(leftParen));
        Parameters = parameters;
        RightParen = rightParen ?? throw new ArgumentNullException(nameof(rightParen));
        Arrow = arrowToken;
        ReturnType = returnType;

        Span = TextSpan.Between(ExternalKeyword, RightParen);

        IsValid = ExternalKeyword.IsValid && FunctionKeyword.IsValid
            && Name.IsValid && LeftParen.IsValid
            && LeftParen.IsValid && Parameters.All(p => p.IsValid)
            && RightParen.IsValid
            && (Arrow?.IsValid ?? true) && (ReturnType?.IsValid ?? true);

        if ((Arrow == null && ReturnType != null) ||
            (ReturnType == null && Arrow != null))
        {
            IsValid = false;
        }
    }

    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitExternalFunctionDeclaration(this, context);
    }
}
