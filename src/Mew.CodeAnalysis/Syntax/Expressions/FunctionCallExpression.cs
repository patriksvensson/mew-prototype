namespace Mew.CodeAnalysis;

public sealed class FunctionCallExpression : ExpressionSyntax
{
    public IdentifierExpression Name { get; }
    public Syntax LeftParen { get; }
    public Syntax RightParen { get; }
    public ImmutableArray<Syntax> Arguments { get; }

    public override TextSpan Span => TextSpan.Between(Name, RightParen);
    public override bool IsValid => Name.IsValid && LeftParen.IsValid && RightParen.IsValid && Arguments.All(x => x.IsValid);

    public FunctionCallExpression(
        SyntaxTree syntaxTree,
        IdentifierExpression name,
        Syntax leftParen,
        Syntax rightParen,
        ImmutableArray<Syntax> arguments)
            : base(syntaxTree)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        LeftParen = leftParen ?? throw new ArgumentNullException(nameof(leftParen));
        RightParen = rightParen ?? throw new ArgumentNullException(nameof(rightParen));
        Arguments = arguments;
    }

    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitFunctionCall(this, context);
    }
}
