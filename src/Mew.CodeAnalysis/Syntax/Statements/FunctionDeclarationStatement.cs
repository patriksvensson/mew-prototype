namespace Mew.CodeAnalysis;

public sealed class FunctionDeclarationStatement : StatementSyntax, IFunctionDeclarationSyntax
{
    public SyntaxToken Keyword { get; }
    public IdentifierExpression Name { get; }
    public Syntax LeftParen { get; }
    public ImmutableArray<Syntax> Parameters { get; }
    public Syntax RightParen { get; }
    public Syntax? ReturnType { get; }
    public SyntaxToken? Arrow { get; }
    public Syntax Body { get; }

    public override TextSpan Span => TextSpan.Between(Keyword, Body);
    public override bool IsValid { get; }

    public FunctionDeclarationStatement(
        SyntaxTree syntaxTree,
        SyntaxToken keyword,
        IdentifierExpression name,
        Syntax leftParen,
        ImmutableArray<Syntax> parameters,
        Syntax rightParen,
        SyntaxToken? arrowToken,
        Syntax? returnType,
        Syntax body)
            : base(syntaxTree)
    {
        EnsureTokenType(keyword, SyntaxTokenKind.Fn);
        EnsureSyntaxType(name, new[] { typeof(IdentifierExpression) });
        EnsureSyntaxType(leftParen, new[] { typeof(SyntaxToken), typeof(RecoverySyntax) });
        EnsureSyntaxType(parameters, new[] { typeof(ParameterSyntax), typeof(RecoverySyntax) });
        EnsureSyntaxType(returnType, new[] { typeof(IdentifierExpression), typeof(RecoverySyntax) });
        EnsureSyntaxType(rightParen, new[] { typeof(SyntaxToken), typeof(RecoverySyntax) });

        Keyword = keyword ?? throw new ArgumentNullException(nameof(keyword));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        LeftParen = leftParen ?? throw new ArgumentNullException(nameof(leftParen));
        Parameters = parameters;
        RightParen = rightParen ?? throw new ArgumentNullException(nameof(rightParen));
        Arrow = arrowToken;
        ReturnType = returnType;
        Body = body ?? throw new ArgumentNullException(nameof(body));

        IsValid = Keyword.IsValid && Name.IsValid && leftParen.IsValid
            && Parameters.All(p => p.IsValid) && RightParen.IsValid
            && (Arrow?.IsValid ?? true) && (ReturnType?.IsValid ?? true)
            && Body.IsValid;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitFunctionDeclaration(this, context);
    }
}
