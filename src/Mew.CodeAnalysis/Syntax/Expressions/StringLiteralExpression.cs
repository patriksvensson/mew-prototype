namespace Mew.CodeAnalysis;

public sealed class StringLiteralExpression : LiteralExpression<string>
{
    public StringLiteralExpression(SyntaxTree syntaxTree, SyntaxToken token, string literal)
        : base(syntaxTree, token, literal)
    {
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitStringLiteral(this, context);
    }
}
