namespace Mew.CodeAnalysis;

public sealed class ExpressionStatement : StatementSyntax
{
    public Syntax Expression { get; }
    public override TextSpan Span => Expression.Span;
    public override bool IsValid => Expression.IsValid;

    public ExpressionStatement(SyntaxTree syntaxTree, Syntax expression)
        : base(syntaxTree)
    {
        Expression = expression ?? throw new ArgumentNullException(nameof(expression));
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitExpression(this, context);
    }
}
