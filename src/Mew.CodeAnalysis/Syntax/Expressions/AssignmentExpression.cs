namespace Mew.CodeAnalysis;

public sealed class AssignmentExpression : ExpressionSyntax
{
    public IdentifierExpression Name { get; }
    public Syntax EqualToken { get; }
    public Syntax Expression { get; }

    public override TextSpan Span => TextSpan.Between(Name, Expression);
    public override bool IsValid => Name.IsValid && EqualToken.IsValid && Expression.IsValid;

    public AssignmentExpression(SyntaxTree syntaxTree, IdentifierExpression name, Syntax equalsToken, Syntax expression)
        : base(syntaxTree)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        EqualToken = equalsToken ?? throw new ArgumentNullException(nameof(equalsToken));
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitAssignment(this, context);
    }
}
