namespace Mew.CodeAnalysis;

public sealed class BooleanLiteralExpression : LiteralExpression<bool>
{
    public BooleanLiteralExpression(SyntaxTree syntaxTree, SyntaxToken token, bool literal)
        : base(syntaxTree, token, literal)
    {
    }

    [DebuggerStepThrough]
    public override void Accept<TContext>(ISyntaxVisitor<TContext> visitor, TContext context)
    {
        visitor.VisitBooleanLiteral(this, context);
    }
}
