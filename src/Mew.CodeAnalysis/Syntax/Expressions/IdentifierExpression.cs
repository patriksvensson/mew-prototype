namespace Mew.CodeAnalysis;

public sealed class IdentifierExpression : ExpressionSyntax
{
    public Syntax Child { get; }

    public override bool IsValid => Child is SyntaxToken;

    public string IdentifierName
    {
        get
        {
            return Child switch
            {
                SyntaxToken identifier => identifier.Lexeme,
                RecoverySyntax recovery => recovery.Tokens.Count > 0 ? "$error" : "$missing",
                _ => throw new NotSupportedException($"Unexpected child node type '{Child.GetType().Name}'."),
            };
        }
    }

    public override TextSpan Span => Child.Span;

    public IdentifierExpression(SyntaxTree syntaxTree, Syntax child)
        : base(syntaxTree)
    {
        Child = child ?? throw new ArgumentNullException(nameof(child));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitIdentifier(this, context);
    }
}
