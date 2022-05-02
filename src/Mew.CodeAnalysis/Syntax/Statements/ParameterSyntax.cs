namespace Mew.CodeAnalysis;

public sealed class ParameterSyntax : Syntax
{
    public IdentifierExpression Name { get; }
    public Syntax Colon { get; }
    public IdentifierExpression Type { get; }

    public override TextSpan Span => TextSpan.Between(Name, Type);
    public override bool IsValid { get; }

    public ParameterSyntax(SyntaxTree syntaxTree, IdentifierExpression name, Syntax colon, IdentifierExpression type)
        : base(syntaxTree)
    {
        EnsureSyntaxType(Name, new[] { typeof(IdentifierExpression), typeof(RecoverySyntax) });
        EnsureSyntaxType(Name, new[] { typeof(SyntaxToken), typeof(RecoverySyntax) });
        EnsureSyntaxType(Type, new[] { typeof(IdentifierExpression), typeof(RecoverySyntax) });

        Name = name ?? throw new ArgumentNullException(nameof(name));
        Colon = colon ?? throw new ArgumentNullException(nameof(colon));
        Type = type ?? throw new ArgumentNullException(nameof(type));

        IsValid = Name.IsValid && Colon.IsValid && Type.IsValid;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitParameter(this, context);
    }
}
