namespace Mew.CodeAnalysis;

public sealed class GroupExpression : ExpressionSyntax
{
    public SyntaxToken Left { get; }
    public ExpressionSyntax Expression { get; }
    public SyntaxToken Right { get; }

    public override TextSpan Span => TextSpan.Between(Left, Right);
    public override bool IsValid => Left.IsValid && Expression.IsValid && Right.IsValid;

    public GroupExpression(SyntaxTree syntaxTree, SyntaxToken left, ExpressionSyntax expression, SyntaxToken right)
        : base(syntaxTree)
    {
        Left = left;
        Expression = expression;
        Right = right;
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitGroup(this, context);
    }
}
